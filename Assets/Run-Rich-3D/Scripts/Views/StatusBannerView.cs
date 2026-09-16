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
        private Transform _labelTransform;
        private Vector3 _labelRestScale = Vector3.one;
        private WealthTier _shownTier;
        private bool _hasShownTier;
        private float _punchElapsed = -1f;
        private const float PunchDuration = 0.34f;
        private const float PunchPeak = 1.55f;

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
            _hasShownTier = false;
            _punchElapsed = -1f;

            if (_label != null)
            {
                _labelTransform = _label.transform;
                _labelRestScale = _labelTransform.localScale;
                if (_labelRestScale.sqrMagnitude < 0.0001f)
                {
                    _labelRestScale = Vector3.one;
                }

                _labelTransform.localRotation = Quaternion.identity;
                _labelTransform.localScale = _labelRestScale;
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
            if (_labelTransform == null)
            {
                return;
            }

            if (_camera != null)
            {
                Vector3 toCamera = _camera.position - _labelTransform.position;
                if (toCamera.sqrMagnitude > 0.0001f)
                {
                    _labelTransform.rotation = Quaternion.LookRotation(toCamera, Vector3.up) * Quaternion.Euler(0f, 180f, 0f);
                }
            }

            UpdatePunch();
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

            if (_hasShownTier && _shownTier != tier)
            {
                StartPunch();
            }

            _shownTier = tier;
            _hasShownTier = true;
        }

        private void StartPunch()
        {
            _punchElapsed = 0f;
        }

        private void UpdatePunch()
        {
            if (_punchElapsed < 0f)
            {
                return;
            }

            _punchElapsed += Time.deltaTime;
            float u = Mathf.Clamp01(_punchElapsed / PunchDuration);
            _labelTransform.localScale = _labelRestScale * PunchScale(u);
            if (u >= 1f)
            {
                _labelTransform.localScale = _labelRestScale;
                _punchElapsed = -1f;
            }
        }

        private static float PunchScale(float u)
        {
            if (u < 0.38f)
            {
                float t = u / 0.38f;
                t = 1f - (1f - t) * (1f - t);
                return Mathf.Lerp(1f, PunchPeak, t);
            }

            float back = (u - 0.38f) / 0.62f;
            back = back * back;
            return Mathf.Lerp(PunchPeak, 1f, back);
        }
    }
}
