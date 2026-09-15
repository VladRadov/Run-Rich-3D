using System;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class LevelController : IDisposable, ILevelEvents
    {
        private readonly LevelModel _model;
        private readonly PlayerModel _player;
        private readonly LevelPieceView[] _pickupViews;
        private readonly Subject<int> _finishReached = new Subject<int>();
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        internal LevelController(
            LevelModel model,
            PlayerModel player,
            LevelPieceView[] pickupViews)
        {
            _model = model;
            _player = player;
            _pickupViews = pickupViews;
        }

        public IObservable<int> FinishReached => _finishReached;

        internal void Initialize()
        {
            Observable.CombineLatest(
                    _player.LateralOffset,
                    _player.ForwardPosition,
                    (x, z) => new Vector2(x, z))
                .Subscribe(Evaluate)
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
        }

        private void Evaluate(Vector2 pose)
        {
            if (_player.Phase.Value != GamePhase.Playing)
            {
                return;
            }

            TryCollectPickups(pose.x, pose.y);
            TryHitObstacles(pose.x, pose.y);
            TryPassGate(pose.x, pose.y);
            TryFinish(pose.x, pose.y);
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
            _player.AddWealth(gate.WealthDeltaFor(x));
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

        private void ResetRun()
        {
            _model.ResetRun();
            for (int i = 0; i < _pickupViews.Length; i++)
            {
                _pickupViews[i].SetVisible(true);
            }
        }
    }
}
