using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class PickupSpinView : LevelPieceView
    {
        [SerializeField] private float _degreesPerSecond = 72f;

        private float _yaw;

        public void BindSpin(float degreesPerSecond)
        {
            Bind();
            _degreesPerSecond = degreesPerSecond > 0.01f ? degreesPerSecond : 72f;
            if (CachedTransform != null)
            {
                _yaw = CachedTransform.localEulerAngles.y;
            }
        }

        private void LateUpdate()
        {
            if (CachedTransform == null)
            {
                return;
            }

            _yaw += _degreesPerSecond * Time.deltaTime;
            if (_yaw >= 360f)
            {
                _yaw -= 360f;
            }

            Vector3 euler = CachedTransform.localEulerAngles;
            CachedTransform.localRotation = Quaternion.Euler(euler.x, _yaw, euler.z);
        }
    }
}
