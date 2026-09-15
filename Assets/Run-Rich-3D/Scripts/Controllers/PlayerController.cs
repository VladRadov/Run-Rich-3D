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
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private PathBend _path;

        private float _dragOriginScreenX;
        private float _dragOriginLateral;

        internal PlayerController(
            PlayerModel model,
            PlayerView view,
            InputModel input,
            float pathWidth,
            float sensitivity,
            float forwardSpeed,
            float offPathSlack)
        {
            _model = model;
            _view = view;
            _input = input;
            _pathWidth = pathWidth;
            _sensitivity = sensitivity;
            _forwardSpeed = forwardSpeed;
            _offPathSlack = offPathSlack;
        }

        internal void Initialize()
        {
            Observable.CombineLatest(
                    _model.LateralOffset,
                    _model.ForwardPosition,
                    (x, z) => new Vector2(x, z))
                .Subscribe(pose => ApplyPose(pose.x, pose.y))
                .AddTo(_disposables);

            _model.Wealth
                .Subscribe(wealth => _view.SetStatus(
                    _model.Rules.TierFrom(wealth),
                    _model.Rules.Normalized(wealth)))
                .AddTo(_disposables);

            _model.Phase
                .Where(phase => phase == GamePhase.WaitingToStart)
                .Subscribe(_ => _view.SetOutfit(_model.OutfitIndex.Value, false))
                .AddTo(_disposables);

            _model.OutfitIndex
                .Subscribe(index =>
                {
                    bool spin = _model.Phase.Value == GamePhase.Playing;
                    _view.SetOutfit(index, spin);
                })
                .AddTo(_disposables);

            _model.GateReskin
                .Subscribe(index => _view.SetOutfit(index, true))
                .AddTo(_disposables);

            _input.Pressed
                .Subscribe(OnPressed)
                .AddTo(_disposables);

            _input.Dragged
                .Subscribe(SteerTo)
                .AddTo(_disposables);

            Observable.EveryUpdate()
                .Where(_ => _model.Phase.Value == GamePhase.Playing)
                .Subscribe(_ =>
                {
                    _model.SetForwardPosition(
                        _model.ForwardPosition.Value + _forwardSpeed * Time.deltaTime);
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
            if (_path == null)
            {
                _view.SetPose(lateral, distance);
                return;
            }

            PathPose pose = _path.Sample(distance, lateral);
            _view.SetPose(new Vector3(pose.X, pose.Y, pose.Z), pose.YawDegrees);
        }

        private void OnPressed(float screenX)
        {
            _dragOriginScreenX = screenX;
            _dragOriginLateral = _model.LateralOffset.Value;

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
            float maxOffset = _pathWidth * 0.5f + _offPathSlack;
            _model.SetLateralOffset(Mathf.Clamp(_dragOriginLateral + worldDelta, -maxOffset, maxOffset));
        }
    }
}
