using UnityEngine;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Lighting", fileName = "LightingSettings")]
    public sealed class LightingSettings : ScriptableObject
    {
        [SerializeField] private Material _skybox;
        [SerializeField] private Color _ambientSky = new Color(0.62f, 0.86f, 1f);
        [SerializeField] private Color _ambientEquator = new Color(0.48f, 0.78f, 0.95f);
        [SerializeField] private Color _ambientGround = new Color(0.78f, 0.78f, 0.72f);
        [SerializeField] private Color _sunColor = new Color(1f, 0.97f, 0.88f);
        [SerializeField] private float _sunIntensity = 1.15f;
        [SerializeField] private float _sunShadowStrength = 0.62f;
        [SerializeField] private string _panoramicShaderName = "Skybox/Panoramic";

        public Material Skybox => _skybox;
        public Color AmbientSky => _ambientSky;
        public Color AmbientEquator => _ambientEquator;
        public Color AmbientGround => _ambientGround;
        public Color SunColor => _sunColor;
        public float SunIntensity => _sunIntensity;
        public float SunShadowStrength => _sunShadowStrength;
        public string PanoramicShaderName => _panoramicShaderName;
    }
}
