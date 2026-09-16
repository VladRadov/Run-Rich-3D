using UnityEngine;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LightingService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Light _sun;
        [SerializeField] private Material _skybox;

        [Header("Ambient")]
        [SerializeField] private Color _ambientSky = new Color(0.62f, 0.86f, 1f);
        [SerializeField] private Color _ambientEquator = new Color(0.48f, 0.78f, 0.95f);
        [SerializeField] private Color _ambientGround = new Color(0.78f, 0.78f, 0.72f);

        private SceneLightingView _view;

        public void Initialize()
        {
            GameObject host = _sun != null ? _sun.gameObject : gameObject;
            _view = EntityViewFactory.CreateOn<SceneLightingView>(host);
            _view.Setup(_sun, _skybox, _ambientSky, _ambientEquator, _ambientGround);
        }

        public void Dispose()
        {
        }
    }
}
