using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Player", fileName = "PlayerSettings")]
    public sealed class PlayerSettings : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float _pathWidth = 5.2f;
        [SerializeField] private float _steerSensitivity = 2.1f;
        [SerializeField] private float _forwardSpeed = 5f;
        [SerializeField] private float _offPathSlack;
        [SerializeField] private float _maxSteerYaw = 42f;
        [SerializeField] private float _steerYawPerSpeed = 11f;
        [SerializeField] private float _steerYawSmooth = 10f;
        [SerializeField] private float _lateralSmoothTime = 0.14f;
        [SerializeField] private float _minLateralLimit = 0.1f;
        [SerializeField] private float _steerIdleSnapDegrees = 0.05f;

        [Header("Outfit")]
        [SerializeField] private float _outfitHeight = 2.7f;
        [SerializeField] private float _spinDuration = 0.48f;
        [SerializeField] private int _startWealth;

        [Header("Wealth HUD")]
        [SerializeField] private int _comfortableWealth = 40;
        [SerializeField] private int _richWealth = 80;
        [SerializeField] private int _maxDisplayWealth = 120;

        [Header("Outfit Thresholds")]
        [SerializeField] private int _middleOutfitWealth = 40;
        [SerializeField] private int _casualOutfitWealth = 60;
        [SerializeField] private int _cocktailOutfitWealth = 80;
        [SerializeField] private int _businessOutfitWealth = 100;
        [SerializeField] private int _blingOutfitWealth = 120;

        [Header("Status Banner")]
        [SerializeField] private Vector3 _bannerLocalPosition = new Vector3(0f, 3.4f, 0f);
        [SerializeField] private Vector3 _bannerLookOffset = new Vector3(0f, 3.45f, -8.4f);
        [SerializeField] private Vector3 _bannerTrackScale = new Vector3(1.2f, 0.1f, 0.1f);
        [SerializeField] private Vector3 _bannerFillScale = new Vector3(1.2f, 0.12f, 0.12f);
        [SerializeField] private Vector3 _bannerLabelOffset = new Vector3(0f, 0.28f, 0f);
        [SerializeField] private float _bannerLabelCharacterSize = 0.045f;
        [SerializeField] private int _bannerLabelFontSize = 64;
        [SerializeField] private float _bannerMinFill = 0.08f;
        [SerializeField] private Color _bannerTrackColor = new Color(0.12f, 0.12f, 0.14f);
        [SerializeField] private float _bannerTrackGlossiness = 0.15f;
        [SerializeField] private float _bannerPunchDuration = 0.34f;
        [SerializeField] private float _bannerPunchPeak = 1.55f;
        [SerializeField] private float _bannerPunchRisePortion = 0.38f;
        [SerializeField] private string _standardShaderName = "Standard";

        [Header("Wealth Colors")]
        [SerializeField] private Color _poorColor = new Color(0.45f, 0.32f, 0.22f);
        [SerializeField] private Color _comfortableColor = new Color(0.35f, 0.48f, 0.72f);
        [SerializeField] private Color _richColor = new Color(0.92f, 0.74f, 0.18f);
        [SerializeField] private Color _poorBarColor = new Color(0.92f, 0.28f, 0.28f);
        [SerializeField] private Color _comfortableBarColor = new Color(0.98f, 0.78f, 0.18f);
        [SerializeField] private Color _richBarColor = new Color(0.28f, 0.82f, 0.38f);

        public float PathWidth => _pathWidth;
        public float SteerSensitivity => _steerSensitivity;
        public float ForwardSpeed => _forwardSpeed;
        public float OffPathSlack => _offPathSlack;
        public float MaxSteerYaw => _maxSteerYaw;
        public float SteerYawPerSpeed => _steerYawPerSpeed;
        public float SteerYawSmooth => _steerYawSmooth;
        public float LateralSmoothTime => _lateralSmoothTime;
        public float MinLateralLimit => _minLateralLimit;
        public float SteerIdleSnapDegrees => _steerIdleSnapDegrees;
        public float OutfitHeight => _outfitHeight;
        public float SpinDuration => _spinDuration;
        public int StartWealth => _startWealth;
        public Vector3 BannerLocalPosition => _bannerLocalPosition;
        public Vector3 BannerLookOffset => _bannerLookOffset;
        public Vector3 BannerTrackScale => _bannerTrackScale;
        public Vector3 BannerFillScale => _bannerFillScale;
        public Vector3 BannerLabelOffset => _bannerLabelOffset;
        public float BannerLabelCharacterSize => _bannerLabelCharacterSize;
        public int BannerLabelFontSize => _bannerLabelFontSize;
        public float BannerMinFill => _bannerMinFill;
        public Color BannerTrackColor => _bannerTrackColor;
        public float BannerTrackGlossiness => _bannerTrackGlossiness;
        public float BannerPunchDuration => _bannerPunchDuration > 0.01f ? _bannerPunchDuration : 0.34f;
        public float BannerPunchPeak => _bannerPunchPeak > 0.01f ? _bannerPunchPeak : 1.55f;
        public float BannerPunchRisePortion => Mathf.Clamp01(_bannerPunchRisePortion);
        public string StandardShaderName => string.IsNullOrEmpty(_standardShaderName) ? "Standard" : _standardShaderName;

        public Color StatusColor(WealthTier tier)
        {
            switch (tier)
            {
                case WealthTier.Comfortable:
                    return _comfortableColor;
                case WealthTier.Rich:
                    return _richColor;
                default:
                    return _poorColor;
            }
        }

        public Color StatusBarColor(WealthTier tier)
        {
            switch (tier)
            {
                case WealthTier.Comfortable:
                    return _comfortableBarColor;
                case WealthTier.Rich:
                    return _richBarColor;
                default:
                    return _poorBarColor;
            }
        }

        public WealthRules CreateWealthRules()
        {
            return new WealthRules(
                _comfortableWealth,
                _richWealth,
                _maxDisplayWealth,
                _middleOutfitWealth,
                _casualOutfitWealth,
                _cocktailOutfitWealth,
                _businessOutfitWealth,
                _blingOutfitWealth);
        }
    }
}
