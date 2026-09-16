using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Services;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class PickupToastController : IDisposable
    {
        private readonly PickupToastPool _pool;
        private readonly ILevelEvents _levelEvents;
        private readonly PlayerModel _player;
        private readonly float _holdSeconds;
        private readonly float _fadeSeconds;
        private readonly float _risePixels;
        private readonly ToastChannel _gain;
        private readonly ToastChannel _loss;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public PickupToastController(
            PickupToastPool pool,
            ILevelEvents levelEvents,
            PlayerModel player,
            float holdSeconds,
            float fadeSeconds,
            float risePixels,
            Vector2 gainRestPosition,
            Vector2 lossRestPosition)
        {
            _pool = pool;
            _levelEvents = levelEvents;
            _player = player;
            _holdSeconds = holdSeconds > 0.01f ? holdSeconds : 1.1f;
            _fadeSeconds = fadeSeconds > 0.01f ? fadeSeconds : 0.45f;
            _risePixels = risePixels > 0.01f ? risePixels : 90f;
            _gain = new ToastChannel(gainRestPosition);
            _loss = new ToastChannel(lossRestPosition);
        }

        public void Initialize()
        {
            _levelEvents.PickupCollected
                .Subscribe(OnPickupCollected)
                .AddTo(_disposables);

            _player.Phase
                .Subscribe(OnPhase)
                .AddTo(_disposables);
        }

        public void Dispose()
        {
            HideImmediate(_gain);
            HideImmediate(_loss);
            _disposables.Dispose();
        }

        private void OnPickupCollected(int delta)
        {
            if (delta > 0)
            {
                Show(_gain, delta, true);
                return;
            }

            if (delta < 0)
            {
                Show(_loss, -delta, false);
            }
        }

        private void OnPhase(GamePhase phase)
        {
            if (phase != GamePhase.Playing)
            {
                HideImmediate(_gain);
                HideImmediate(_loss);
            }
        }

        private void Show(ToastChannel channel, int magnitude, bool isGain)
        {
            if (channel.View == null)
            {
                channel.View = _pool.Get();
            }

            channel.View.SetRestPosition(channel.RestPosition);
            channel.Model.Add(magnitude);
            int signed = isGain ? channel.Model.Amount.Value : -channel.Model.Amount.Value;
            channel.View.SetDelta(signed);
            channel.View.SetPresentation(1f, 0f);
            RestartHide(channel);
        }

        private void RestartHide(ToastChannel channel)
        {
            CancelHide(channel);
            channel.HideCts = new CancellationTokenSource();
            RunHideAsync(channel, channel.HideCts.Token).Forget();
        }

        private async UniTaskVoid RunHideAsync(ToastChannel channel, CancellationToken token)
        {
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_holdSeconds), cancellationToken: token);
                float elapsed = 0f;
                while (elapsed < _fadeSeconds)
                {
                    elapsed += Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / _fadeSeconds);
                    float eased = t * t * (3f - 2f * t);
                    if (channel.View != null)
                    {
                        channel.View.SetPresentation(1f - eased, _risePixels * eased);
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }

                Recycle(channel);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void HideImmediate(ToastChannel channel)
        {
            CancelHide(channel);
            Recycle(channel);
        }

        private void Recycle(ToastChannel channel)
        {
            channel.Model.Hide();
            if (channel.View == null)
            {
                return;
            }

            _pool.Release(channel.View);
            channel.View = null;
        }

        private static void CancelHide(ToastChannel channel)
        {
            if (channel.HideCts == null)
            {
                return;
            }

            channel.HideCts.Cancel();
            channel.HideCts.Dispose();
            channel.HideCts = null;
        }

        private sealed class ToastChannel
        {
            public ToastChannel(Vector2 restPosition)
            {
                RestPosition = restPosition;
                Model = new PickupToastModel();
            }

            public Vector2 RestPosition { get; }
            public PickupToastModel Model { get; }
            public PickupToastView View { get; set; }
            public CancellationTokenSource HideCts { get; set; }
        }
    }
}
