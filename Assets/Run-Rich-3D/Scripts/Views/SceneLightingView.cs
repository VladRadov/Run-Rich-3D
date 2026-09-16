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

        internal void Setup(Light sun, Material skybox, Color ambientSky, Color ambientEquator, Color ambientGround)
        {
            _sun = sun;
            _skybox = skybox;
            _ambientSky = ambientSky;
            _ambientEquator = ambientEquator;
            _ambientGround = ambientGround;
            _cachedSun = sun;
            Apply();
        }

        internal void Apply()
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

        private static void EnsurePanoramic(Material skybox)
        {
            if (skybox.shader != null && skybox.shader.name == "Skybox/Panoramic")
            {
                return;
            }

            Shader panoramic = Shader.Find("Skybox/Panoramic");
            if (panoramic != null)
            {
                skybox.shader = panoramic;
            }
        }
    }
}
