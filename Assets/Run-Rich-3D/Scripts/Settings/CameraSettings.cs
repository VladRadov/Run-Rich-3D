using UnityEngine;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Camera", fileName = "CameraSettings")]
    public sealed class CameraSettings : ScriptableObject
    {
        [SerializeField] private Vector3 _followOffset = new Vector3(0f, 5.8f, -8.4f);
        [SerializeField] private float _pitchDegrees = 18f;
        [SerializeField] private float _horizontalSmoothTime = 0.08f;
        [SerializeField] private float _fieldOfView = 50f;
        [SerializeField] private float _nearClip = 0.1f;
        [SerializeField] private float _farClip = 220f;

        public Vector3 FollowOffset => _followOffset;
        public float PitchDegrees => _pitchDegrees;
        public float HorizontalSmoothTime => _horizontalSmoothTime;
        public float FieldOfView => _fieldOfView;
        public float NearClip => _nearClip;
        public float FarClip => _farClip;
    }
}
