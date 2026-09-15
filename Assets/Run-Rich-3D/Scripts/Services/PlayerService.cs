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

        [Header("Cowboy Meshes")]
        [SerializeField] private Mesh _casualMesh;
        [SerializeField] private Mesh _poorMesh;
        [SerializeField] private Mesh _middleMesh;
        [SerializeField] private Mesh _richMesh;
        [SerializeField] private Mesh _millionaireMesh;
        [SerializeField] private Material _playerMaterial;
        [SerializeField] private Texture2D _playerAtlas;

        [Header("Movement")]
        [SerializeField] private float _pathWidth = 5.2f;
        [SerializeField] private float _steerSensitivity = 2.1f;
        [SerializeField] private float _forwardSpeed = 12f;
        [SerializeField] private float _offPathSlack = 0.9f;

        [Header("Outfit")]
        [SerializeField] private float _outfitHeight = 1.85f;
        [SerializeField] private float _spinDuration = 0.48f;
        [SerializeField] private Vector3 _outfitStandEuler = new Vector3(90f, 0f, 0f);
        [SerializeField] private int _startWealth;

        [Header("Wealth HUD")]
        [SerializeField] private int _comfortableWealth = 40;
        [SerializeField] private int _richWealth = 80;
        [SerializeField] private int _maxDisplayWealth = 120;

        [Header("Outfit Thresholds")]
        [SerializeField] private int _poorOutfitWealth = 20;
        [SerializeField] private int _middleOutfitWealth = 40;
        [SerializeField] private int _richOutfitWealth = 80;
        [SerializeField] private int _millionaireOutfitWealth = 120;

        [Header("Status Banner")]
        [SerializeField] private Vector3 _bannerLocalPosition = new Vector3(0f, 2.35f, 0f);
        [SerializeField] private Vector3 _bannerLookOffset = new Vector3(0f, 3.45f, -8.4f);
        [SerializeField] private Vector3 _bannerTrackScale = new Vector3(1.2f, 0.1f, 0.1f);
        [SerializeField] private Vector3 _bannerFillScale = new Vector3(1.2f, 0.12f, 0.12f);
        [SerializeField] private Vector3 _bannerLabelOffset = new Vector3(0f, 0.28f, 0f);
        [SerializeField] private float _bannerLabelCharacterSize = 0.045f;
        [SerializeField] private int _bannerLabelFontSize = 64;
        [SerializeField] private float _bannerMinFill = 0.08f;
        [SerializeField] private float _materialGlossiness = 0.18f;
        [SerializeField] private float _materialMetallic = 0.05f;

        private PlayerModel _model;
        private PlayerView _view;
        private PlayerController _controller;
        private Material _runtimeMaterial;

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
            _view.BuildOutfits(CollectMeshes(), CreatePlayerMaterial(), _outfitHeight, _outfitStandEuler);
            _view.AttachBanner(CreateBanner());
            _view.SetStatus(WealthTier.Poor, 0f);

            _controller = new PlayerController(
                _model,
                _view,
                _inputService.Model,
                _pathWidth,
                _steerSensitivity,
                _forwardSpeed,
                _offPathSlack);
            _controller.Initialize();
        }

        internal void BindPath(PathBend path)
        {
            _controller?.BindPath(path);
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
                _poorOutfitWealth,
                _middleOutfitWealth,
                _richOutfitWealth,
                _millionaireOutfitWealth);
        }

        private Mesh[] CollectMeshes()
        {
            return new[]
            {
                _casualMesh,
                _poorMesh,
                _middleMesh,
                _richMesh,
                _millionaireMesh
            };
        }

        private Material CreatePlayerMaterial()
        {
            if (_runtimeMaterial != null)
            {
                return _runtimeMaterial;
            }

            Shader shader = Shader.Find("Standard");
            _runtimeMaterial = shader != null ? new Material(shader) : _playerMaterial;
            if (_runtimeMaterial == null)
            {
                return null;
            }

            Texture atlas = _playerAtlas;
            if (atlas == null && _playerMaterial != null)
            {
                atlas = _playerMaterial.GetTexture("_MainTex");
            }

            if (atlas != null)
            {
                _runtimeMaterial.mainTexture = atlas;
            }

            _runtimeMaterial.color = Color.white;
            _runtimeMaterial.SetFloat("_Glossiness", _materialGlossiness);
            _runtimeMaterial.SetFloat("_Metallic", _materialMetallic);
            return _runtimeMaterial;
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
