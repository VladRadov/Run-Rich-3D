using UnityEngine;
using UnityEngine.Rendering;

namespace RunRich3D.Views
{
    public sealed class SceneLightingView : MonoBehaviour
    {
        [SerializeField] private Light _sun;
        [SerializeField] private Material _skybox;
        [SerializeField] private Color _ambientSky = new Color(0.62f, 0.86f, 1f);
        [SerializeField] private Color _ambientEquator = new Color(0.48f, 0.78f, 0.95f);
        [SerializeField] private Color _ambientGround = new Color(0.78f, 0.78f, 0.72f);

        private Light _cachedSun;

        internal void Setup(Light sun, Material skybox)
        {
            _sun = sun;
            _skybox = skybox;
            _cachedSun = sun;
            Apply();
        }

        internal void Apply()
        {
            if (_skybox != null)
            {
                RenderSettings.skybox = _skybox;
            }

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = _ambientSky;
            RenderSettings.ambientEquatorColor = _ambientEquator;
            RenderSettings.ambientGroundColor = _ambientGround;
            RenderSettings.sun = _cachedSun;

            if (_cachedSun != null)
            {
                _cachedSun.shadows = LightShadows.Soft;
            }
        }
    }
}
