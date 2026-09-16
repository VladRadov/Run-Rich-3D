using System;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class PlayerController : IDisposable
    {
        private readonly PlayerModel _model;
        private readonly PlayerView _view;
        private readonly InputModel _input;
        private readonly float _pathWidth;
        private readonly float _sensitivity;
        private readonly float _forwardSpeed;
        private readonly float _offPathSlack;
        private readonly float _maxSteerYaw;
        private readonly float _steerYawPerSpeed;
        private readonly float _steerYawSmooth;
        private readonly float _lateralSmoothTime;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private PathBend _path;

        private float _dragOriginScreenX;
        private float _dragOriginLateral;
        private float _targetLateral;
        private float _lateralVelocity;
        private float _lastLateral;
        private float _steerYaw;
        private bool _hasLastLateral;

        internal PlayerController(
            PlayerModel model,
            PlayerView view,
            InputModel input,
            float pathWidth,
            float sensitivity,
            float forwardSpeed,
            float offPathSlack,
            float maxSteerYaw,
            float steerYawPerSpeed,
            float steerYawSmooth,
            float lateralSmoothTime)
        {
            _model = model;
            _view = view;
            _input = input;
            _pathWidth = pathWidth;
            _sensitivity = sensitivity;
            _forwardSpeed = forwardSpeed;
            _offPathSlack = offPathSlack;
            _maxSteerYaw = maxSteerYaw > 1f ? maxSteerYaw : 42f;
            _steerYawPerSpeed = steerYawPerSpeed > 0.01f ? steerYawPerSpeed : 11f;
            _steerYawSmooth = steerYawSmooth > 0.01f ? steerYawSmooth : 10f;
            _lateralSmoothTime = lateralSmoothTime > 0.01f ? lateralSmoothTime : 0.14f;
        }

        internal void Initialize()
        {
            _model.Wealth
                .Subscribe(wealth => _view.SetStatus(
                    _model.Rules.TierFrom(wealth),
                    _model.Rules.Normalized(wealth)))
                .AddTo(_disposables);

            _model.Phase
                .Subscribe(phase =>
                {
                    bool playing = phase == GamePhase.Playing;
                    _view.SetWalking(playing);
                    if (!playing)
                    {
                        _targetLateral = _model.LateralOffset.Value;
                        _lateralVelocity = 0f;
                        _view.SetOutfit(_model.OutfitIndex.Value, false);
                    }
                })
                .AddTo(_disposables);

            _model.OutfitIndex
                .Subscribe(index =>
                {
                    bool spin = _model.Phase.Value == GamePhase.Playing;
                    _view.SetOutfit(index, spin);
                })
                .AddTo(_disposables);

            _input.Pressed
                .Subscribe(OnPressed)
                .AddTo(_disposables);

            _input.Dragged
                .Subscribe(SteerTo)
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Subscribe(_ =>
                {
                    if (_model.Phase.Value == GamePhase.Playing)
                    {
                        _model.SetForwardPosition(
                            _model.ForwardPosition.Value + _forwardSpeed * Time.deltaTime);
                        MoveLaterally();
                    }

                    ApplyPose(_model.LateralOffset.Value, _model.ForwardPosition.Value);
                })
                .AddTo(_disposables);
        }

        internal void BindPath(PathBend path)
        {
            _path = path;
            ApplyPose(_model.LateralOffset.Value, _model.ForwardPosition.Value);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void ApplyPose(float lateral, float distance)
        {
            float steerYaw = UpdateSteerYaw(lateral);
            if (_path == null)
            {
                _view.SetPose(lateral, distance, steerYaw);
                return;
            }

            PathPose pose = _path.Sample(distance, lateral);
            _view.SetPose(new Vector3(pose.X, pose.Y, pose.Z), pose.YawDegrees, steerYaw);
        }

        private float UpdateSteerYaw(float lateral)
        {
            float dt = Time.deltaTime;
            float targetYaw = 0f;
            if (_hasLastLateral && dt > 0.0001f && _model.Phase.Value == GamePhase.Playing)
            {
                float speed = (lateral - _lastLateral) / dt;
                targetYaw = Mathf.Clamp(speed * _steerYawPerSpeed, -_maxSteerYaw, _maxSteerYaw);
            }

            _lastLateral = lateral;
            _hasLastLateral = true;

            if (dt <= 0.0001f)
            {
                return _steerYaw;
            }

            float blend = 1f - Mathf.Exp(-_steerYawSmooth * dt);
            _steerYaw = Mathf.Lerp(_steerYaw, targetYaw, blend);
            if (Mathf.Abs(_steerYaw) < 0.05f && Mathf.Abs(targetYaw) < 0.05f)
            {
                _steerYaw = 0f;
            }

            return _steerYaw;
        }

        private void OnPressed(float screenX)
        {
            _dragOriginScreenX = screenX;
            _dragOriginLateral = _model.LateralOffset.Value;
            _targetLateral = _dragOriginLateral;

            if (_model.Phase.Value == GamePhase.WaitingToStart)
            {
                _model.SetPhase(GamePhase.Playing);
            }
        }

        private void SteerTo(float screenX)
        {
            if (_model.Phase.Value != GamePhase.Playing)
            {
                return;
            }

            float normalizedDelta = (screenX - _dragOriginScreenX) / Screen.width;
            float worldDelta = normalizedDelta * _pathWidth * _sensitivity;
            float maxOffset = MaxLateralOffset();
            float desired = _dragOriginLateral + worldDelta;
            float clamped = Mathf.Clamp(desired, -maxOffset, maxOffset);
            _targetLateral = clamped;
            if (Mathf.Abs(desired - clamped) > 0.0001f)
            {
                _dragOriginLateral = clamped;
                _dragOriginScreenX = screenX;
            }
        }

        private void MoveLaterally()
        {
            float current = _model.LateralOffset.Value;
            float next = Mathf.SmoothDamp(
                current,
                _targetLateral,
                ref _lateralVelocity,
                _lateralSmoothTime);
            if (Mathf.Abs(next - _targetLateral) < 0.001f)
            {
                next = _targetLateral;
                _lateralVelocity = 0f;
            }

            float maxOffset = MaxLateralOffset();
            _model.SetLateralOffset(Mathf.Clamp(next, -maxOffset, maxOffset));
        }

        private float MaxLateralOffset()
        {
            float slack = _offPathSlack < 0f ? 0f : _offPathSlack;
            float half = _pathWidth * 0.5f + slack;
            return half > 0.1f ? half : 0.1f;
        }
    }
}
