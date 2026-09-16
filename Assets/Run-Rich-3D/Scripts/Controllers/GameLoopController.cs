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

            _hud.WinCollected
                .Subscribe(CollectWin)
                .AddTo(_disposables);

            _player.Phase
                .Subscribe(OnPhase)
                .AddTo(_disposables);

            _player.Wealth
                .Subscribe(_hud.SetRunScore)
                .AddTo(_disposables);

            _loop.LevelNumber
                .Subscribe(level => _hud.SetLevel(FormatLevel(level)))
                .AddTo(_disposables);

            _loop.BankedCoins
                .Subscribe(_hud.SetCoins)
                .AddTo(_disposables);

            _player.LateralOffset
                .Subscribe(CheckWater)
                .AddTo(_disposables);

            _levelEvents.FinishReached
                .Subscribe(OnFinishReached)
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
                    break;
                case GamePhase.Lose:
                    _hud.ShowLose();
                    break;
                case GamePhase.WaitingToStart:
                    _hud.ShowRun(FormatLevel(level), _player.Wealth.Value);
                    _hud.SetSwipeHintVisible(true);
                    _hud.SetSideButtonsVisible(true);
                    _hud.SetWorldProgressVisible(true);
                    _hud.SetCenterStatsVisible(false);
                    break;
                default:
                    _hud.ShowRun(FormatLevel(level), _player.Wealth.Value);
                    _hud.SetSwipeHintVisible(false);
                    _hud.SetSideButtonsVisible(false);
                    _hud.SetWorldProgressVisible(false);
                    _hud.SetCenterStatsVisible(true);
                    break;
            }
        }

        private void OnFinishReached(int doorMultiplier)
        {
            if (_player.Phase.Value != GamePhase.Playing)
            {
                return;
            }

            int multiplier = doorMultiplier < 2 ? 2 : doorMultiplier;
            int reward = _player.Wealth.Value * multiplier;
            _player.SetPhase(GamePhase.Win);
            _hud.ShowWin(_loop.LevelNumber.Value, reward);
        }

        private void CollectWin(int amount)
        {
            _loop.AddBankedCoins(amount);
            _loop.AdvanceLevel();
            _player.Reset(0);
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
            if (_player.Phase.Value == GamePhase.Lose)
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
