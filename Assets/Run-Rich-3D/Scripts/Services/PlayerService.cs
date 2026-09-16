using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PlayerService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private GameObject _playerEntity;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private Font _labelFont;
        [SerializeField] private RuntimeAnimatorController _animatorController;
        [SerializeField] private StatusBannerView _statusBanner;

        private PlayerSettings _settings;
        private InputService _inputService;
        private PlayerModel _model;
        private PlayerView _view;
        private PlayerController _controller;

        public PlayerModel Model => _model;
        public PlayerView View => _view;

        [Inject]
        public void Construct(PlayerSettings settings, InputService inputService)
        {
            _settings = settings;
            _inputService = inputService;
        }

        public void Initialize()
        {
            _model = new PlayerModel(_settings.CreateWealthRules());
            _model.Reset(_settings.StartWealth);

            Font font = _labelFont;
            if (font == null)
            {
                font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }

            _view = EntityViewFactory.CreateOn<PlayerView>(_playerEntity);
            _view.Bind(_visualRoot, _settings.SpinDuration);
            _view.BindPlayerSkins(_animatorController, _settings.OutfitHeight);
            if (_statusBanner == null)
            {
                _statusBanner = new StatusBannerFactory(_settings, font).Create(_playerEntity.transform);
            }

            _view.AttachBanner(_statusBanner);
            _view.SetStatus(WealthTier.Poor, 0f);

            _controller = new PlayerController(_model, _view, _inputService.Model, _settings);
            _controller.Initialize();
        }

        public void BindPath(PathBend path)
        {
            _controller?.BindPath(path);
            _view?.AlignToSurface();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
