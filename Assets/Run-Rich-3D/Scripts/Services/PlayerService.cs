using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PlayerService : MonoBehaviour, IGameService
    {
        [SerializeField] private GameObject _playerEntity;
        [SerializeField] private Transform _visualRoot;
        [SerializeField] private InputService _inputService;
        [SerializeField] private float _pathWidth = 5.2f;
        [SerializeField] private float _steerSensitivity = 2.1f;
        [SerializeField] private float _forwardSpeed = 12f;
        [SerializeField] private float _offPathSlack = 0.9f;

        private PlayerModel _model;
        private PlayerView _view;
        private PlayerController _controller;

        internal PlayerModel Model => _model;
        internal PlayerView View => _view;

        public void Initialize()
        {
            _model = new PlayerModel();
            _model.Reset(0);

            _view = EntityViewFactory.CreateOn<PlayerView>(_playerEntity);
            _view.Bind(_visualRoot);

            _controller = new PlayerController(
                _model,
                _view,
                _inputService.Model,
                _pathWidth,
                _steerSensitivity,
                _forwardSpeed,
                _offPathSlack);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }
    }
}
