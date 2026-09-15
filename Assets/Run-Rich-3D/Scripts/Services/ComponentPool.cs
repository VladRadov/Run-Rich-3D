using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Services
{
    internal abstract class ComponentPool<T> : ObjectPool<T> where T : Component
    {
        private readonly Transform _parent;

        protected ComponentPool(Transform parent)
        {
            _parent = parent;
        }

        protected Transform Parent => _parent;

        protected override void OnGet(T item)
        {
            if (item != null)
            {
                item.gameObject.SetActive(true);
            }
        }

        protected override void OnRelease(T item)
        {
            if (item == null)
            {
                return;
            }

            item.gameObject.SetActive(false);
            if (_parent != null)
            {
                item.transform.SetParent(_parent, false);
            }
        }

        protected override void OnDispose(T item)
        {
            if (item != null)
            {
                Object.Destroy(item.gameObject);
            }
        }
    }
}
