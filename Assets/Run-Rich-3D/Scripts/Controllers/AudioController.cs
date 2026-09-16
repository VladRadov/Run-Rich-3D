using System;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class AudioController : IDisposable
    {
        private readonly AudioView _view;
        private readonly PlayerModel _player;
        private readonly ILevelEvents _events;
        private readonly float _stepInterval;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private float _stepElapsed;
        private bool _skipNextStatus;

        internal AudioController(AudioView view, PlayerModel player, ILevelEvents events, float stepInterval)
        {
            _view = view;
            _player = player;
            _events = events;
            _stepInterval = stepInterval > 0.05f ? stepInterval : 0.52f;
        }

        internal void Initialize()
        {
            if (_view == null)
            {
                return;
            }

            _skipNextStatus = true;
            if (_events != null)
            {
                _events.WealthGained
                    .Subscribe(_ => _view.PlayDollar())
                    .AddTo(_disposables);
                _events.WealthLost
                    .Subscribe(_ => _view.PlayBottle())
                    .AddTo(_disposables);
                _events.FlagRaised
                    .Subscribe(_ => _view.PlayFlag())
                    .AddTo(_disposables);
                _events.DoorOpened
                    .Subscribe(_ => _view.PlayDoor())
                    .AddTo(_disposables);
            }

            if (_player != null)
            {
                _player.OutfitIndex
                    .Subscribe(_ => OnOutfitChanged())
                    .AddTo(_disposables);
                _player.Phase
                    .Subscribe(OnPhase)
                    .AddTo(_disposables);
            }

            Observable.EveryUpdate()
                .Subscribe(_ => TickSteps())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnOutfitChanged()
        {
            if (_skipNextStatus)
            {
                _skipNextStatus = false;
                return;
            }

            if (_player != null && _player.Phase.Value == GamePhase.Playing)
            {
                _view.PlayStatus();
            }
        }

        private void OnPhase(GamePhase phase)
        {
            if (phase == GamePhase.Win)
            {
                _view.PlayWin();
                return;
            }

            if (phase == GamePhase.Lose)
            {
                _view.PlayLose();
            }
        }

        private void TickSteps()
        {
            if (_player == null || _player.Phase.Value != GamePhase.Playing)
            {
                _stepElapsed = 0f;
                return;
            }

            _stepElapsed += Time.deltaTime;
            if (_stepElapsed < _stepInterval)
            {
                return;
            }

            _stepElapsed = 0f;
            _view.PlayFootstep();
        }
    }
}
