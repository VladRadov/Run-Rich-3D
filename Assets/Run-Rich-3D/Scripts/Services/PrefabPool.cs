using System;
using UnityEngine;

namespace RunRich3D.Services
{
    internal sealed class PrefabPool<T> : ComponentPool<T> where T : Component
    {
        private readonly GameObject _prefab;
        private readonly Action<T> _onCreated;

        internal PrefabPool(Transform parent, GameObject prefab, Action<T> onCreated)
            : base(parent)
        {
            _prefab = prefab;
            _onCreated = onCreated;
        }

        protected override T Create()
        {
            GameObject instance = UnityEngine.Object.Instantiate(_prefab, Parent);
            T view = EntityViewFactory.CreateOn<T>(instance);
            if (_onCreated != null)
            {
                _onCreated(view);
            }

            return view;
        }
    }
}
