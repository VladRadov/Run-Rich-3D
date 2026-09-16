using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Services;
using RunRich3D.Settings;
using AudioSettings = RunRich3D.Settings.AudioSettings;
using LightingSettings = RunRich3D.Settings.LightingSettings;

namespace RunRich3D.Installers
{
    public sealed class GameInstaller : MonoInstaller
    {
        [Header("Settings")]
        [SerializeField] private PlayerSettings _playerSettings;
        [SerializeField] private CameraSettings _cameraSettings;
        [SerializeField] private AudioSettings _audioSettings;
        [SerializeField] private LightingSettings _lightingSettings;
        [SerializeField] private InputSettings _inputSettings;
        [SerializeField] private HudSettings _hudSettings;
        [SerializeField] private LevelSettings _levelSettings;

        [Header("Services")]
        [SerializeField] private LightingService _lightingService;
        [SerializeField] private InputService _inputService;
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private CameraService _cameraService;
        [SerializeField] private LevelService _levelService;
        [SerializeField] private PickupEffectService _pickupEffectService;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private GameLoopService _gameLoopService;

        public override void InstallBindings()
        {
            BindSettings();
            BindServices();
            Container.BindInterfacesAndSelfTo<GameEntryPoint>().AsSingle();
        }

        private void BindSettings()
        {
            BindInstance(_playerSettings);
            BindInstance(_cameraSettings);
            BindInstance(_audioSettings);
            BindInstance(_lightingSettings);
            BindInstance(_inputSettings);
            BindInstance(_hudSettings);
            BindInstance(_levelSettings);
        }

        private void BindServices()
        {
            BindInstance(_lightingService);
            BindInstance(_inputService);
            BindInstance(_playerService);
            BindInstance(_cameraService);
            BindInstance(_levelService);
            BindInstance(_pickupEffectService);
            BindInstance(_audioService);
            BindInstance(_gameLoopService);
        }

        private void BindInstance<T>(T instance) where T : UnityEngine.Object
        {
            if (instance != null)
            {
                Container.BindInstance(instance);
            }
        }
    }
}
