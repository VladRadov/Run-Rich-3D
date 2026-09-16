using UnityEngine;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Input", fileName = "InputSettings")]
    public sealed class InputSettings : ScriptableObject
    {
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] private float _matchWidthOrHeight = 1f;

        public Vector2 ReferenceResolution => _referenceResolution;
        public float MatchWidthOrHeight => _matchWidthOrHeight;
    }
}
