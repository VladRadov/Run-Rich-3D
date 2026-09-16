using System;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class PlayerController : IDisposable
    {
        private readonly PlayerModel _model;
        private readonly PlayerView _view;
        private readonly InputModel _input;
        private readonly PlayerSettings _settings;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private PathBend _path;

        private float _dragOriginScreenX;
        private float _dragOriginLateral;
        private float _targetLateral;
        private float _lateralVelocity;
        private float _lastLateral;
        private float _steerYaw;
        private bool _hasLastLateral;

        public PlayerController(
            PlayerModel model,
            PlayerView view,
            InputModel input,
            PlayerSettings settings)
        {
            _model = model;
            _view = view;
            _input = input;
            _settings = settings;
        }

        public void Initialize()
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
                    _view.SetDancing(phase == GamePhase.Win);
                    if (!playing)
                    {
                        _targetLateral = _model.LateralOffset.Value;
                        _lateralVelocity = 0f;
                    }
                })
                .AddTo(_disposables);

            _model.OutfitIndex
                .Subscribe(index =>
                {
                    if (_model.Phase.Value == GamePhase.Win)
                    {
                        return;
                    }

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
                            _model.ForwardPosition.Value + _settings.ForwardSpeed * Time.deltaTime);
                        MoveLaterally();
                    }

                    ApplyPose(_model.LateralOffset.Value, _model.ForwardPosition.Value);
                })
                .AddTo(_disposables);
        }

        public void BindPath(PathBend path)
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
                targetYaw = Mathf.Clamp(speed * _settings.SteerYawPerSpeed, -_settings.MaxSteerYaw, _settings.MaxSteerYaw);
            }

            _lastLateral = lateral;
            _hasLastLateral = true;

            if (dt <= 0.0001f)
            {
                return _steerYaw;
            }

            float blend = 1f - Mathf.Exp(-_settings.SteerYawSmooth * dt);
            _steerYaw = Mathf.Lerp(_steerYaw, targetYaw, blend);
            if (Mathf.Abs(_steerYaw) < _settings.SteerIdleSnapDegrees && Mathf.Abs(targetYaw) < _settings.SteerIdleSnapDegrees)
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
            float worldDelta = normalizedDelta * _settings.PathWidth * _settings.SteerSensitivity;
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
                _settings.LateralSmoothTime);
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
            float slack = _settings.OffPathSlack < 0f ? 0f : _settings.OffPathSlack;
            float half = _settings.PathWidth * 0.5f + slack;
            return half > _settings.MinLateralLimit ? half : _settings.MinLateralLimit;
        }
    }
}
