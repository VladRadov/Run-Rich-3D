using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class CameraService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Camera _camera;
        [SerializeField] private PlayerService _playerService;

        [Header("Follow")]
        [SerializeField] private Vector3 _offset = new Vector3(0f, 5.8f, -8.4f);
        [SerializeField] private float _pitch = 18f;
        [SerializeField] private float _horizontalSmoothTime = 0.08f;

        private FollowCameraView _view;
        private CameraController _controller;

        internal FollowCameraView View => _view;

        public void Initialize()
        {
            _view = EntityViewFactory.CreateOn<FollowCameraView>(_camera.gameObject);
            _view.BindSettings(_offset, _pitch, _horizontalSmoothTime);
            _camera.clearFlags = CameraClearFlags.Skybox;
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
