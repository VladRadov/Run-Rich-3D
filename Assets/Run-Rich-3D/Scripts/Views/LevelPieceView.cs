using UnityEngine;

namespace RunRich3D.Views
{
    public class LevelPieceView : MonoBehaviour
    {
        [Header("Renderer")]
        [SerializeField] private MeshRenderer _renderer;

        protected MeshRenderer CachedRenderer { get; private set; }
        protected Transform CachedTransform { get; private set; }

        public void Bind()
        {
            CachedTransform = transform;
            CachedRenderer = _renderer != null ? _renderer : GetComponent<MeshRenderer>();
        }

        public virtual void SetVisible(bool visible)
        {
            if (CachedTransform == null)
            {
                CachedTransform = transform;
            }

            CachedTransform.gameObject.SetActive(visible);
        }
    }
}
