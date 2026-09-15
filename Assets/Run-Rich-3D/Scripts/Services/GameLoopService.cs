using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class GameLoopService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private InputService _inputService;
        [SerializeField] private LevelService _levelService;

        [Header("HUD")]
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

        [Header("Lose")]
        [SerializeField] private float _loseAbsX = 3.2f;

        [Header("Pickup Toast")]
        [SerializeField] private float _toastHoldSeconds = 1.1f;
        [SerializeField] private float _toastFadeSeconds = 0.45f;
        [SerializeField] private float _toastRisePixels = 90f;

        private GameLoopModel _model;
        private HudView _view;
        private GameLoopController _controller;
        private PickupToastPool _toastPool;
        private PickupToastController _toastController;

        internal GameLoopModel Model => _model;

        public void Initialize()
        {
            GameObject hudRoot = EnsureHudRoot();
            _model = new GameLoopModel();
            _view = EntityViewFactory.CreateOn<HudView>(hudRoot);
            _view.Bind(
                _font,
                _buttonTexture,
                _retryTexture,
                _billsTexture,
                _arrowTexture,
                _fingerTexture,
                _settingsTexture,
                _noAdsTexture,
                _shopSkinTexture,
                _pickupsTexture,
                _parquetTexture);

            _controller = new GameLoopController(
                _model,
                _playerService.Model,
                _view,
                _levelService.Events,
                _loseAbsX);
            _controller.Initialize();

            _toastPool = new PickupToastPool(
                hudRoot.transform,
                _font,
                _dollarTexture,
                new Vector2(0f, 80f));
            _toastController = new PickupToastController(
                _toastPool,
                _levelService.Events,
                _playerService.Model,
                _toastHoldSeconds,
                _toastFadeSeconds,
                _toastRisePixels);
            _toastController.Initialize();
        }

        public void Dispose()
        {
            _toastController?.Dispose();
            _toastController = null;
            _toastPool?.Dispose();
            _toastPool = null;
            _controller?.Dispose();
            _controller = null;
        }

        private GameObject EnsureHudRoot()
        {
            Transform canvas = _inputService.CanvasRoot;
            Transform existing = canvas.Find("GameHud");
            if (existing != null)
            {
                return existing.gameObject;
            }

            var hud = new GameObject("GameHud", typeof(RectTransform));
            hud.transform.SetParent(canvas, false);
            hud.transform.SetAsLastSibling();
            var rect = (RectTransform)hud.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return hud;
        }
    }
}
