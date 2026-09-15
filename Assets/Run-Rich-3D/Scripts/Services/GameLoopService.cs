using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class GameLoopService : MonoBehaviour, IGameService
    {
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private InputService _inputService;
        [SerializeField] private LevelService _levelService;
        [SerializeField] private float _loseAbsX = 3.2f;
        [SerializeField] private Font _font;
        [SerializeField] private Texture2D _buttonTexture;
        [SerializeField] private Texture2D _retryTexture;
        [SerializeField] private Texture2D _dollarTexture;

        private GameLoopModel _model;
        private HudView _view;
        private GameLoopController _controller;

        internal GameLoopModel Model => _model;

        public void Initialize()
        {
            GameObject hudRoot = EnsureHudRoot();
            _model = new GameLoopModel();
            _view = EntityViewFactory.CreateOn<HudView>(hudRoot);
            _view.Bind(_font, _buttonTexture, _retryTexture, _dollarTexture);

            _controller = new GameLoopController(
                _model,
                _playerService.Model,
                _view,
                _levelService.Events,
                _loseAbsX);
            _controller.Initialize();
        }

        public void Dispose()
        {
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
