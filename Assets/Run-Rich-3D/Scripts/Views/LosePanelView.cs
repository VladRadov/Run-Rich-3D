using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich3D.Views
{
    public sealed class LosePanelView : MonoBehaviour
    {
        private readonly Subject<Unit> _retryClicked = new Subject<Unit>();
        private Font _font;
        private Texture2D _bannerTexture;
        private Texture2D _buttonTexture;
        private RectTransform _banner;
        private bool _built;
        private bool _busy;
        private CancellationTokenSource _introCts;

        internal IObservable<Unit> RetryClicked => _retryClicked;

        internal void Bind(Font font, Texture2D bannerTexture, Texture2D buttonTexture)
        {
            _font = font;
            _bannerTexture = bannerTexture;
            _buttonTexture = buttonTexture;
            EnsureBuilt();
        }

        internal void Show()
        {
            EnsureBuilt();
            gameObject.SetActive(true);
            _busy = false;
            PlayIntro();
        }

        internal void Hide()
        {
            CancelIntro();
            _busy = false;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            CancelIntro();
        }

        private void OnDestroy()
        {
            CancelIntro();
            _retryClicked.OnCompleted();
            _retryClicked.Dispose();
        }

        private void EnsureBuilt()
        {
            if (_built)
            {
                return;
            }

            var root = (RectTransform)transform;
            Stretch(root);
            BuildBanner(root);
            BuildRetryButton(root);
            gameObject.SetActive(false);
            _built = true;
        }

        private void BuildBanner(RectTransform parent)
        {
            _banner = CreateChild("Banner", parent);
            _banner.anchorMin = new Vector2(0f, 1f);
            _banner.anchorMax = new Vector2(1f, 1f);
            _banner.pivot = new Vector2(0.5f, 1f);
            _banner.anchoredPosition = Vector2.zero;
            _banner.sizeDelta = new Vector2(0f, 340f);
            var image = _banner.gameObject.AddComponent<RawImage>();
            image.texture = _bannerTexture;
            image.color = Color.white;
            image.raycastTarget = false;

            var title = CreateText(
                "FailTitle",
                _banner,
                72,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0f, 8f),
                new Vector2(960f, 110f));
            title.text = "НЕУДАЧА";
        }

        private void BuildRetryButton(RectTransform parent)
        {
            var buttonRect = CreateChild("RetryButton", parent);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0f, -388f);
            buttonRect.sizeDelta = new Vector2(580f, 176f);
            var image = buttonRect.gameObject.AddComponent<RawImage>();
            image.texture = _buttonTexture;
            image.raycastTarget = true;
            var button = buttonRect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            button.onClick.AddListener(OnRetryClicked);

            CreateText(
                "RetryLabel",
                buttonRect,
                46,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                new Vector2(480f, 90f)).text = "ПОВТОРИТЬ";
        }

        private void OnRetryClicked()
        {
            if (_busy)
            {
                return;
            }

            _busy = true;
            _retryClicked.OnNext(Unit.Default);
        }

        private void PlayIntro()
        {
            CancelIntro();
            _introCts = new CancellationTokenSource();
            AnimateBannerAsync(_introCts.Token).Forget();
        }

        private async UniTaskVoid AnimateBannerAsync(CancellationToken token)
        {
            if (_banner == null)
            {
                return;
            }

            Vector2 hidden = new Vector2(0f, 360f);
            Vector2 shown = Vector2.zero;
            _banner.anchoredPosition = hidden;
            float t = 0f;
            while (t < 1f)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                t += Time.unscaledDeltaTime / 0.42f;
                float u = Mathf.Clamp01(t);
                float eased = 1f - (1f - u) * (1f - u);
                _banner.anchoredPosition = Vector2.LerpUnclamped(hidden, shown, eased);
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            _banner.anchoredPosition = shown;
        }

        private void CancelIntro()
        {
            if (_introCts == null)
            {
                return;
            }

            _introCts.Cancel();
            _introCts.Dispose();
            _introCts = null;
        }

        private Text CreateText(
            string name,
            RectTransform parent,
            int size,
            FontStyle style,
            TextAnchor alignment,
            Vector2 position,
            Vector2 sizeDelta)
        {
            var rect = CreateChild(name, parent);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = sizeDelta;
            var text = rect.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.55f);
            outline.effectDistance = new Vector2(2f, -2f);
            return text;
        }

        private static RectTransform CreateChild(string name, RectTransform parent)
        {
            var child = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)child.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
