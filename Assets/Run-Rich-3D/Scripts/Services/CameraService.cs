using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class CameraService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Camera _camera;

        private CameraSettings _settings;
        private PlayerService _playerService;
        private FollowCameraView _view;
        private CameraController _controller;

        public FollowCameraView View => _view;

        [Inject]
        public void Construct(CameraSettings settings, PlayerService playerService)
        {
            _settings = settings;
            _playerService = playerService;
        }

        public void Initialize()
        {
            _view = EntityViewFactory.CreateOn<FollowCameraView>(_camera.gameObject);
            _view.BindSettings(_settings.FollowOffset, _settings.PitchDegrees, _settings.HorizontalSmoothTime);
            _camera.clearFlags = CameraClearFlags.Skybox;
            _camera.fieldOfView = _settings.FieldOfView;
            _camera.nearClipPlane = _settings.NearClip;
            _camera.farClipPlane = _settings.FarClip;
            _controller = new CameraController(_view, _playerService.View);
            _controller.Initialize();
            if (_camera != null && _playerService.View != null)
            {
                _playerService.View.BindBannerCamera(_camera.transform);
            }
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
