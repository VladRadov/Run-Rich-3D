using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class WorldLabelView : MonoBehaviour
    {
        [SerializeField] private TextMesh _label;

        internal void Bind(string text, Font font, Color color, float characterSize)
        {
            if (_label == null)
            {
                _label = GetComponent<TextMesh>();
            }

            _label.text = text;
            _label.font = font;
            _label.color = color;
            _label.characterSize = characterSize;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.fontSize = 64;

            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null && font != null && font.material != null)
            {
                renderer.sharedMaterial = font.material;
            }
        }
    }
}
