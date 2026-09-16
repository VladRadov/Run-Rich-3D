using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class PickupSignView : MonoBehaviour
    {
        private Transform _cached;
        private Transform _anchor;
        private MeshRenderer _renderer;
        private Vector3 _worldOffset;
        private Quaternion _facing;
        private float _worldScale = 0.7f;

        public void Bind(Transform anchor, Vector3 worldOffset, Quaternion facing, float worldScale)
        {
            _cached = transform;
            _anchor = anchor;
            if (_renderer == null)
            {
                _renderer = GetComponent<MeshRenderer>();
            }

            _worldOffset = worldOffset;
            _facing = facing;
            _worldScale = worldScale > 0.01f ? worldScale : 0.7f;
            Apply();
        }

        private void LateUpdate()
        {
            Apply();
        }

        private void Apply()
        {
            if (_cached == null || _anchor == null)
            {
                return;
            }

            bool visible = _anchor.gameObject.activeInHierarchy;
            if (_renderer != null && _renderer.enabled != visible)
            {
                _renderer.enabled = visible;
            }

            if (!visible)
            {
                return;
            }

            _cached.SetPositionAndRotation(_anchor.position + _worldOffset, _facing);
            Vector3 parentScale = _anchor.lossyScale;
            _cached.localScale = new Vector3(
                _worldScale / Mathf.Max(0.01f, Mathf.Abs(parentScale.x)),
                _worldScale / Mathf.Max(0.01f, Mathf.Abs(parentScale.y)),
                _worldScale / Mathf.Max(0.01f, Mathf.Abs(parentScale.z)));
        }
    }
}
