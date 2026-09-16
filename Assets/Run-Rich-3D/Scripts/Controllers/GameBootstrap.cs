using UnityEngine;
using RunRich3D.Services;

namespace RunRich3D.Controllers
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [Header("Services")]
        [SerializeField] private LightingService _lightingService;
        [SerializeField] private InputService _inputService;
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private CameraService _cameraService;
        [SerializeField] private LevelService _levelService;
        [SerializeField] private PickupEffectService _pickupEffectService;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private GameLoopService _gameLoopService;

        private void Awake()
        {
            _lightingService.Initialize();
            _inputService.Initialize();
            _playerService.Initialize();
            _cameraService.Initialize();
            _levelService.Initialize();
            _pickupEffectService.Initialize();
            if (_audioService != null)
            {
                _audioService.Initialize();
            }
            _gameLoopService.Initialize();
        }

        private void OnDestroy()
        {
            _gameLoopService?.Dispose();
            _audioService?.Dispose();
            _pickupEffectService?.Dispose();
            _levelService?.Dispose();
            _cameraService?.Dispose();
            _playerService?.Dispose();
            _inputService?.Dispose();
            _lightingService?.Dispose();
        }
    }
}
