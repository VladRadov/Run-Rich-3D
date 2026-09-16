using UnityEngine;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PickupToastPool : ComponentPool<PickupToastView>
    {
        private readonly Font _font;
        private readonly Texture2D _dollarTexture;
        private readonly Vector2 _restPosition;

        public PickupToastPool(Transform parent, Font font, Texture2D dollarTexture, Vector2 restPosition)
            : base(parent)
        {
            _font = font;
            _dollarTexture = dollarTexture;
            _restPosition = restPosition;
        }

        protected override PickupToastView Create()
        {
            var go = new GameObject("PickupToast", typeof(RectTransform), typeof(CanvasGroup));
            go.transform.SetParent(Parent, false);
            var view = EntityViewFactory.CreateOn<PickupToastView>(go);
            view.Bind(_font, _dollarTexture, _restPosition);
            return view;
        }
    }
}
