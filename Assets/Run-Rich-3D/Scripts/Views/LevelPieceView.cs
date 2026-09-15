using UnityEngine;

namespace RunRich3D.Views
{
    public class LevelPieceView : MonoBehaviour
    {
        [Header("Renderer")]
        [SerializeField] private MeshRenderer _renderer;

        protected MeshRenderer CachedRenderer { get; private set; }
        protected Transform CachedTransform { get; private set; }

        internal void Bind()
        {
            CachedTransform = transform;
            CachedRenderer = _renderer != null ? _renderer : GetComponent<MeshRenderer>();
        }

        internal virtual void SetVisible(bool visible)
        {
            if (CachedTransform == null)
            {
                CachedTransform = transform;
            }

            CachedTransform.gameObject.SetActive(visible);
        }
    }
}
