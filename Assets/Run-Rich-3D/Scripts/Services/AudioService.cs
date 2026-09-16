using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class AudioService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private LevelService _levelService;

        [Header("Clips")]
        [SerializeField] private AudioClip[] _footsteps;
        [SerializeField] private AudioClip _dollar;
        [SerializeField] private AudioClip _bottle;
        [SerializeField] private AudioClip _flag;
        [SerializeField] private AudioClip _status;
        [SerializeField] private AudioClip _door;
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _lose;

        [Header("Tuning")]
        [SerializeField] private float _stepInterval = 0.52f;

        private AudioView _view;
        private AudioController _controller;

        public void Initialize()
        {
            _view = EntityViewFactory.CreateOn<AudioView>(gameObject);
            _view.Bind(_footsteps, _dollar, _bottle, _flag, _status, _door, _win, _lose);
            _controller = new AudioController(
                _view,
                _playerService != null ? _playerService.Model : null,
                _levelService != null ? _levelService.Events : null,
                _stepInterval);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
