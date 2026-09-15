using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class FlagView : LevelPieceView
    {
        [Header("Motion")]
        [SerializeField] private float _triggerZ;
        [SerializeField] private Quaternion _downRotation = Quaternion.identity;
        [SerializeField] private Quaternion _upRotation = Quaternion.identity;

        private Quaternion _runtimeDown;
        private Quaternion _runtimeUp;
        private float _raised = -1f;
        private bool _motionBound;

        internal float TriggerZ => _triggerZ;

        private void Awake()
        {
            EnsureMotion();
        }

        internal void BindMotion(float triggerZ, Quaternion downRotation, Quaternion upRotation)
        {
            _triggerZ = triggerZ;
            _downRotation = downRotation;
            _upRotation = upRotation;
            _runtimeDown = downRotation;
            _runtimeUp = upRotation;
            _motionBound = true;
            Bind();
            SetRaised(0f);
        }

        internal void SetRaised(float raised)
        {
            EnsureMotion();
            float t = Mathf.Clamp01(raised);
            if (CachedTransform == null)
            {
                Bind();
            }

            if (Mathf.Abs(t - _raised) < 0.001f)
            {
                return;
            }

            _raised = t;
            CachedTransform.localRotation = Quaternion.Slerp(_runtimeDown, _runtimeUp, t);
        }

        private void EnsureMotion()
        {
            if (_motionBound)
            {
                return;
            }

            _runtimeDown = _downRotation;
            _runtimeUp = _upRotation;
            _motionBound = true;
            Bind();
            SetRaised(0f);
        }
    }
}
