using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Views
{
    public sealed class StatusBannerView : MonoBehaviour
    {
        [Header("Banner")]
        [SerializeField] private TextMesh _label;
        [SerializeField] private Transform _fill;
        [SerializeField] private MeshRenderer _fillRenderer;

        private Material _fillMaterial;
        private float _fillMaxWidth;
        private float _minFill = 0.08f;
        private Transform _camera;

        internal void Bind(
            TextMesh label,
            Transform fill,
            MeshRenderer fillRenderer,
            Material fillMaterial,
            Font font,
            float characterSize,
            int fontSize,
            float minFill)
        {
            _label = label;
            _fill = fill;
            _fillRenderer = fillRenderer;
            _fillMaterial = fillMaterial;
            _fillMaxWidth = fill != null ? fill.localScale.x : 1.2f;
            _minFill = minFill;

            if (_label != null)
            {
                _label.transform.localRotation = Quaternion.identity;
                _label.font = font;
                _label.anchor = TextAnchor.MiddleCenter;
                _label.alignment = TextAlignment.Center;
                _label.characterSize = characterSize;
                _label.fontSize = fontSize;
                if (font != null && font.material != null)
                {
                    var renderer = _label.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = font.material;
                    }
                }
            }

            if (_fillRenderer != null && _fillMaterial != null)
            {
                _fillRenderer.sharedMaterial = _fillMaterial;
            }
        }

        internal void BindCamera(Transform camera)
        {
            _camera = camera;
        }

        private void LateUpdate()
        {
            if (_label == null || _camera == null)
            {
                return;
            }

            Vector3 toCamera = _camera.position - _label.transform.position;
            if (toCamera.sqrMagnitude < 0.0001f)
            {
                return;
            }

            _label.transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
        }

        internal void SetStatus(WealthTier tier, float normalizedFill)
        {
            Color color = WealthPalette.BarOf(tier);
            if (_label != null)
            {
                _label.text = WealthScale.Label(tier);
                _label.color = color;
            }

            if (_fill != null)
            {
                float width = Mathf.Max(_minFill, Mathf.Clamp01(normalizedFill)) * _fillMaxWidth;
                Vector3 scale = _fill.localScale;
                scale.x = width;
                _fill.localScale = scale;
                Vector3 position = _fill.localPosition;
                position.x = (width - _fillMaxWidth) * 0.5f;
                _fill.localPosition = position;
            }

            if (_fillMaterial != null)
            {
                _fillMaterial.color = color;
            }
        }
    }
}
