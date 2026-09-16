using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Settings;

namespace RunRich3D.Views
{
    public sealed class StatusBannerView : MonoBehaviour
    {
        [Header("Banner")]
        [SerializeField] private TextMesh _label;
        [SerializeField] private Transform _fill;
        [SerializeField] private MeshRenderer _fillRenderer;

        private PlayerSettings _settings;
        private Material _fillMaterial;
        private float _fillMaxWidth;
        private float _minFill = 0.08f;
        private Transform _camera;
        private Transform _labelTransform;
        private Vector3 _labelRestScale = Vector3.one;
        private WealthTier _shownTier;
        private bool _hasShownTier;
        private float _punchElapsed = -1f;

        public void Bind(
            TextMesh label,
            Transform fill,
            MeshRenderer fillRenderer,
            Material fillMaterial,
            Font font,
            float characterSize,
            int fontSize,
            float minFill,
            PlayerSettings settings)
        {
            _label = label;
            _fill = fill;
            _fillRenderer = fillRenderer;
            _fillMaterial = fillMaterial;
            _fillMaxWidth = fill != null ? fill.localScale.x : 1.2f;
            _minFill = minFill;
            _settings = settings;
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

        public void BindCamera(Transform camera)
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

        public void SetStatus(WealthTier tier, float normalizedFill)
        {
            Color color = _settings != null ? _settings.StatusBarColor(tier) : WealthPalette.BarOf(tier);
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
            float duration = _settings != null ? _settings.BannerPunchDuration : 0.34f;
            float u = Mathf.Clamp01(_punchElapsed / duration);
            _labelTransform.localScale = _labelRestScale * PunchScale(u);
            if (u >= 1f)
            {
                _labelTransform.localScale = _labelRestScale;
                _punchElapsed = -1f;
            }
        }

        private float PunchScale(float u)
        {
            float peak = _settings != null ? _settings.BannerPunchPeak : 1.55f;
            float rise = _settings != null ? Mathf.Clamp(_settings.BannerPunchRisePortion, 0.01f, 0.95f) : 0.38f;

            if (u < rise)
            {
                float t = u / rise;
                t = 1f - (1f - t) * (1f - t);
                return Mathf.Lerp(1f, peak, t);
            }

            float back = (u - rise) / (1f - rise);
            back = back * back;
            return Mathf.Lerp(peak, 1f, back);
        }
    }
}
