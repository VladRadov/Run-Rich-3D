using UnityEngine;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Settings;
using RunRich3D.Views;
using AudioSettings = RunRich3D.Settings.AudioSettings;

namespace RunRich3D.Services
{
    public sealed class AudioService : MonoBehaviour, IGameService
    {
        [SerializeField] private AudioView _view;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _stepSource;

        private AudioSettings _settings;
        private PlayerService _playerService;
        private LevelService _levelService;
        private AudioController _controller;

        [Inject]
        public void Construct(AudioSettings settings, PlayerService playerService, LevelService levelService)
        {
            _settings = settings;
            _playerService = playerService;
            _levelService = levelService;
        }

        public void Initialize()
        {
            if (_view == null)
            {
                _view = EntityViewFactory.CreateOn<AudioView>(gameObject);
            }

            _sfxSource = EnsureSource(_sfxSource, "SfxSource", _settings.SfxVolume);
            _stepSource = EnsureSource(_stepSource, "StepSource", _settings.FootstepVolume);
            _view.Bind(
                _sfxSource,
                _stepSource,
                _settings.Footsteps,
                _settings.Dollar,
                _settings.Bottle,
                _settings.Flag,
                _settings.Status,
                _settings.Door,
                _settings.Win,
                _settings.Lose);
            _controller = new AudioController(
                _view,
                _playerService != null ? _playerService.Model : null,
                _levelService != null ? _levelService.Events : null,
                _settings);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }

        private AudioSource EnsureSource(AudioSource source, string name, float volume)
        {
            if (source == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(transform, false);
                source = go.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = false;
            source.volume = volume;
            return source;
        }
    }
}
