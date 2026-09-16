using System;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class CameraController : IDisposable
    {
        private readonly FollowCameraView _view;
        private readonly PlayerView _playerView;

        public CameraController(FollowCameraView view, PlayerView playerView)
        {
            _view = view;
            _playerView = playerView;
        }

        public void Initialize()
        {
            _view.SetTarget(_playerView.MovementRoot);
        }

        public void Dispose()
        {
        }
    }
}
