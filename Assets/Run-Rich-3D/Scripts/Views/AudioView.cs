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

        public void Bind(
            AudioSource sfx,
            AudioSource steps,
            AudioClip[] footsteps,
            AudioClip dollar,
            AudioClip bottle,
            AudioClip flag,
            AudioClip status,
            AudioClip door,
            AudioClip win,
            AudioClip lose)
        {
            _sfx = sfx;
            _steps = steps;
            _footsteps = footsteps;
            _dollar = dollar;
            _bottle = bottle;
            _flag = flag;
            _status = status;
            _door = door;
            _win = win;
            _lose = lose;
            _bound = _sfx != null && _steps != null;
        }

        public void PlayDollar()
        {
            Play(_dollar);
        }

        public void PlayBottle()
        {
            Play(_bottle);
        }

        public void PlayFlag()
        {
            Play(_flag);
        }

        public void PlayStatus()
        {
            Play(_status);
        }

        public void PlayDoor()
        {
            Play(_door);
        }

        public void PlayWin()
        {
            Play(_win);
        }

        public void PlayLose()
        {
            Play(_lose);
        }

        public void PlayFootstep()
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
    }
}
