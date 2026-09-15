using System;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class GameLoopController : IDisposable
    {
        private readonly GameLoopModel _loop;
        private readonly PlayerModel _player;
        private readonly HudView _hud;
        private readonly ILevelEvents _levelEvents;
        private readonly float _loseAbsX;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        internal GameLoopController(
            GameLoopModel loop,
            PlayerModel player,
            HudView hud,
            ILevelEvents levelEvents,
            float loseAbsX)
        {
            _loop = loop;
            _player = player;
            _hud = hud;
            _levelEvents = levelEvents;
            _loseAbsX = loseAbsX;
        }

        internal void Initialize()
        {
            _hud.ActionClicked
                .Subscribe(_ => OnHudAction())
                .AddTo(_disposables);

            _player.Phase
                .Subscribe(OnPhase)
                .AddTo(_disposables);

            _player.Wealth
                .Subscribe(_hud.SetWealth)
                .AddTo(_disposables);

            _loop.LevelNumber
                .Subscribe(level => _hud.SetLevel(FormatLevel(level)))
                .AddTo(_disposables);

            _player.LateralOffset
                .Subscribe(CheckWater)
                .AddTo(_disposables);

            _levelEvents.FinishReached
                .Subscribe(_ => OnFinishReached())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }

        private void OnPhase(GamePhase phase)
        {
            int level = _loop.LevelNumber.Value;
            switch (phase)
            {
                case GamePhase.Win:
                    _hud.ShowResult("ВЫ ПОБЕДИЛИ", FormatLevel(level) + " завершён", "ДАЛЬШЕ", true);
                    break;
                case GamePhase.Lose:
                    _hud.ShowResult("ПОРАЖЕНИЕ", "Попробуйте ещё раз", "ЗАНОВО", false);
                    break;
                default:
                    _hud.ShowRun(FormatLevel(level), _player.Wealth.Value);
                    break;
            }
        }

        private void OnFinishReached()
        {
            if (_player.Phase.Value == GamePhase.Playing)
            {
                _player.SetPhase(GamePhase.Win);
            }
        }

        private void CheckWater(float lateralOffset)
        {
            if (_player.Phase.Value == GamePhase.Playing && Mathf.Abs(lateralOffset) >= _loseAbsX)
            {
                _player.SetPhase(GamePhase.Lose);
            }
        }

        private void OnHudAction()
        {
            GamePhase phase = _player.Phase.Value;
            if (phase == GamePhase.Win)
            {
                _loop.AdvanceLevel();
                _player.Reset(0);
                return;
            }

            if (phase == GamePhase.Lose)
            {
                _player.Reset(0);
            }
        }

        private static string FormatLevel(int level)
        {
            return "Уровень " + level;
        }
    }
}
