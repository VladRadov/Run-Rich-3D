using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using RunRich3D.Models;
using RunRich3D.Services;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class PickupEffectController : IDisposable
    {
        private readonly PrefabPool<PooledParticleEffectView> _gainPool;
        private readonly PrefabPool<PooledParticleEffectView> _lossPool;
        private readonly PlayerView _playerView;
        private readonly ILevelEvents _events;
        private readonly PlayerModel _player;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();
        private CancellationTokenSource _cts = new CancellationTokenSource();

        internal PickupEffectController(
            PrefabPool<PooledParticleEffectView> gainPool,
            PrefabPool<PooledParticleEffectView> lossPool,
            PlayerView playerView,
            ILevelEvents events,
            PlayerModel player)
        {
            _gainPool = gainPool;
            _lossPool = lossPool;
            _playerView = playerView;
            _events = events;
            _player = player;
        }

        internal void Initialize()
        {
            if (_events != null)
            {
                if (_gainPool != null)
                {
                    _events.WealthGained
                        .Subscribe(_ => Play(_gainPool))
                        .AddTo(_disposables);
                }

                if (_lossPool != null)
                {
                    _events.WealthLost
                        .Subscribe(_ => Play(_lossPool))
                        .AddTo(_disposables);
                }
            }

            if (_player != null)
            {
                _player.Phase
                    .Where(phase => phase == GamePhase.WaitingToStart)
                    .Subscribe(_ => ResetEffects())
                    .AddTo(_disposables);
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
            CancelToken();
            _gainPool?.ReleaseAll();
            _lossPool?.ReleaseAll();
        }

        private void Play(PrefabPool<PooledParticleEffectView> pool)
        {
            if (pool == null || _playerView == null || _cts == null)
            {
                return;
            }

            PooledParticleEffectView view = pool.Get();
            view.Play(_playerView.EffectWorldCenter);
            ReleaseWhenDone(pool, view, _cts.Token).Forget();
        }

        private async UniTaskVoid ReleaseWhenDone(
            PrefabPool<PooledParticleEffectView> pool,
            PooledParticleEffectView view,
            CancellationToken token)
        {
            try
            {
                float seconds = view != null && view.Duration > 0.05f ? view.Duration : 1.5f;
                await UniTask.Delay(TimeSpan.FromSeconds(seconds), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                if (view != null)
                {
                    view.StopAndClear();
                    pool.Release(view);
                }
            }
        }

        private void ResetEffects()
        {
            CancelToken();
            _cts = new CancellationTokenSource();
            _gainPool?.ReleaseAll();
            _lossPool?.ReleaseAll();
        }

        private void CancelToken()
        {
            if (_cts == null)
            {
                return;
            }

            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
