using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class CameraService : MonoBehaviour, IGameService
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerService _playerService;

        private FollowCameraView _view;
        private CameraController _controller;

        internal FollowCameraView View => _view;

        public void Initialize()
        {
            _view = EntityViewFactory.CreateOn<FollowCameraView>(_camera.gameObject);
            _controller = new CameraController(_view, _playerService.View);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
