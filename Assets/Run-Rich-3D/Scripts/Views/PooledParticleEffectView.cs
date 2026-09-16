using UnityEngine;

namespace RunRich3D.Views
{
    public sealed class PooledParticleEffectView : MonoBehaviour
    {
        private ParticleSystem[] _systems;
        private ParticleSystem _rootSystem;
        private float _duration = 1.5f;
        private Transform _cached;

        public float Duration => _duration;

        public void Bind()
        {
            _cached = transform;
            DisableCfxrAutoDestroy();
            _rootSystem = GetComponent<ParticleSystem>();
            _systems = GetComponentsInChildren<ParticleSystem>(true);
            _duration = MeasureDuration();
        }

        public void Play(Vector3 worldPosition)
        {
            if (_cached == null)
            {
                Bind();
            }

            _cached.position = worldPosition;
            _cached.rotation = Quaternion.identity;
            _cached.localScale = Vector3.one;
            Restart();
        }

        public void StopAndClear()
        {
            if (_systems == null)
            {
                return;
            }

            for (int i = 0; i < _systems.Length; i++)
            {
                if (_systems[i] != null)
                {
                    _systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                }
            }
        }

        private void Restart()
        {
            if (_rootSystem != null)
            {
                _rootSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _rootSystem.Play(true);
                return;
            }

            if (_systems == null)
            {
                return;
            }

            for (int i = 0; i < _systems.Length; i++)
            {
                if (_systems[i] == null)
                {
                    continue;
                }

                _systems[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _systems[i].Play(true);
            }
        }

        private float MeasureDuration()
        {
            float longest = 0.5f;
            if (_systems == null)
            {
                return longest;
            }

            for (int i = 0; i < _systems.Length; i++)
            {
                ParticleSystem system = _systems[i];
                if (system == null)
                {
                    continue;
                }

                ParticleSystem.MainModule main = system.main;
                float life = main.startLifetime.constantMax;
                float delay = main.startDelay.constantMax;
                float duration = main.duration + delay + life;
                if (duration > longest)
                {
                    longest = duration;
                }
            }

            return longest;
        }

        private void DisableCfxrAutoDestroy()
        {
            MonoBehaviour[] behaviours = GetComponents<MonoBehaviour>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                MonoBehaviour behaviour = behaviours[i];
                if (behaviour == null || behaviour.GetType().Name != "CFXR_Effect")
                {
                    continue;
                }

                System.Reflection.FieldInfo field = behaviour.GetType().GetField("clearBehavior");
                if (field == null || !field.FieldType.IsEnum)
                {
                    continue;
                }

                field.SetValue(behaviour, System.Enum.ToObject(field.FieldType, 0));
            }
        }
    }
}
