using System;
using System.Collections.Generic;

namespace RunRich3D.Models
{
    internal abstract class ObjectPool<T> : IDisposable where T : class
    {
        private readonly Stack<T> _available = new Stack<T>();
        private readonly HashSet<T> _active = new HashSet<T>();
        private readonly List<T> _created = new List<T>();

        protected abstract T Create();

        protected virtual void OnGet(T item)
        {
        }

        protected virtual void OnRelease(T item)
        {
        }

        protected virtual void OnDispose(T item)
        {
        }

        internal T Get()
        {
            T item = _available.Count > 0 ? _available.Pop() : CreateAndTrack();
            _active.Add(item);
            OnGet(item);
            return item;
        }

        internal void Release(T item)
        {
            if (item == null || !_active.Remove(item))
            {
                return;
            }

            OnRelease(item);
            _available.Push(item);
        }

        internal void ReleaseAll()
        {
            if (_active.Count == 0)
            {
                return;
            }

            var snapshot = new T[_active.Count];
            _active.CopyTo(snapshot);
            for (int i = 0; i < snapshot.Length; i++)
            {
                Release(snapshot[i]);
            }
        }

        public virtual void Dispose()
        {
            ReleaseAll();
            for (int i = 0; i < _created.Count; i++)
            {
                OnDispose(_created[i]);
            }

            _created.Clear();
            _available.Clear();
            _active.Clear();
        }

        private T CreateAndTrack()
        {
            T item = Create();
            _created.Add(item);
            return item;
        }
    }
}
