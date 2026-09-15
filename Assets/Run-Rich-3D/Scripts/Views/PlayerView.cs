using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class PlayerView : MonoBehaviour
    {
        [SerializeField] private Transform _visualRoot;

        private Transform _cachedTransform;

        internal Transform MovementRoot
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }

                return _cachedTransform;
            }
        }

        internal void Bind(Transform visualRoot)
        {
            _cachedTransform = transform;
            _visualRoot = visualRoot != null ? visualRoot : _cachedTransform;
        }

        internal void SetPose(float lateralOffset, float forwardPosition)
        {
            MovementRoot.position = new Vector3(lateralOffset, 0f, forwardPosition);
        }
    }
}
