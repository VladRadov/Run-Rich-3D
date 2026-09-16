using UnityEngine;
using UnityEngine.Rendering;

namespace RunRich3D.Views
{
    public sealed class SceneLightingView : MonoBehaviour
    {
        [Header("Light")]
        [SerializeField] private Light _sun;
        [SerializeField] private Material _skybox;

        [Header("Ambient")]
        [SerializeField] private Color _ambientSky = new Color(0.62f, 0.86f, 1f);
        [SerializeField] private Color _ambientEquator = new Color(0.48f, 0.78f, 0.95f);
        [SerializeField] private Color _ambientGround = new Color(0.78f, 0.78f, 0.72f);

        private Light _cachedSun;
        private string _panoramicShaderName = "Skybox/Panoramic";

        public void Setup(Light sun, Material skybox, Color ambientSky, Color ambientEquator, Color ambientGround)
        {
            Setup(sun, skybox, ambientSky, ambientEquator, ambientGround, _panoramicShaderName);
        }

        public void Setup(Light sun, Material skybox, Color ambientSky, Color ambientEquator, Color ambientGround, string panoramicShaderName)
        {
            _sun = sun;
            _skybox = skybox;
            _ambientSky = ambientSky;
            _ambientEquator = ambientEquator;
            _ambientGround = ambientGround;
            _cachedSun = sun;
            _panoramicShaderName = panoramicShaderName;
            Apply();
        }

        public void Apply()
        {
            if (_skybox != null)
            {
                EnsurePanoramic(_skybox);
                RenderSettings.skybox = _skybox;
                DynamicGI.UpdateEnvironment();
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = _ambientSky;
            RenderSettings.ambientEquatorColor = _ambientEquator;
            RenderSettings.ambientGroundColor = _ambientGround;
            RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
            RenderSettings.sun = _cachedSun;

            if (_cachedSun != null)
            {
                _cachedSun.shadows = LightShadows.Soft;
            }
        }

        private void EnsurePanoramic(Material skybox)
        {
            if (skybox.shader != null && skybox.shader.name == _panoramicShaderName)
            {
                return;
            }

            Shader panoramic = Shader.Find(_panoramicShaderName);
            if (panoramic != null)
            {
                skybox.shader = panoramic;
            }
        }
    }
}
