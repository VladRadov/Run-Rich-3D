using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class FollowCameraView : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 5.8f, -8.4f);
        [SerializeField] private float _pitch = 18f;
        [SerializeField] private float _horizontalSmoothTime = 0.08f;

        private Vector3 _followVelocity;

        public void BindSettings(Vector3 offset, float pitch, float horizontalSmoothTime)
        {
            _offset = offset;
            _pitch = pitch;
            _horizontalSmoothTime = horizontalSmoothTime;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 desired = _target.position + _target.rotation * _offset;
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desired,
                ref _followVelocity,
                _horizontalSmoothTime);
            transform.rotation = Quaternion.Euler(_pitch, _target.eulerAngles.y, 0f);
        }
    }
}
