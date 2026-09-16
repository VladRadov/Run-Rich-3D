using System;
using UniRx;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class LevelController : IDisposable, ILevelEvents
    {
        private readonly float _flagRaiseStart;
        private readonly float _flagRaiseEnd;
        private readonly LevelModel _model;
        private readonly PlayerModel _player;
        private readonly LevelPieceView[] _pickupViews;
        private readonly FlagView[] _flagViews;
        private readonly GateView _gateView;
        private readonly Subject<int> _finishReached = new Subject<int>();
        private readonly Subject<int> _moneyCollected = new Subject<int>();
        private readonly Subject<int> _wealthGained = new Subject<int>();
        private readonly Subject<int> _wealthLost = new Subject<int>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        internal LevelController(
            LevelModel model,
            PlayerModel player,
            LevelPieceView[] pickupViews,
            FlagView[] flagViews,
            GateView gateView,
            float flagRaiseStart,
            float flagRaiseEnd)
        {
            _model = model;
            _player = player;
            _pickupViews = pickupViews;
            _flagViews = flagViews;
            _gateView = gateView;
            _flagRaiseStart = flagRaiseStart;
            _flagRaiseEnd = flagRaiseEnd;
        }

        public IObservable<int> FinishReached => _finishReached;
        public IObservable<int> PickupCollected => _moneyCollected;
        public IObservable<int> WealthGained => _wealthGained;
        public IObservable<int> WealthLost => _wealthLost;

        internal void Initialize()
        {
            Observable.EveryUpdate()
                .Subscribe(_ => Evaluate(_player.LateralOffset.Value, _player.ForwardPosition.Value))
                .AddTo(_disposables);

            _player.Phase
                .Where(phase => phase == GamePhase.WaitingToStart)
                .Subscribe(_ => ResetRun())
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _finishReached.OnCompleted();
            _finishReached.Dispose();
            _moneyCollected.OnCompleted();
            _moneyCollected.Dispose();
            _wealthGained.OnCompleted();
            _wealthGained.Dispose();
            _wealthLost.OnCompleted();
            _wealthLost.Dispose();
        }

        private void Evaluate(float x, float z)
        {
            UpdateFlags(z);
            if (_player.Phase.Value != GamePhase.Playing)
            {
                return;
            }

            TryCollectPickups(x, z);
            TryHitObstacles(x, z);
            TryPassGate(x, z);
            TryFinish(x, z);
        }

        private void TryCollectPickups(float x, float z)
        {
            PickupRecord[] pickups = _model.Pickups;
            for (int i = 0; i < pickups.Length; i++)
            {
                PickupRecord pickup = pickups[i];
                if (!pickup.Overlaps(x, z))
                {
                    continue;
                }

                pickup.Consume();
                _player.AddWealth(pickup.WealthDelta);
                if (pickup.WealthDelta != 0)
                {
                    _moneyCollected.OnNext(pickup.WealthDelta);
                }

                NotifyWealthDelta(pickup.WealthDelta);

                if (i < _pickupViews.Length)
                {
                    _pickupViews[i].SetVisible(false);
                }
            }
        }

        private void TryHitObstacles(float x, float z)
        {
            ObstacleRecord[] obstacles = _model.Obstacles;
            for (int i = 0; i < obstacles.Length; i++)
            {
                ObstacleRecord obstacle = obstacles[i];
                if (!obstacle.Overlaps(x, z))
                {
                    continue;
                }

                obstacle.Consume();
                _player.AddWealth(obstacle.WealthPenalty);
            }
        }

        private void TryPassGate(float x, float z)
        {
            GateRecord gate = _model.Gate;
            if (!gate.Overlaps(z))
            {
                return;
            }

            gate.Consume();
            int wealthDelta = gate.WealthDeltaFor(x);
            _player.AddWealth(wealthDelta);
            NotifyWealthDelta(wealthDelta);
            if (_gateView != null)
            {
                _gateView.HidePassedSide(x <= 0f);
            }
        }

        private void TryFinish(float x, float z)
        {
            FinishRecord finish = _model.Finish;
            if (!finish.Reached(z))
            {
                return;
            }

            finish.Consume();
            int multiplier = finish.MultiplierFor(x);
            _player.ApplyMultiplier(multiplier);
            _finishReached.OnNext(multiplier);
        }

        private void UpdateFlags(float forward)
        {
            if (_flagViews == null)
            {
                return;
            }

            for (int i = 0; i < _flagViews.Length; i++)
            {
                _flagViews[i].SetRaised(RaiseAmount(forward, _flagViews[i].TriggerZ));
            }
        }

        private float RaiseAmount(float playerZ, float flagZ)
        {
            float ahead = flagZ - playerZ;
            if (ahead <= _flagRaiseEnd)
            {
                return 1f;
            }

            if (ahead >= _flagRaiseStart)
            {
                return 0f;
            }

            return 1f - (ahead - _flagRaiseEnd) / (_flagRaiseStart - _flagRaiseEnd);
        }

        private void NotifyWealthDelta(int wealthDelta)
        {
            if (wealthDelta > 0)
            {
                _wealthGained.OnNext(wealthDelta);
            }
            else if (wealthDelta < 0)
            {
                _wealthLost.OnNext(wealthDelta);
            }
        }

        private void ResetRun()
        {
            _model.ResetRun();
            if (_gateView != null)
            {
                _gateView.ShowAll();
            }

            for (int i = 0; i < _pickupViews.Length; i++)
            {
                _pickupViews[i].SetVisible(true);
            }

            if (_flagViews == null)
            {
                return;
            }

            for (int i = 0; i < _flagViews.Length; i++)
            {
                _flagViews[i].SetRaised(0f);
            }
        }
    }
}
