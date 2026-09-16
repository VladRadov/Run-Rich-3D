using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using RunRich3D.Settings;

namespace RunRich3D.Views
{
    public sealed class WinPanelView : MonoBehaviour
    {
        private readonly Subject<int> _collected = new Subject<int>();
        private HudSettings _settings;
        private Font _font;
        private Texture2D _bannerTexture;
        private Texture2D _gaugeTexture;
        private Texture2D _needleTexture;
        private Texture2D _orangeTexture;
        private Texture2D _blueTexture;
        private Texture2D _playTexture;
        private Texture2D _billsTexture;
        private RectTransform _banner;
        private RectTransform _needle;
        private RectTransform _adOverlay;
        private Text _levelLabel;
        private Text _doneLabel;
        private Text _adTitle;
        private Text _adAmount;
        private Text _claimAmount;
        private Button _adButton;
        private Button _claimButton;
        private int _baseReward;
        private int _adMultiplier = 2;
        private float _needleSweep;
        private bool _built;
        private bool _needleActive;
        private bool _busy;
        private CancellationTokenSource _introCts;

        public IObservable<int> Collected => _collected;

        public void Bind(
            HudSettings settings,
            Font font,
            Texture2D bannerTexture,
            Texture2D gaugeTexture,
            Texture2D needleTexture,
            Texture2D orangeTexture,
            Texture2D blueTexture,
            Texture2D playTexture,
            Texture2D billsTexture)
        {
            _settings = settings;
            _font = font;
            _bannerTexture = bannerTexture;
            _gaugeTexture = gaugeTexture;
            _needleTexture = needleTexture;
            _orangeTexture = orangeTexture;
            _blueTexture = blueTexture;
            _playTexture = playTexture;
            _billsTexture = billsTexture;
            EnsureBuilt();
        }

        public void Show(int levelNumber, int baseReward)
        {
            EnsureBuilt();
            gameObject.SetActive(true);
            _baseReward = baseReward < 0 ? 0 : baseReward;
            _busy = false;
            _needleActive = true;
            _adMultiplier = FirstAdMultiplier();
            _levelLabel.text = (_settings != null ? _settings.FormatLevel(levelNumber) : "Уровень " + levelNumber);
            _doneLabel.text = _settings != null ? _settings.WinDoneText : "ЗАВЕРШЕНО";
            _claimAmount.text = _baseReward.ToString();
            RefreshAdLabels();
            if (_adButton != null)
            {
                _adButton.interactable = true;
            }

            if (_claimButton != null)
            {
                _claimButton.interactable = true;
            }

            if (_adOverlay != null)
            {
                _adOverlay.gameObject.SetActive(false);
            }

            PlayIntro();
        }

        public void Hide()
        {
            CancelIntro();
            _needleActive = false;
            _busy = false;
            if (_adOverlay != null)
            {
                _adOverlay.gameObject.SetActive(false);
            }

            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!_needleActive || _needle == null)
            {
                return;
            }

            _needleSweep += Time.unscaledDeltaTime * (_settings != null ? _settings.NeedleSweepSpeed : 1.15f);
            float wave = Mathf.PingPong(_needleSweep, 1f);
            float eased = wave * wave * (3f - 2f * wave);
            float minAngle = _settings != null ? _settings.NeedleMinAngle : -72f;
            float maxAngle = _settings != null ? _settings.NeedleMaxAngle : 72f;
            float angle = Mathf.Lerp(minAngle, maxAngle, eased);
            _needle.localEulerAngles = new Vector3(0f, 0f, -angle);
            int next = MultiplierFromAngle(angle);
            if (next != _adMultiplier)
            {
                _adMultiplier = next;
                RefreshAdLabels();
            }
        }

        private void OnDisable()
        {
            CancelIntro();
            _needleActive = false;
        }

        private void OnDestroy()
        {
            CancelIntro();
            _collected.OnCompleted();
            _collected.Dispose();
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
            BuildTitle(root);
            BuildGauge(root);
            BuildAdButton(root);
            BuildClaimButton(root);
            BuildAdOverlay(root);
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
            _banner.sizeDelta = new Vector2(0f, _settings != null ? _settings.BannerHeight : 340f);
            var image = _banner.gameObject.AddComponent<RawImage>();
            image.texture = _bannerTexture;
            image.color = Color.white;
            image.raycastTarget = false;

            _levelLabel = CreateText(
                "Level",
                _banner,
                68,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0f, 42f),
                new Vector2(960f, 84f));
            _doneLabel = CreateText(
                "Done",
                _banner,
                64,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0f, -32f),
                new Vector2(960f, 78f));
        }

        private void BuildTitle(RectTransform parent)
        {
            var title = CreateText(
                "WinTitle",
                parent,
                62,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                new Vector2(960f, 120f));
            var rect = (RectTransform)title.transform;
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -360f);
            title.text = "ВЫ ПОБЕДИЛИ!";
            title.color = new Color(1f, 0.86f, 0.22f);
        }

        private void BuildGauge(RectTransform parent)
        {
            var gauge = CreateChild("Gauge", parent);
            gauge.anchorMin = new Vector2(0.5f, 0.5f);
            gauge.anchorMax = new Vector2(0.5f, 0.5f);
            gauge.pivot = new Vector2(0.5f, 0.5f);
            gauge.anchoredPosition = new Vector2(0f, -322f);
            gauge.sizeDelta = new Vector2(500f, 500f);
            var image = gauge.gameObject.AddComponent<RawImage>();
            image.texture = _gaugeTexture;
            image.raycastTarget = false;

            PlaceLabel("X2", gauge, new Vector2(-168f, 18f), "x2");
            PlaceLabel("X3", gauge, new Vector2(-58f, 128f), "x3");
            PlaceLabel("X4", gauge, new Vector2(58f, 128f), "x4");
            PlaceLabel("X5", gauge, new Vector2(168f, 18f), "x5");

            _needle = CreateChild("Needle", gauge);
            _needle.anchorMin = new Vector2(0.5f, 0.5f);
            _needle.anchorMax = new Vector2(0.5f, 0.5f);
            _needle.pivot = new Vector2(0.5f, 0.22f);
            _needle.anchoredPosition = new Vector2(0f, -18f);
            _needle.sizeDelta = new Vector2(150f, 170f);
            var needleImage = _needle.gameObject.AddComponent<RawImage>();
            needleImage.texture = _needleTexture;
            needleImage.raycastTarget = false;
        }

        private void BuildAdButton(RectTransform parent)
        {
            var buttonRect = CreateChild("AdButton", parent);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0f, -388f);
            buttonRect.sizeDelta = new Vector2(500f, 132f);
            var image = buttonRect.gameObject.AddComponent<RawImage>();
            image.texture = _orangeTexture;
            image.raycastTarget = true;
            _adButton = CreateButton(buttonRect.gameObject, image, OnAdClicked);

            _adTitle = CreateText(
                "AdTitle",
                buttonRect,
                26,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Vector2(-24f, 32f),
                new Vector2(320f, 36f));
            _adAmount = CreateText(
                "AdAmount",
                buttonRect,
                44,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Vector2(-24f, -16f),
                new Vector2(320f, 58f));
            AddBillsIcon(buttonRect, new Vector2(72f, -16f), new Vector2(74f, 44f));
            AddPlayIcon(buttonRect);
        }

        private void BuildClaimButton(RectTransform parent)
        {
            var buttonRect = CreateChild("ClaimButton", parent);
            buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
            buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
            buttonRect.pivot = new Vector2(0.5f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(0f, -578f);
            buttonRect.sizeDelta = new Vector2(580f, 176f);
            var image = buttonRect.gameObject.AddComponent<RawImage>();
            image.texture = _blueTexture;
            image.raycastTarget = true;
            _claimButton = CreateButton(buttonRect.gameObject, image, OnClaimClicked);

            CreateText(
                "ClaimTitle",
                buttonRect,
                34,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0f, 36f),
                new Vector2(460f, 44f)).text = _settings != null ? _settings.ClaimText : "ПОЛУЧИТЬ";
            _claimAmount = CreateText(
                "ClaimAmount",
                buttonRect,
                56,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(-18f, -24f),
                new Vector2(300f, 72f));
            AddBillsIcon(buttonRect, new Vector2(128f, -24f), new Vector2(88f, 54f));
        }

        private void BuildAdOverlay(RectTransform parent)
        {
            _adOverlay = CreateChild("AdOverlay", parent);
            Stretch(_adOverlay);
            var dimmer = _adOverlay.gameObject.AddComponent<Image>();
            dimmer.color = new Color(0.02f, 0.02f, 0.04f, 0.94f);
            dimmer.raycastTarget = true;
            var label = CreateText(
                "AdLabel",
                _adOverlay,
                48,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                Vector2.zero,
                new Vector2(800f, 80f));
            label.text = _settings != null ? _settings.AdOverlayText : "Реклама";
            _adOverlay.gameObject.SetActive(false);
        }

        private void PlaceLabel(string name, RectTransform parent, Vector2 position, string text)
        {
            CreateText(name, parent, 34, FontStyle.Bold, TextAnchor.MiddleCenter, position, new Vector2(90f, 48f)).text = text;
        }

        private void AddBillsIcon(RectTransform parent, Vector2 position, Vector2 size)
        {
            if (_billsTexture == null)
            {
                return;
            }

            var icon = CreateChild("Bills", parent);
            icon.anchorMin = new Vector2(0.5f, 0.5f);
            icon.anchorMax = new Vector2(0.5f, 0.5f);
            icon.pivot = new Vector2(0.5f, 0.5f);
            icon.anchoredPosition = position;
            icon.sizeDelta = size;
            var image = icon.gameObject.AddComponent<RawImage>();
            image.texture = _billsTexture;
            image.raycastTarget = false;
        }

        private void AddPlayIcon(RectTransform parent)
        {
            if (_playTexture == null)
            {
                return;
            }

            var icon = CreateChild("Play", parent);
            icon.anchorMin = new Vector2(1f, 0.5f);
            icon.anchorMax = new Vector2(1f, 0.5f);
            icon.pivot = new Vector2(1f, 0.5f);
            icon.anchoredPosition = new Vector2(-22f, 0f);
            icon.sizeDelta = new Vector2(64f, 64f);
            var image = icon.gameObject.AddComponent<RawImage>();
            image.texture = _playTexture;
            image.raycastTarget = false;
        }

        private void RefreshAdLabels()
        {
            if (_adTitle != null)
            {
                _adTitle.text = (_settings != null ? _settings.ClaimAdPrefix : "ПОЛУЧИТЬ x") + _adMultiplier;
            }

            if (_adAmount != null)
            {
                _adAmount.text = (_baseReward * _adMultiplier).ToString();
            }
        }

        private void OnClaimClicked()
        {
            if (_busy)
            {
                return;
            }

            Complete(_baseReward);
        }

        private void OnAdClicked()
        {
            if (_busy)
            {
                return;
            }

            _needleActive = false;
            SimulateAdAsync().Forget();
        }

        private async UniTaskVoid SimulateAdAsync()
        {
            _busy = true;
            if (_adButton != null)
            {
                _adButton.interactable = false;
            }

            if (_claimButton != null)
            {
                _claimButton.interactable = false;
            }

            if (_adOverlay != null)
            {
                _adOverlay.gameObject.SetActive(true);
            }

            CancellationToken token = this.GetCancellationTokenOnDestroy();
            try
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_settings != null ? _settings.AdOverlaySeconds : 1.4f), cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (_adOverlay != null)
            {
                _adOverlay.gameObject.SetActive(false);
            }

            Complete(_baseReward * _adMultiplier);
        }

        private void Complete(int amount)
        {
            _busy = true;
            _needleActive = false;
            _collected.OnNext(amount < 0 ? 0 : amount);
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

            Vector2 hidden = new Vector2(0f, _settings != null ? _settings.BannerHiddenOffsetY : 360f);
            Vector2 shown = Vector2.zero;
            _banner.anchoredPosition = hidden;
            float t = 0f;
            float dropSeconds = _settings != null ? _settings.BannerDropSeconds : 0.42f;
            while (t < 1f)
            {
                if (token.IsCancellationRequested)
                {
                    return;
                }

                t += Time.unscaledDeltaTime / dropSeconds;
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

        private int FirstAdMultiplier()
        {
            int[] multipliers = Multipliers();
            return multipliers[0];
        }

        private int[] Multipliers()
        {
            return _settings != null ? _settings.AdMultipliers : new[] { 2, 3, 4, 5 };
        }

        private int MultiplierFromAngle(float angle)
        {
            int[] multipliers = Multipliers();
            float minAngle = _settings != null ? _settings.NeedleMinAngle : -72f;
            float maxAngle = _settings != null ? _settings.NeedleMaxAngle : 72f;
            float u = Mathf.InverseLerp(minAngle, maxAngle, angle);
            int index = Mathf.Clamp(Mathf.FloorToInt(u * multipliers.Length), 0, multipliers.Length - 1);
            return multipliers[index];
        }

        private static Button CreateButton(GameObject target, Graphic graphic, UnityEngine.Events.UnityAction onClick)
        {
            var button = target.AddComponent<Button>();
            button.targetGraphic = graphic;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            button.onClick.AddListener(onClick);
            return button;
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
