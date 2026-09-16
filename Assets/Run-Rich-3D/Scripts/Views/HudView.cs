using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using RunRich3D.Settings;

namespace RunRich3D.Views
{
    public sealed class HudView : MonoBehaviour
    {
        private readonly Subject<Unit> _actionClicked = new Subject<Unit>();
        private WinPanelView _winPanel;
        private LosePanelView _losePanel;

        private HudSettings _settings;
        private Font _font;
        private Texture2D _buttonTexture;
        private Texture2D _retryTexture;
        private Texture2D _billsTexture;
        private Texture2D _arrowTexture;
        private Texture2D _fingerTexture;
        private Texture2D _settingsTexture;
        private Texture2D _noAdsTexture;
        private Texture2D _shopSkinTexture;
        private Texture2D _pickupsTexture;
        private Texture2D _parquetTexture;
        private GameObject _swipeHint;
        private GameObject _sideButtons;
        private GameObject _worldProgress;

        private RectTransform _runRoot;
        private RectTransform _resultRoot;
        private Text _levelLabel;
        private Text _runScoreLabel;
        private Text _coinsLabel;
        private Image _resultDimmer;
        private bool _built;
        private int _runScoreTarget;
        private float _runScoreDisplayed;
        private int _coinsTarget;
        private float _coinsDisplayed;

        public IObservable<Unit> ActionClicked => _actionClicked;
        public IObservable<int> WinCollected => _winPanel != null ? _winPanel.Collected : Observable.Empty<int>();

        public void Bind(HudSettings settings)
        {
            _settings = settings;
            Bind(
                settings.Font,
                settings.ButtonTexture,
                settings.RetryTexture,
                settings.BillsTexture,
                settings.ArrowTexture,
                settings.FingerTexture,
                settings.SettingsTexture,
                settings.NoAdsTexture,
                settings.ShopSkinTexture,
                settings.PickupsTexture,
                settings.ParquetTexture);
            BindWinPanel(
                settings.WinBannerTexture,
                settings.WinGaugeTexture,
                settings.WinNeedleTexture,
                settings.WinOrangeButtonTexture,
                settings.WinBlueButtonTexture,
                settings.WinPlayTexture);
            BindLosePanel(settings.LoseBannerTexture, settings.LoseButtonTexture);
        }

        public void Bind(
            Font font,
            Texture2D buttonTexture,
            Texture2D retryTexture,
            Texture2D billsTexture,
            Texture2D arrowTexture,
            Texture2D fingerTexture,
            Texture2D settingsTexture,
            Texture2D noAdsTexture,
            Texture2D shopSkinTexture,
            Texture2D pickupsTexture,
            Texture2D parquetTexture)
        {
            _font = font;
            _buttonTexture = buttonTexture;
            _retryTexture = retryTexture;
            _billsTexture = billsTexture;
            _arrowTexture = arrowTexture;
            _fingerTexture = fingerTexture;
            _settingsTexture = settingsTexture;
            _noAdsTexture = noAdsTexture;
            _shopSkinTexture = shopSkinTexture;
            _pickupsTexture = pickupsTexture;
            _parquetTexture = parquetTexture;
            EnsureBuilt();
        }

        public void BindWinPanel(
            Texture2D bannerTexture,
            Texture2D gaugeTexture,
            Texture2D needleTexture,
            Texture2D orangeTexture,
            Texture2D blueTexture,
            Texture2D playTexture)
        {
            EnsureBuilt();
            if (_winPanel != null)
            {
                _winPanel.Bind(
                    _settings,
                    _font,
                    bannerTexture,
                    gaugeTexture,
                    needleTexture,
                    orangeTexture,
                    blueTexture,
                    playTexture,
                    _billsTexture);
            }
        }

        public void BindLosePanel(Texture2D bannerTexture, Texture2D buttonTexture)
        {
            EnsureBuilt();
            if (_losePanel == null)
            {
                return;
            }

            _losePanel.Bind(_settings, _font, bannerTexture, buttonTexture);
            _losePanel.RetryClicked.Subscribe(_ => _actionClicked.OnNext(Unit.Default));
        }

        public void ShowRun(string levelText, int runScore)
        {
            EnsureBuilt();
            _runRoot.gameObject.SetActive(true);
            _resultRoot.gameObject.SetActive(false);
            if (_winPanel != null)
            {
                _winPanel.Hide();
            }

            if (_losePanel != null)
            {
                _losePanel.Hide();
            }

            _levelLabel.text = levelText;
            SnapRunScore(runScore);
        }

        public void ShowLose()
        {
            EnsureBuilt();
            _runRoot.gameObject.SetActive(false);
            _resultRoot.gameObject.SetActive(true);
            if (_winPanel != null)
            {
                _winPanel.Hide();
            }

            SetResultDimmer(_settings != null ? _settings.ResultDimmerAlpha : 0.18f);
            if (_losePanel != null)
            {
                _losePanel.Show();
            }
        }

        public void ShowWin(int levelNumber, int collectedReward)
        {
            EnsureBuilt();
            _runRoot.gameObject.SetActive(false);
            _resultRoot.gameObject.SetActive(true);
            if (_losePanel != null)
            {
                _losePanel.Hide();
            }

            SetResultDimmer(_settings != null ? _settings.ResultDimmerAlpha : 0.18f);
            if (_winPanel != null)
            {
                _winPanel.Show(levelNumber, collectedReward);
            }
        }

        public void SetRunScore(int score)
        {
            _runScoreTarget = score < 0 ? 0 : score;
        }

        public void SetCoins(int coins)
        {
            _coinsTarget = coins < 0 ? 0 : coins;
        }

        public void SetSwipeHintVisible(bool visible)
        {
            if (_swipeHint != null)
            {
                _swipeHint.SetActive(visible);
            }
        }

        public void SetSideButtonsVisible(bool visible)
        {
            if (_sideButtons != null)
            {
                _sideButtons.SetActive(visible);
            }
        }

        public void SetWorldProgressVisible(bool visible)
        {
            if (_worldProgress != null)
            {
                _worldProgress.SetActive(visible);
            }
        }

        public void SetCenterStatsVisible(bool visible)
        {
            if (_levelLabel != null)
            {
                _levelLabel.gameObject.SetActive(visible);
            }

            if (_runScoreLabel != null)
            {
                _runScoreLabel.gameObject.SetActive(visible);
            }
        }

        public void SetLevel(string levelText)
        {
            if (_levelLabel == null)
            {
                return;
            }

            _levelLabel.text = levelText;
        }

        private void Update()
        {
            TickCounter(ref _runScoreDisplayed, _runScoreTarget, _runScoreLabel);
            TickCounter(ref _coinsDisplayed, _coinsTarget, _coinsLabel);
        }

        private void SnapRunScore(int score)
        {
            _runScoreTarget = score < 0 ? 0 : score;
            _runScoreDisplayed = _runScoreTarget;
            ApplyCounterText(_runScoreLabel, _runScoreTarget);
        }

        private void TickCounter(ref float displayed, int target, Text label)
        {
            if (label == null)
            {
                return;
            }

            float gap = target - displayed;
            float snapGap = _settings != null ? _settings.CounterSnapGap : 0.05f;
            if (Mathf.Abs(gap) < snapGap)
            {
                displayed = target;
                ApplyCounterText(label, target);
                return;
            }

            float catchUp = _settings != null ? _settings.CounterCatchUpSeconds : 0.28f;
            float minSpeed = _settings != null ? _settings.CounterMinSpeed : 36f;
            float speed = Mathf.Max(minSpeed, Mathf.Abs(gap) / catchUp);
            displayed = Mathf.MoveTowards(displayed, target, speed * Time.deltaTime);
            ApplyCounterText(label, Mathf.RoundToInt(displayed));
        }

        private static void ApplyCounterText(Text label, int value)
        {
            if (label != null)
            {
                label.text = value.ToString();
            }
        }

        private void OnDestroy()
        {
            _actionClicked.OnCompleted();
            _actionClicked.Dispose();
        }

        private void EnsureBuilt()
        {
            if (_built)
            {
                return;
            }

            var root = (RectTransform)transform;
            Stretch(root);

            _runRoot = CreatePanel("RunHud", root);
            Stretch(_runRoot);

            _levelLabel = CreateText(
                "LevelLabel",
                _runRoot,
                42,
                FontStyle.Bold,
                TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -268f),
                new Vector2(720f, 80f));

            _runScoreLabel = CreateText(
                "RunScore",
                _runRoot,
                96,
                FontStyle.Bold,
                TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -340f),
                new Vector2(720f, 140f));

            BuildWorldProgress(_runRoot);
            BuildCoinsChip(_runRoot);
            BuildSettingsButton(_runRoot);
            BuildSideButtons(_runRoot);
            BuildSwipeHint(_runRoot);
            SetCenterStatsVisible(false);

            _resultRoot = CreatePanel("ResultPanel", root);
            Stretch(_resultRoot);
            _resultDimmer = _resultRoot.gameObject.AddComponent<Image>();
            _resultDimmer.color = new Color(0f, 0f, 0f, 0.58f);
            _resultDimmer.raycastTarget = true;

            var loseRoot = CreateChild("LosePanel", _resultRoot);
            Stretch(loseRoot);
            _losePanel = loseRoot.gameObject.AddComponent<LosePanelView>();

            var winRoot = CreateChild("WinPanel", _resultRoot);
            Stretch(winRoot);
            _winPanel = winRoot.gameObject.AddComponent<WinPanelView>();
            _resultRoot.gameObject.SetActive(false);
            _built = true;
        }

        private void SetResultDimmer(float alpha)
        {
            if (_resultDimmer != null)
            {
                _resultDimmer.color = new Color(0f, 0f, 0f, alpha);
            }
        }

        private void BuildWorldProgress(RectTransform parent)
        {
            float width = _settings != null ? _settings.WorldProgressSize.x : 860f;
            float height = _settings != null ? _settings.WorldProgressSize.y : 156f;
            int nodeCount = _settings != null ? _settings.WorldProgressNodeCount : 5;
            float iconSize = _settings != null ? _settings.WorldProgressIconSize : 84f;
            float nodeSize = _settings != null ? _settings.WorldProgressNodeSize : 44f;
            float trackHeight = _settings != null ? _settings.WorldProgressTrackHeight : 18f;
            float top = _settings != null ? _settings.WorldProgressTop : -92f;

            var bar = CreateChild("WorldProgress", parent);
            _worldProgress = bar.gameObject;
            bar.anchorMin = new Vector2(0.5f, 1f);
            bar.anchorMax = new Vector2(0.5f, 1f);
            bar.pivot = new Vector2(0.5f, 1f);
            bar.anchoredPosition = new Vector2(0f, top);
            bar.sizeDelta = new Vector2(width, height);

            var background = bar.gameObject.AddComponent<Image>();
            background.sprite = CreateCapsuleSprite(Mathf.RoundToInt(width), Mathf.RoundToInt(height));
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = new Color(0.05f, 0.22f, 0.30f, 0.92f);
            background.raycastTarget = false;

            float trackY = 12f;
            var track = CreateChild("Track", bar);
            track.anchorMin = new Vector2(0.5f, 0.5f);
            track.anchorMax = new Vector2(0.5f, 0.5f);
            track.pivot = new Vector2(0.5f, 0.5f);
            track.anchoredPosition = new Vector2(0f, trackY);
            track.sizeDelta = new Vector2(width - 52f, trackHeight);
            var trackImage = track.gameObject.AddComponent<Image>();
            trackImage.sprite = CreateCapsuleSprite(Mathf.RoundToInt(width - 52f), Mathf.RoundToInt(trackHeight));
            trackImage.color = Color.white;
            trackImage.raycastTarget = false;

            float innerLeft = 22f + iconSize;
            float innerWidth = width - innerLeft * 2f;
            for (int i = 1; i < nodeCount; i++)
            {
                float x = -innerWidth * 0.5f + innerWidth * (i / (float)nodeCount);
                var tick = CreateChild("Tick" + i, bar);
                tick.anchorMin = new Vector2(0.5f, 0.5f);
                tick.anchorMax = new Vector2(0.5f, 0.5f);
                tick.pivot = new Vector2(0.5f, 0.5f);
                tick.anchoredPosition = new Vector2(x, trackY);
                tick.sizeDelta = new Vector2(4f, 28f);
                var tickImage = tick.gameObject.AddComponent<Image>();
                tickImage.color = new Color(0.05f, 0.22f, 0.30f, 0.7f);
                tickImage.raycastTarget = false;
            }

            CreateDestinationIcon("StartIcon", bar, new Vector2(22f, trackY), new Vector2(0f, 0.5f));
            CreateDestinationIcon("FinishIcon", bar, new Vector2(-22f, trackY), new Vector2(1f, 0.5f));

            var circleSprite = CreateCapsuleSprite(64, 64);
            for (int i = 0; i < nodeCount; i++)
            {
                float x = -innerWidth * 0.5f + innerWidth * ((i + 0.5f) / nodeCount);
                var node = CreateChild("Node" + (i + 1), bar);
                node.anchorMin = new Vector2(0.5f, 0.5f);
                node.anchorMax = new Vector2(0.5f, 0.5f);
                node.pivot = new Vector2(0.5f, 0.5f);
                node.anchoredPosition = new Vector2(x, trackY - 42f);
                node.sizeDelta = new Vector2(nodeSize, nodeSize);
                var nodeImage = node.gameObject.AddComponent<Image>();
                nodeImage.sprite = circleSprite;
                nodeImage.color = Color.white;
                nodeImage.raycastTarget = false;

                var label = CreateText(
                    "Number",
                    node,
                    24,
                    FontStyle.Bold,
                    TextAnchor.MiddleCenter,
                    new Vector2(0.5f, 0.5f),
                    new Vector2(0.5f, 0.5f),
                    Vector2.zero,
                    new Vector2(nodeSize, nodeSize));
                label.text = (i + 1).ToString();
                label.color = new Color(0.12f, 0.2f, 0.28f);
                var outline = label.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }
        }

        private void CreateDestinationIcon(string name, RectTransform parent, Vector2 position, Vector2 pivot)
        {
            float size = _settings != null ? _settings.WorldProgressIconSize : 84f;
            var icon = CreateChild(name, parent);
            icon.anchorMin = pivot;
            icon.anchorMax = pivot;
            icon.pivot = pivot;
            icon.anchoredPosition = position;
            icon.sizeDelta = new Vector2(size, size);

            var frame = icon.gameObject.AddComponent<Image>();
            frame.sprite = CreateCapsuleSprite(Mathf.RoundToInt(size), Mathf.RoundToInt(size));
            frame.color = Color.white;
            frame.raycastTarget = false;

            var maskRect = CreateChild("Mask", icon);
            maskRect.anchorMin = new Vector2(0.5f, 0.5f);
            maskRect.anchorMax = new Vector2(0.5f, 0.5f);
            maskRect.pivot = new Vector2(0.5f, 0.5f);
            maskRect.anchoredPosition = Vector2.zero;
            maskRect.sizeDelta = new Vector2(size - 8f, size - 8f);
            var maskImage = maskRect.gameObject.AddComponent<Image>();
            maskImage.sprite = CreateCapsuleSprite(Mathf.RoundToInt(size - 8f), Mathf.RoundToInt(size - 8f));
            maskImage.color = Color.white;
            maskImage.raycastTarget = false;
            var mask = maskRect.gameObject.AddComponent<Mask>();
            mask.showMaskGraphic = false;

            if (_parquetTexture == null)
            {
                return;
            }

            var photo = CreateChild("Photo", maskRect);
            Stretch(photo);
            var photoImage = photo.gameObject.AddComponent<RawImage>();
            photoImage.texture = _parquetTexture;
            photoImage.uvRect = new Rect(0.16f, 0.12f, 0.68f, 0.68f);
            photoImage.raycastTarget = false;
        }

        private void BuildCoinsChip(RectTransform parent)
        {
            var chip = CreateChild("CoinsChip", parent);
            chip.anchorMin = new Vector2(1f, 1f);
            chip.anchorMax = new Vector2(1f, 1f);
            chip.pivot = new Vector2(1f, 1f);
            chip.anchoredPosition = new Vector2(-24f, -22f);
            chip.sizeDelta = new Vector2(276f, 52f);

            var background = chip.gameObject.AddComponent<Image>();
            background.sprite = CreateCapsuleSprite(280, 52);
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = new Color(0.04f, 0.04f, 0.06f, 0.58f);
            background.raycastTarget = false;

            if (_billsTexture != null)
            {
                var icon = CreateChild("Bills", chip);
                var image = icon.gameObject.AddComponent<RawImage>();
                image.texture = _billsTexture;
                image.raycastTarget = false;
                icon.anchorMin = new Vector2(1f, 0.5f);
                icon.anchorMax = new Vector2(1f, 0.5f);
                icon.pivot = new Vector2(1f, 0.5f);
                icon.anchoredPosition = new Vector2(-8f, 0f);
                icon.sizeDelta = new Vector2(70f, 42f);
            }

            _coinsLabel = CreateText(
                "Coins",
                chip,
                32,
                FontStyle.Bold,
                TextAnchor.MiddleRight,
                new Vector2(0f, 0f),
                new Vector2(1f, 1f),
                Vector2.zero,
                Vector2.zero);
            var coinsRect = (RectTransform)_coinsLabel.transform;
            coinsRect.offsetMin = new Vector2(22f, 4f);
            coinsRect.offsetMax = new Vector2(-80f, -4f);
            _coinsLabel.text = "0";
        }

        private void BuildSettingsButton(RectTransform parent)
        {
            float size = _settings != null ? _settings.SettingsButtonSize : 80f;
            var button = CreateChild("Settings", parent);
            button.anchorMin = new Vector2(0f, 1f);
            button.anchorMax = new Vector2(0f, 1f);
            button.pivot = new Vector2(0f, 1f);
            button.anchoredPosition = new Vector2(24f, -22f);
            button.sizeDelta = new Vector2(size, size);

            var background = button.gameObject.AddComponent<Image>();
            background.sprite = CreateCapsuleSprite(Mathf.RoundToInt(size), Mathf.RoundToInt(size));
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = new Color(0.04f, 0.04f, 0.06f, 0.58f);
            background.raycastTarget = false;

            if (_settingsTexture == null)
            {
                return;
            }

            var icon = CreateChild("Icon", button);
            var image = icon.gameObject.AddComponent<RawImage>();
            image.texture = _settingsTexture;
            image.raycastTarget = false;
            icon.anchorMin = new Vector2(0.5f, 0.5f);
            icon.anchorMax = new Vector2(0.5f, 0.5f);
            icon.pivot = new Vector2(0.5f, 0.5f);
            icon.anchoredPosition = Vector2.zero;
            icon.sizeDelta = new Vector2(44f, 44f);
        }

        private void BuildSideButtons(RectTransform parent)
        {
            var column = CreateChild("SideButtons", parent);
            Stretch(column);
            _sideButtons = column.gameObject;

            float width = _settings != null ? _settings.SideButtonSize.x : 148f;
            float height = _settings != null ? _settings.SideButtonSize.y : 86f;
            float gap = _settings != null ? _settings.SideButtonGap : 18f;
            float step = height + gap;
            CreateSideIcon("NoAds", column, _noAdsTexture, width, height, step);
            CreateSideIcon("ShopSkin", column, _shopSkinTexture, width, height, 0f);
            CreateSideIcon("Pickups", column, _pickupsTexture, width, height, -step);
        }

        private static void CreateSideIcon(
            string name,
            RectTransform parent,
            Texture2D texture,
            float width,
            float height,
            float y)
        {
            if (texture == null)
            {
                return;
            }

            var icon = CreateChild(name, parent);
            icon.anchorMin = new Vector2(1f, 0.5f);
            icon.anchorMax = new Vector2(1f, 0.5f);
            icon.pivot = new Vector2(1f, 0.5f);
            icon.anchoredPosition = new Vector2(-18f, y);
            icon.sizeDelta = new Vector2(width, height);
            var image = icon.gameObject.AddComponent<RawImage>();
            image.texture = texture;
            image.raycastTarget = false;
        }

        private void BuildSwipeHint(RectTransform parent)
        {
            var hint = CreateChild("SwipeHint", parent);
            hint.anchorMin = new Vector2(0.5f, 0f);
            hint.anchorMax = new Vector2(0.5f, 0f);
            hint.pivot = new Vector2(0.5f, 0f);
            hint.anchoredPosition = new Vector2(0f, _settings != null ? _settings.SwipeHintBottom : 88f);
            _swipeHint = hint.gameObject;

            var capsule = CreateChild("Capsule", hint);
            capsule.anchorMin = new Vector2(0.5f, 1f);
            capsule.anchorMax = new Vector2(0.5f, 1f);
            capsule.pivot = new Vector2(0.5f, 1f);
            capsule.anchoredPosition = Vector2.zero;

            var label = CreateText(
                "HintLabel",
                capsule,
                28,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(40f, 56f));
            label.text = _settings != null ? _settings.SwipeHintText : "Проведите по экрану, чтобы повернуть";
            float capsuleWidth = Mathf.Ceil(label.preferredWidth + 56f);
            const float capsuleHeight = 64f;
            capsule.sizeDelta = new Vector2(capsuleWidth, capsuleHeight);
            ((RectTransform)label.transform).sizeDelta = new Vector2(capsuleWidth - 24f, 56f);
            hint.sizeDelta = new Vector2(capsuleWidth, 130f);

            var background = capsule.gameObject.AddComponent<Image>();
            background.sprite = CreateCapsuleSprite(Mathf.RoundToInt(capsuleWidth), Mathf.RoundToInt(capsuleHeight));
            background.type = Image.Type.Simple;
            background.preserveAspect = false;
            background.color = new Color(0.04f, 0.04f, 0.06f, 0.58f);
            background.raycastTarget = false;
            background.transform.SetAsFirstSibling();

            if (_arrowTexture == null)
            {
                return;
            }

            var arrow = CreateChild("Arrows", hint);
            var image = arrow.gameObject.AddComponent<RawImage>();
            image.texture = _arrowTexture;
            image.raycastTarget = false;
            arrow.anchorMin = new Vector2(0.5f, 1f);
            arrow.anchorMax = new Vector2(0.5f, 1f);
            arrow.pivot = new Vector2(0.5f, 1f);
            arrow.anchoredPosition = new Vector2(0f, -72f);
            arrow.sizeDelta = new Vector2(capsuleWidth, 52f);

            if (_fingerTexture == null)
            {
                return;
            }

            var finger = CreateChild("Finger", arrow);
            var fingerImage = finger.gameObject.AddComponent<RawImage>();
            fingerImage.texture = _fingerTexture;
            fingerImage.raycastTarget = false;
            finger.anchorMin = new Vector2(0.5f, 0.5f);
            finger.anchorMax = new Vector2(0.5f, 0.5f);
            finger.pivot = new Vector2(0.35f, 0.85f);
            finger.anchoredPosition = Vector2.zero;
            finger.sizeDelta = new Vector2(92f, 104f);
            finger.SetAsLastSibling();
            var motion = finger.gameObject.AddComponent<SwipeHintFingerView>();
            float travel = Mathf.Max(8f, capsuleWidth * 0.5f - 20f);
            motion.Bind(
                travel,
                _settings != null ? _settings.FingerOutSeconds : 0.55f,
                _settings != null ? _settings.FingerInSeconds : 0.5f,
                _settings != null ? _settings.FingerPauseSeconds : 0.45f);
        }

        private static Sprite CreateCapsuleSprite(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            var pixels = new Color32[width * height];
            float radius = (height - 1) * 0.5f;
            float leftCx = radius;
            float rightCx = width - 1 - radius;
            float cy = radius;
            float feather = 1.6f;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float dx;
                    if (x < leftCx)
                    {
                        dx = x - leftCx;
                    }
                    else if (x > rightCx)
                    {
                        dx = x - rightCx;
                    }
                    else
                    {
                        dx = 0f;
                    }

                    float dy = y - cy;
                    float distance = Mathf.Sqrt(dx * dx + dy * dy);
                    float alpha = Mathf.Clamp01((radius + feather - distance) / feather);
                    pixels[y * width + x] = new Color32(255, 255, 255, (byte)(alpha * 255f));
                }
            }

            texture.SetPixels32(pixels);
            texture.Apply(false, false);
            texture.hideFlags = HideFlags.HideAndDontSave;
            return Sprite.Create(texture, new Rect(0f, 0f, width, height), new Vector2(0.5f, 0.5f), 100f);
        }

        private Text CreateText(
            string name,
            RectTransform parent,
            int size,
            FontStyle style,
            TextAnchor alignment,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 position,
            Vector2 sizeDelta)
        {
            var rect = CreateChild(name, parent);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
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

        private static RectTransform CreatePanel(string name, RectTransform parent)
        {
            return CreateChild(name, parent);
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
