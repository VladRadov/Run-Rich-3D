using UnityEngine;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LightingService : MonoBehaviour, IGameService
    {
        [SerializeField] private Light _sun;
        [SerializeField] private Material _skybox;

        private SceneLightingView _view;

        public void Initialize()
        {
            _view = EntityViewFactory.CreateOn<SceneLightingView>(_sun.gameObject);
            _view.Setup(_sun, _skybox);
        }

        public void Dispose()
        {
        }
    }
}
