using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class PickupEffectService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private LevelService _levelService;

        [Header("Prefabs")]
        [SerializeField] private GameObject _dollarsEffectPrefab;
        [SerializeField] private GameObject _bottleEffectPrefab;

        private PrefabPool<PooledParticleEffectView> _gainPool;
        private PrefabPool<PooledParticleEffectView> _lossPool;
        private PickupEffectController _controller;

        public void Initialize()
        {
            if (_playerService == null || _playerService.View == null || _levelService == null)
            {
                return;
            }

            Transform poolParent = EnsurePoolRoot(_playerService.View.MovementRoot);
            _gainPool = CreatePool(poolParent, _dollarsEffectPrefab);
            _lossPool = CreatePool(poolParent, _bottleEffectPrefab);
            _controller = new PickupEffectController(
                _gainPool,
                _lossPool,
                _playerService.View,
                _levelService.Events,
                _playerService.Model);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
            _gainPool?.Dispose();
            _gainPool = null;
            _lossPool?.Dispose();
            _lossPool = null;
        }

        private static PrefabPool<PooledParticleEffectView> CreatePool(Transform parent, GameObject prefab)
        {
            if (prefab == null)
            {
                return null;
            }

            return new PrefabPool<PooledParticleEffectView>(parent, prefab, view => view.Bind());
        }

        private static Transform EnsurePoolRoot(Transform playerRoot)
        {
            Transform existing = playerRoot.Find("VfxPool");
            if (existing != null)
            {
                return existing;
            }

            var root = new GameObject("VfxPool").transform;
            root.SetParent(playerRoot, false);
            root.localPosition = Vector3.zero;
            root.localRotation = Quaternion.identity;
            root.localScale = Vector3.one;
            return root;
        }
    }
}
