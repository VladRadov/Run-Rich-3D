using UnityEngine;
using UnityEngine.UI;

namespace RunRich3D.Views
{
    public sealed class PickupToastView : MonoBehaviour
    {
        private static readonly Color GainColor = new Color(0.38f, 0.92f, 0.42f);
        private static readonly Color LossColor = new Color(1f, 0.28f, 0.28f);

        private RectTransform _rect;
        private CanvasGroup _group;
        private Text _label;
        private RawImage _icon;
        private Vector2 _restPosition;
        private bool _built;

        internal void Bind(Font font, Texture2D dollarTexture, Vector2 restPosition)
        {
            _restPosition = restPosition;
            EnsureBuilt(font, dollarTexture);
            SetPresentation(1f, 0f);
        }

        internal void SetRestPosition(Vector2 restPosition)
        {
            _restPosition = restPosition;
        }

        internal void SetDelta(int signedAmount)
        {
            if (_label != null)
            {
                _label.text = signedAmount > 0
                    ? "+" + signedAmount + " $"
                    : signedAmount + " $";
                _label.color = signedAmount >= 0 ? GainColor : LossColor;
            }

            if (_icon != null)
            {
                _icon.color = signedAmount >= 0 ? Color.white : LossColor;
            }
        }

        internal void SetPresentation(float alpha, float offsetY)
        {
            if (_group != null)
            {
                _group.alpha = Mathf.Clamp01(alpha);
            }

            if (_rect != null)
            {
                _rect.anchoredPosition = _restPosition + new Vector2(0f, offsetY);
            }
        }

        private void EnsureBuilt(Font font, Texture2D dollarTexture)
        {
            if (_built)
            {
                return;
            }

            _rect = (RectTransform)transform;
            _rect.anchorMin = new Vector2(0.5f, 0.5f);
            _rect.anchorMax = new Vector2(0.5f, 0.5f);
            _rect.pivot = new Vector2(0.5f, 0.5f);
            _rect.sizeDelta = new Vector2(460f, 110f);
            _rect.anchoredPosition = _restPosition;

            _group = GetComponent<CanvasGroup>();
            if (_group == null)
            {
                _group = gameObject.AddComponent<CanvasGroup>();
            }

            _group.interactable = false;
            _group.blocksRaycasts = false;

            if (dollarTexture != null)
            {
                var icon = CreateChild("Icon", _rect);
                _icon = icon.gameObject.AddComponent<RawImage>();
                _icon.texture = dollarTexture;
                _icon.raycastTarget = false;
                icon.anchorMin = new Vector2(0.5f, 0.5f);
                icon.anchorMax = new Vector2(0.5f, 0.5f);
                icon.pivot = new Vector2(1f, 0.5f);
                icon.anchoredPosition = new Vector2(-12f, 0f);
                icon.sizeDelta = new Vector2(78f, 78f);
            }

            var labelRect = CreateChild("Amount", _rect);
            labelRect.anchorMin = new Vector2(0.5f, 0.5f);
            labelRect.anchorMax = new Vector2(0.5f, 0.5f);
            labelRect.pivot = new Vector2(0f, 0.5f);
            labelRect.anchoredPosition = new Vector2(4f, 0f);
            labelRect.sizeDelta = new Vector2(280f, 110f);

            _label = labelRect.gameObject.AddComponent<Text>();
            _label.font = font;
            _label.fontSize = 64;
            _label.fontStyle = FontStyle.Bold;
            _label.alignment = TextAnchor.MiddleLeft;
            _label.color = new Color(0.38f, 0.92f, 0.42f);
            _label.horizontalOverflow = HorizontalWrapMode.Overflow;
            _label.verticalOverflow = VerticalWrapMode.Overflow;
            _label.raycastTarget = false;

            var outline = labelRect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.6f);
            outline.effectDistance = new Vector2(2f, -2f);

            _built = true;
        }

        private static RectTransform CreateChild(string name, RectTransform parent)
        {
            var child = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)child.transform;
            rect.SetParent(parent, false);
            return rect;
        }
    }
}
