using System;
using Zenject;
using RunRich3D.Services;

namespace RunRich3D.Controllers
{
    public sealed class GameEntryPoint : IInitializable, IDisposable
    {
        private readonly IGameService[] _startup;
        private bool _initialized;

        public GameEntryPoint(
            LightingService lighting,
            InputService input,
            PlayerService player,
            CameraService camera,
            LevelService level,
            PickupEffectService pickupEffects,
            AudioService audio,
            GameLoopService gameLoop)
        {
            _startup = new IGameService[]
            {
                lighting,
                input,
                player,
                camera,
                level,
                pickupEffects,
                audio,
                gameLoop
            };
        }

        public void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            for (int i = 0; i < _startup.Length; i++)
            {
                if (_startup[i] != null)
                {
                    _startup[i].Initialize();
                }
            }

            _initialized = true;
        }

        public void Dispose()
        {
            if (!_initialized)
            {
                return;
            }

            for (int i = _startup.Length - 1; i >= 0; i--)
            {
                _startup[i]?.Dispose();
            }

            _initialized = false;
        }
    }
}
