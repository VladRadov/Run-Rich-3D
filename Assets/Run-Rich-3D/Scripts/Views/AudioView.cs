using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class AudioView : MonoBehaviour
    {
        private AudioSource _sfx;
        private AudioSource _steps;
        private AudioClip[] _footsteps;
        private AudioClip _dollar;
        private AudioClip _bottle;
        private AudioClip _flag;
        private AudioClip _status;
        private AudioClip _door;
        private AudioClip _win;
        private AudioClip _lose;
        private int _stepIndex;
        private bool _bound;

        internal void Bind(
            AudioClip[] footsteps,
            AudioClip dollar,
            AudioClip bottle,
            AudioClip flag,
            AudioClip status,
            AudioClip door,
            AudioClip win,
            AudioClip lose)
        {
            _footsteps = footsteps;
            _dollar = dollar;
            _bottle = bottle;
            _flag = flag;
            _status = status;
            _door = door;
            _win = win;
            _lose = lose;
            _sfx = EnsureSource("SfxSource", 0.9f);
            _steps = EnsureSource("StepSource", 0.55f);
            _bound = true;
        }

        internal void PlayDollar()
        {
            Play(_dollar);
        }

        internal void PlayBottle()
        {
            Play(_bottle);
        }

        internal void PlayFlag()
        {
            Play(_flag);
        }

        internal void PlayStatus()
        {
            Play(_status);
        }

        internal void PlayDoor()
        {
            Play(_door);
        }

        internal void PlayWin()
        {
            Play(_win);
        }

        internal void PlayLose()
        {
            Play(_lose);
        }

        internal void PlayFootstep()
        {
            if (!_bound || _steps == null || _footsteps == null || _footsteps.Length == 0)
            {
                return;
            }

            AudioClip clip = _footsteps[_stepIndex % _footsteps.Length];
            _stepIndex++;
            if (clip != null)
            {
                _steps.PlayOneShot(clip);
            }
        }

        private void Play(AudioClip clip)
        {
            if (!_bound || _sfx == null || clip == null)
            {
                return;
            }

            _sfx.PlayOneShot(clip);
        }

        private AudioSource EnsureSource(string name, float volume)
        {
            Transform existing = transform.Find(name);
            AudioSource source = existing != null ? existing.GetComponent<AudioSource>() : null;
            if (source == null)
            {
                var go = new GameObject(name);
                go.transform.SetParent(transform, false);
                source = go.AddComponent<AudioSource>();
            }

            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = false;
            source.volume = volume;
            return source;
        }
    }
}
