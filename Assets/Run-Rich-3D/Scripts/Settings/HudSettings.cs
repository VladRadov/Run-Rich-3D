using UnityEngine;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/HUD", fileName = "HudSettings")]
    public sealed class HudSettings : ScriptableObject
    {
        [Header("Assets")]
        [SerializeField] private Font _font;
        [SerializeField] private Texture2D _buttonTexture;
        [SerializeField] private Texture2D _retryTexture;
        [SerializeField] private Texture2D _dollarTexture;
        [SerializeField] private Texture2D _billsTexture;
        [SerializeField] private Texture2D _arrowTexture;
        [SerializeField] private Texture2D _fingerTexture;
        [SerializeField] private Texture2D _settingsTexture;
        [SerializeField] private Texture2D _noAdsTexture;
        [SerializeField] private Texture2D _shopSkinTexture;
        [SerializeField] private Texture2D _pickupsTexture;
        [SerializeField] private Texture2D _parquetTexture;
        [SerializeField] private Texture2D _winBannerTexture;
        [SerializeField] private Texture2D _winGaugeTexture;
        [SerializeField] private Texture2D _winNeedleTexture;
        [SerializeField] private Texture2D _winOrangeButtonTexture;
        [SerializeField] private Texture2D _winBlueButtonTexture;
        [SerializeField] private Texture2D _winPlayTexture;
        [SerializeField] private Texture2D _loseBannerTexture;
        [SerializeField] private Texture2D _loseButtonTexture;

        [Header("Run")]
        [SerializeField] private float _loseAbsX = 3.2f;
        [SerializeField] private int _minDoorMultiplier = 2;
        [SerializeField] private string _levelLabelPrefix = "Уровень ";
        [SerializeField] private float _resultDimmerAlpha = 0.18f;
        [SerializeField] private float _counterSnapGap = 0.05f;
        [SerializeField] private float _counterMinSpeed = 36f;
        [SerializeField] private float _counterCatchUpSeconds = 0.28f;

        [Header("Toast")]
        [SerializeField] private float _toastHoldSeconds = 1.1f;
        [SerializeField] private float _toastFadeSeconds = 0.45f;
        [SerializeField] private float _toastRisePixels = 90f;
        [SerializeField] private Vector2 _toastRestPosition = new Vector2(0f, 80f);
        [SerializeField] private Vector2 _toastLossRestPosition = new Vector2(0f, -20f);

        [Header("Win")]
        [SerializeField] private int[] _adMultipliers = { 2, 3, 4, 5 };
        [SerializeField] private float _needleSweepSpeed = 1.15f;
        [SerializeField] private float _needleMinAngle = -72f;
        [SerializeField] private float _needleMaxAngle = 72f;
        [SerializeField] private string _winDoneText = "ЗАВЕРШЕНО";
        [SerializeField] private string _claimText = "ПОЛУЧИТЬ";
        [SerializeField] private string _claimAdPrefix = "ПОЛУЧИТЬ x";
        [SerializeField] private string _retryText = "ПОВТОРИТЬ";
        [SerializeField] private string _failText = "НЕУДАЧА";
        [SerializeField] private string _adOverlayText = "Реклама";
        [SerializeField] private float _bannerDropSeconds = 0.42f;
        [SerializeField] private float _bannerHiddenOffsetY = 360f;
        [SerializeField] private float _bannerHeight = 340f;
        [SerializeField] private float _adOverlaySeconds = 1.4f;

        [Header("Copy")]
        [SerializeField] private string _swipeHintText = "Проведите по экрану, чтобы повернуть";

        [Header("Hint Finger")]
        [SerializeField] private float _fingerOutSeconds = 0.55f;
        [SerializeField] private float _fingerInSeconds = 0.5f;
        [SerializeField] private float _fingerPauseSeconds = 0.45f;

        [Header("Layout")]
        [SerializeField] private Vector2 _worldProgressSize = new Vector2(860f, 156f);
        [SerializeField] private float _worldProgressTop = -92f;
        [SerializeField] private int _worldProgressNodeCount = 5;
        [SerializeField] private float _worldProgressIconSize = 84f;
        [SerializeField] private float _worldProgressNodeSize = 44f;
        [SerializeField] private float _worldProgressTrackHeight = 18f;
        [SerializeField] private float _settingsButtonSize = 80f;
        [SerializeField] private Vector2 _sideButtonSize = new Vector2(148f, 86f);
        [SerializeField] private float _sideButtonGap = 18f;
        [SerializeField] private float _swipeHintBottom = 88f;

        public Font Font => _font;
        public Texture2D ButtonTexture => _buttonTexture;
        public Texture2D RetryTexture => _retryTexture;
        public Texture2D DollarTexture => _dollarTexture;
        public Texture2D BillsTexture => _billsTexture;
        public Texture2D ArrowTexture => _arrowTexture;
        public Texture2D FingerTexture => _fingerTexture;
        public Texture2D SettingsTexture => _settingsTexture;
        public Texture2D NoAdsTexture => _noAdsTexture;
        public Texture2D ShopSkinTexture => _shopSkinTexture;
        public Texture2D PickupsTexture => _pickupsTexture;
        public Texture2D ParquetTexture => _parquetTexture;
        public Texture2D WinBannerTexture => _winBannerTexture;
        public Texture2D WinGaugeTexture => _winGaugeTexture;
        public Texture2D WinNeedleTexture => _winNeedleTexture;
        public Texture2D WinOrangeButtonTexture => _winOrangeButtonTexture;
        public Texture2D WinBlueButtonTexture => _winBlueButtonTexture;
        public Texture2D WinPlayTexture => _winPlayTexture;
        public Texture2D LoseBannerTexture => _loseBannerTexture;
        public Texture2D LoseButtonTexture => _loseButtonTexture;
        public float LoseAbsX => _loseAbsX;
        public int MinDoorMultiplier => _minDoorMultiplier < 2 ? 2 : _minDoorMultiplier;
        public string LevelLabelPrefix => _levelLabelPrefix;
        public float ResultDimmerAlpha => _resultDimmerAlpha;
        public float CounterSnapGap => _counterSnapGap;
        public float CounterMinSpeed => _counterMinSpeed;
        public float CounterCatchUpSeconds => _counterCatchUpSeconds;
        public float ToastHoldSeconds => _toastHoldSeconds;
        public float ToastFadeSeconds => _toastFadeSeconds;
        public float ToastRisePixels => _toastRisePixels;
        public Vector2 ToastRestPosition => _toastRestPosition;
        public Vector2 ToastLossRestPosition => _toastLossRestPosition;
        public int[] AdMultipliers => _adMultipliers != null && _adMultipliers.Length > 0 ? _adMultipliers : new[] { 2, 3, 4, 5 };
        public float NeedleSweepSpeed => _needleSweepSpeed;
        public float NeedleMinAngle => _needleMinAngle;
        public float NeedleMaxAngle => _needleMaxAngle;
        public string WinDoneText => _winDoneText;
        public string ClaimText => _claimText;
        public string ClaimAdPrefix => _claimAdPrefix;
        public string RetryText => _retryText;
        public string FailText => _failText;
        public string AdOverlayText => _adOverlayText;
        public float BannerDropSeconds => _bannerDropSeconds > 0.01f ? _bannerDropSeconds : 0.42f;
        public float BannerHiddenOffsetY => _bannerHiddenOffsetY;
        public float BannerHeight => _bannerHeight;
        public float AdOverlaySeconds => _adOverlaySeconds > 0.01f ? _adOverlaySeconds : 1.4f;
        public string SwipeHintText => _swipeHintText;
        public float FingerOutSeconds => _fingerOutSeconds;
        public float FingerInSeconds => _fingerInSeconds;
        public float FingerPauseSeconds => _fingerPauseSeconds;
        public Vector2 WorldProgressSize => _worldProgressSize;
        public float WorldProgressTop => _worldProgressTop;
        public int WorldProgressNodeCount => _worldProgressNodeCount < 1 ? 1 : _worldProgressNodeCount;
        public float WorldProgressIconSize => _worldProgressIconSize;
        public float WorldProgressNodeSize => _worldProgressNodeSize;
        public float WorldProgressTrackHeight => _worldProgressTrackHeight;
        public float SettingsButtonSize => _settingsButtonSize;
        public Vector2 SideButtonSize => _sideButtonSize;
        public float SideButtonGap => _sideButtonGap;
        public float SwipeHintBottom => _swipeHintBottom;

        public string FormatLevel(int level)
        {
            return _levelLabelPrefix + level;
        }
    }
}
