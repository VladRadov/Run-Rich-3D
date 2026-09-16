using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class GameLoopService : MonoBehaviour, IGameService
    {
        [SerializeField] private RectTransform _hudRoot;

        private HudSettings _settings;
        private PlayerService _playerService;
        private InputService _inputService;
        private LevelService _levelService;
        private GameLoopModel _model;
        private HudView _view;
        private GameLoopController _controller;
        private PickupToastPool _toastPool;
        private PickupToastController _toastController;

        public GameLoopModel Model => _model;

        [Inject]
        public void Construct(
            HudSettings settings,
            PlayerService playerService,
            InputService inputService,
            LevelService levelService)
        {
            _settings = settings;
            _playerService = playerService;
            _inputService = inputService;
            _levelService = levelService;
        }

        public void Initialize()
        {
            GameObject hudRoot = EnsureHudRoot();
            _model = new GameLoopModel();
            _view = EntityViewFactory.CreateOn<HudView>(hudRoot);
            _view.Bind(_settings);
            _controller = new GameLoopController(
                _model,
                _playerService.Model,
                _view,
                _levelService.Events,
                _settings);
            _controller.Initialize();

            _toastPool = new PickupToastPool(
                hudRoot.transform,
                _settings.Font,
                _settings.DollarTexture,
                _settings.ToastRestPosition);
            _toastController = new PickupToastController(
                _toastPool,
                _levelService.Events,
                _playerService.Model,
                _settings.ToastHoldSeconds,
                _settings.ToastFadeSeconds,
                _settings.ToastRisePixels,
                _settings.ToastRestPosition,
                _settings.ToastLossRestPosition);
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
            if (_hudRoot != null)
            {
                return _hudRoot.gameObject;
            }

            var hud = new GameObject("GameHud", typeof(RectTransform));
            hud.transform.SetParent(_inputService.CanvasRoot, false);
            hud.transform.SetAsLastSibling();
            _hudRoot = (RectTransform)hud.transform;
            _hudRoot.anchorMin = Vector2.zero;
            _hudRoot.anchorMax = Vector2.one;
            _hudRoot.offsetMin = Vector2.zero;
            _hudRoot.offsetMax = Vector2.zero;
            return hud;
        }
    }
}
