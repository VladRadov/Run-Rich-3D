using UnityEngine;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Audio", fileName = "AudioSettings")]
    public sealed class AudioSettings : ScriptableObject
    {
        [Header("Clips")]
        [SerializeField] private AudioClip[] _footsteps;
        [SerializeField] private AudioClip _dollar;
        [SerializeField] private AudioClip _bottle;
        [SerializeField] private AudioClip _flag;
        [SerializeField] private AudioClip _status;
        [SerializeField] private AudioClip _door;
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _lose;

        [Header("Tuning")]
        [SerializeField] private float _stepInterval = 0.52f;
        [SerializeField] private float _minStepInterval = 0.05f;
        [SerializeField] private float _sfxVolume = 0.9f;
        [SerializeField] private float _footstepVolume = 0.55f;

        public AudioClip[] Footsteps => _footsteps;
        public AudioClip Dollar => _dollar;
        public AudioClip Bottle => _bottle;
        public AudioClip Flag => _flag;
        public AudioClip Status => _status;
        public AudioClip Door => _door;
        public AudioClip Win => _win;
        public AudioClip Lose => _lose;
        public float StepInterval => _stepInterval > _minStepInterval ? _stepInterval : _minStepInterval;
        public float SfxVolume => _sfxVolume;
        public float FootstepVolume => _footstepVolume;
    }
}
