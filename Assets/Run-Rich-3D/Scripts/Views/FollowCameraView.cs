using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class FollowCameraView : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 5.8f, -8.4f);
        [SerializeField] private float _pitch = 18f;
        [SerializeField] private float _horizontalSmoothTime = 0.08f;

        private float _horizontalVelocity;

        internal void SetTarget(Transform target)
        {
            _target = target;
        }

        private void LateUpdate()
        {
            if (_target == null)
            {
                return;
            }

            Vector3 targetPosition = _target.position;
            float x = Mathf.SmoothDamp(
                transform.position.x,
                targetPosition.x + _offset.x,
                ref _horizontalVelocity,
                _horizontalSmoothTime);

            transform.position = new Vector3(
                x,
                targetPosition.y + _offset.y,
                targetPosition.z + _offset.z);

            transform.rotation = Quaternion.Euler(_pitch, 0f, 0f);
        }
    }
}
