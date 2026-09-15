using UnityEngine;

namespace RunRich3D.Services
{
    internal static class EntityViewFactory
    {
        internal static T CreateOn<T>(GameObject entity) where T : Component
        {
            var view = entity.GetComponent<T>();
            if (view == null)
            {
                view = entity.AddComponent<T>();
            }

            return view;
        }
    }
}
