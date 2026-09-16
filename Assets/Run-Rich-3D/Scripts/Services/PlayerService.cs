using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PlayerService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private GameObject _playerEntity;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private InputService _inputService;
        [SerializeField] private Font _labelFont;

        [Header("Player Visual")]
        [SerializeField] private RuntimeAnimatorController _animatorController;

        [Header("Movement")]
        [SerializeField] private float _pathWidth = 5.2f;
        [SerializeField] private float _steerSensitivity = 2.1f;
        [SerializeField] private float _forwardSpeed = 12f;
        [SerializeField] private float _offPathSlack = 0f;
        [SerializeField] private float _maxSteerYaw = 42f;
        [SerializeField] private float _steerYawPerSpeed = 11f;
        [SerializeField] private float _steerYawSmooth = 10f;
        [SerializeField] private float _lateralSmoothTime = 0.14f;

        [Header("Outfit")]
        [SerializeField] private float _outfitHeight = 2.7f;
        [SerializeField] private float _spinDuration = 0.48f;
        [SerializeField] private int _startWealth;

        [Header("Wealth HUD")]
        [SerializeField] private int _comfortableWealth = 40;
        [SerializeField] private int _richWealth = 80;
        [SerializeField] private int _maxDisplayWealth = 120;

        [Header("Outfit Thresholds (min coins)")]
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

        private PlayerModel _model;
        private PlayerView _view;
        private PlayerController _controller;

        internal PlayerModel Model => _model;
        internal PlayerView View => _view;

        public void Initialize()
        {
            _model = new PlayerModel(CreateWealthRules());
            _model.Reset(_startWealth);

            if (_labelFont == null)
            {
                _labelFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            _view = EntityViewFactory.CreateOn<PlayerView>(_playerEntity);
            _view.Bind(_visualRoot, _spinDuration);
            _view.BindPlayerSkins(_animatorController, _outfitHeight);
            _view.AttachBanner(CreateBanner());
            _view.SetStatus(WealthTier.Poor, 0f);

            _controller = new PlayerController(
                _model,
                _view,
                _inputService.Model,
                _pathWidth,
                _steerSensitivity,
                _forwardSpeed,
                _offPathSlack,
                _maxSteerYaw,
                _steerYawPerSpeed,
                _steerYawSmooth,
                _lateralSmoothTime);
            _controller.Initialize();
        }

        internal void BindPath(PathBend path)
        {
            _controller?.BindPath(path);
            _view?.AlignToSurface();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }

        private WealthRules CreateWealthRules()
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

        private StatusBannerView CreateBanner()
        {
            Transform existing = _playerEntity.transform.Find("StatusBanner");
            GameObject root = existing != null ? existing.gameObject : new GameObject("StatusBanner");
            root.transform.SetParent(_playerEntity.transform, false);
            root.transform.localPosition = _bannerLocalPosition;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;

            Transform trackTransform = EnsureCube(root.transform, "Track", _bannerTrackScale, Vector3.zero);
            Transform fillTransform = EnsureCube(root.transform, "Fill", _bannerFillScale, Vector3.zero);
            TextMesh label = EnsureLabel(root.transform);

            var trackRenderer = trackTransform.GetComponent<MeshRenderer>();
            var fillRenderer = fillTransform.GetComponent<MeshRenderer>();
            Material trackMaterial = CreateTint(new Color(0.12f, 0.12f, 0.14f));
            Material fillMaterial = CreateTint(WealthPalette.BarOf(WealthTier.Poor));
            if (trackRenderer != null)
            {
                trackRenderer.sharedMaterial = trackMaterial;
            }

            var banner = EntityViewFactory.CreateOn<StatusBannerView>(root);
            banner.Bind(label, fillTransform, fillRenderer, fillMaterial, _labelFont, _bannerLabelCharacterSize, _bannerLabelFontSize, _bannerMinFill);
            return banner;
        }

        private static Transform EnsureCube(Transform parent, string name, Vector3 scale, Vector3 localPosition)
        {
            Transform existing = parent.Find(name);
            GameObject cube;
            if (existing != null)
            {
                cube = existing.gameObject;
            }
            else
            {
                cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = name;
                cube.transform.SetParent(parent, false);
            }

            cube.transform.localPosition = localPosition;
            cube.transform.localRotation = Quaternion.identity;
            cube.transform.localScale = scale;

            var collider = cube.GetComponent<Collider>();
            if (collider != null)
            {
                Destroy(collider);
            }

            return cube.transform;
        }

        private TextMesh EnsureLabel(Transform parent)
        {
            Transform existing = parent.Find("Label");
            GameObject labelObject;
            if (existing != null)
            {
                labelObject = existing.gameObject;
            }
            else
            {
                labelObject = new GameObject("Label");
                labelObject.transform.SetParent(parent, false);
            }

            labelObject.transform.localPosition = _bannerLabelOffset;
            labelObject.transform.localRotation = Quaternion.identity;
            labelObject.transform.localScale = Vector3.one;

            var label = labelObject.GetComponent<TextMesh>();
            if (label == null)
            {
                label = labelObject.AddComponent<TextMesh>();
            }

            return label;
        }

        private static Material CreateTint(Color color)
        {
            Shader shader = Shader.Find("Standard");
            if (shader == null)
            {
                return null;
            }

            var material = new Material(shader);
            material.color = color;
            material.SetFloat("_Glossiness", 0.15f);
            return material;
        }
    }
}
