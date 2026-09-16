using UnityEngine;
using Zenject;
using RunRich3D.Views;
using LightingSettings = RunRich3D.Settings.LightingSettings;

namespace RunRich3D.Services
{
    public sealed class LightingService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Light _sun;

        private LightingSettings _settings;
        private SceneLightingView _view;

        [Inject]
        public void Construct(LightingSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            GameObject host = _sun != null ? _sun.gameObject : gameObject;
            _view = EntityViewFactory.CreateOn<SceneLightingView>(host);
            _view.Setup(
                _sun,
                _settings.Skybox,
                _settings.AmbientSky,
                _settings.AmbientEquator,
                _settings.AmbientGround,
                _settings.PanoramicShaderName);
            if (_sun != null)
            {
                _sun.color = _settings.SunColor;
                _sun.intensity = _settings.SunIntensity;
                _sun.shadowStrength = _settings.SunShadowStrength;
            }
        }

        public void Dispose()
        {
        }
    }
}
