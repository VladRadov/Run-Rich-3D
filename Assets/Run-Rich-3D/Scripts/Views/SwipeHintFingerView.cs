using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class SwipeHintFingerView : MonoBehaviour
    {
        private RectTransform _rect;
        private float _travel;
        private float _outSeconds = 0.55f;
        private float _inSeconds = 0.5f;
        private float _pauseSeconds = 0.45f;
        private CancellationTokenSource _loopCts;

        public void Bind(float travel, float outSeconds, float inSeconds, float pauseSeconds)
        {
            _rect = (RectTransform)transform;
            _travel = travel > 1f ? travel : 1f;
            _outSeconds = outSeconds > 0.01f ? outSeconds : 0.55f;
            _inSeconds = inSeconds > 0.01f ? inSeconds : 0.5f;
            _pauseSeconds = pauseSeconds > 0.01f ? pauseSeconds : 0.45f;
            SetX(0f);
            RestartLoop();
        }

        private void OnEnable()
        {
            if (_rect != null)
            {
                RestartLoop();
            }
        }

        private void OnDisable()
        {
            CancelLoop();
        }

        private void RestartLoop()
        {
            CancelLoop();
            if (!isActiveAndEnabled || _rect == null)
            {
                return;
            }

            SetX(0f);
            _loopCts = new CancellationTokenSource();
            RunLoopAsync(_loopCts.Token).Forget();
        }

        private async UniTaskVoid RunLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    await MoveXAsync(0f, -_travel, _outSeconds, token);
                    await MoveXAsync(-_travel, 0f, _inSeconds, token);
                    await UniTask.Delay(TimeSpan.FromSeconds(_pauseSeconds), cancellationToken: token);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async UniTask MoveXAsync(float from, float to, float duration, CancellationToken token)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                float eased = t * t * (3f - 2f * t);
                SetX(Mathf.Lerp(from, to, eased));
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            SetX(to);
        }

        private void SetX(float x)
        {
            if (_rect == null)
            {
                return;
            }

            Vector2 position = _rect.anchoredPosition;
            position.x = x;
            _rect.anchoredPosition = position;
        }

        private void CancelLoop()
        {
            if (_loopCts == null)
            {
                return;
            }

            _loopCts.Cancel();
            _loopCts.Dispose();
            _loopCts = null;
        }
    }
}
