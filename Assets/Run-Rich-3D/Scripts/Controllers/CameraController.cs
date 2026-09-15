using System;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    internal sealed class CameraController : IDisposable
    {
        private readonly FollowCameraView _view;
        private readonly PlayerView _playerView;

        internal CameraController(FollowCameraView view, PlayerView playerView)
        {
            _view = view;
            _playerView = playerView;
        }

        internal void Initialize()
        {
            _view.SetTarget(_playerView.MovementRoot);
        }

        public void Dispose()
        {
        }
    }
}
