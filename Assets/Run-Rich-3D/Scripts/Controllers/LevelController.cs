using System;
using UniRx;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class LevelController : IDisposable, ILevelEvents
    {
        private readonly LevelModel _model;
        private readonly PlayerModel _player;
        private readonly LevelPieceView[] _pickupViews;
        private readonly GateView _gateView;
        private readonly FinishDoorsView _finishDoors;
        private readonly LevelPresentationController _presentation;
        private readonly int _minDoorMultiplier;
        private readonly Subject<int> _finishReached = new Subject<int>();
        private readonly Subject<int> _moneyCollected = new Subject<int>();
        private readonly Subject<int> _wealthGained = new Subject<int>();
        private readonly Subject<int> _wealthLost = new Subject<int>();
        private readonly Subject<Unit> _flagRaised = new Subject<Unit>();
        private readonly Subject<Unit> _doorOpened = new Subject<Unit>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public LevelController(
            LevelModel model,
            PlayerModel player,
            LevelPieceView[] pickupViews,
            FlagView[] flagViews,
            GateView gateView,
            FinishDoorsView finishDoors,
            float flagRaiseStart,
            float flagRaiseEnd,
            float flagRaiseDetectThreshold,
            int minDoorMultiplier)
        {
            _model = model;
            _player = player;
            _pickupViews = pickupViews;
            _gateView = gateView;
            _finishDoors = finishDoors;
            _minDoorMultiplier = minDoorMultiplier < 2 ? 2 : minDoorMultiplier;
            _presentation = new LevelPresentationController(flagViews, finishDoors, flagRaiseStart, flagRaiseEnd, flagRaiseDetectThreshold);
        }

        public IObservable<int> FinishReached => _finishReached;
        public IObservable<int> PickupCollected => _moneyCollected;
        public IObservable<int> WealthGained => _wealthGained;
        public IObservable<int> WealthLost => _wealthLost;
        public IObservable<Unit> FlagRaised => _flagRaised;
        public IObservable<Unit> DoorOpened => _doorOpened;

        public void Initialize()
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
            _flagRaised.OnCompleted();
            _flagRaised.Dispose();
            _doorOpened.OnCompleted();
            _doorOpened.Dispose();
        }

        private void Evaluate(float x, float z)
        {
            bool playing = _player.Phase.Value == GamePhase.Playing;
            int flags = _presentation.UpdateFlags(z, playing);
            for (int i = 0; i < flags; i++)
            {
                _flagRaised.OnNext(Unit.Default);
            }

            int doors = _presentation.UpdateDoors(z, playing);
            for (int i = 0; i < doors; i++)
            {
                _doorOpened.OnNext(Unit.Default);
            }

            if (!playing)
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
            float line = finish.Spawn.Z;
            if (_finishDoors != null && _finishDoors.StopForward > line)
            {
                line = _finishDoors.StopForward;
            }

            if (finish.IsConsumed || z < line)
            {
                return;
            }

            finish.Consume();
            int multiplier = _finishDoors != null ? _finishDoors.DoorMultiplier(z) : _minDoorMultiplier;
            _finishReached.OnNext(multiplier);
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

            _presentation.Reset();
        }
    }
}
