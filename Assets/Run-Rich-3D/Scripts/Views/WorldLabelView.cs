using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class WorldLabelView : MonoBehaviour
    {
        [Header("Label")]
        [SerializeField] private TextMesh _label;
        [SerializeField] private string _text;
        [SerializeField] private Font _font;
        [SerializeField] private Color _color = Color.white;
        [SerializeField] private float _characterSize = 0.06f;
        [SerializeField] private int _fontSize = 64;

        internal static Quaternion TowardCamera { get; } = Quaternion.identity;

        private void OnEnable()
        {
            if (!string.IsNullOrEmpty(_text))
            {
                Bind(_text, _font, _color, _characterSize, _fontSize);
            }
        }

        internal void Bind(string text, Font font, Color color, float characterSize, int fontSize)
        {
            if (_label == null)
            {
                _label = GetComponent<TextMesh>();
            }

            if (_label == null)
            {
                return;
            }

            _label.text = text;
            _label.font = font;
            _label.color = color;
            _label.characterSize = characterSize;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.fontSize = fontSize;

            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null && font != null && font.material != null)
            {
                renderer.sharedMaterial = font.material;
            }
        }
    }
}
