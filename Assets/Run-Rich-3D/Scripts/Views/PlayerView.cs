using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Views
{
    public sealed class PlayerView : MonoBehaviour
    {
        [Header("Visual")]
        [SerializeField] private Transform _visualRoot;

        private float _spinDuration = 0.48f;

        private Transform _cachedTransform;
        private Transform _spinRoot;
        private GameObject[] _outfits;
        private MeshRenderer[] _outfitRenderers;
        private StatusBannerView _banner;
        private int _activeOutfit = -1;
        private float _spinElapsed;
        private bool _spinning;

        internal Transform MovementRoot
        {
            get
            {
                if (_cachedTransform == null)
                {
                    _cachedTransform = transform;
                }

                return _cachedTransform;
            }
        }

        internal void Bind(Transform visualRoot, float spinDuration)
        {
            _cachedTransform = transform;
            _visualRoot = visualRoot != null ? visualRoot : _cachedTransform;
            _spinDuration = spinDuration > 0f ? spinDuration : 0.48f;
        }

        internal void AttachBanner(StatusBannerView banner)
        {
            _banner = banner;
        }

        internal void BindBannerCamera(Transform camera)
        {
            if (_banner != null)
            {
                _banner.BindCamera(camera);
            }
        }

        internal void BuildOutfits(Mesh[] meshes, Material material, float height, Vector3 standEuler)
        {
            if (_visualRoot == null || meshes == null || meshes.Length == 0)
            {
                return;
            }

            ClearLegacyVisuals();
            RebuildSpinRoot();
            _outfits = new GameObject[meshes.Length];
            _outfitRenderers = new MeshRenderer[meshes.Length];
            Quaternion standUp = Quaternion.Euler(standEuler);
            for (int i = 0; i < meshes.Length; i++)
            {
                if (meshes[i] == null)
                {
                    continue;
                }

                var node = new GameObject(OutfitName(i)).transform;
                node.SetParent(_spinRoot, false);
                OutfitMeshFitter.Build(node, meshes[i], material, height, standUp);
                _outfits[i] = node.gameObject;
                _outfitRenderers[i] = node.GetComponent<MeshRenderer>();
                SetOutfitVisible(i, false);
            }

            _activeOutfit = -1;
            ShowOutfit(FirstAvailableOutfit());
        }

        internal void SetOutfit(int index, bool spin)
        {
            int resolved = ResolveOutfit(index);
            if (resolved < 0)
            {
                return;
            }

            ShowOutfit(resolved);
            if (spin)
            {
                StartSpin();
            }
            else
            {
                CancelSpin();
            }
        }

        internal void SetStatus(WealthTier tier, float normalizedFill)
        {
            if (_banner != null)
            {
                _banner.SetStatus(tier, normalizedFill);
            }
        }

        internal void SetPose(float lateralOffset, float forwardPosition)
        {
            SetPose(new Vector3(lateralOffset, 0f, forwardPosition), 0f);
        }

        internal void SetPose(Vector3 worldPosition, float yawDegrees)
        {
            MovementRoot.position = worldPosition;
            MovementRoot.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
        }

        private void LateUpdate()
        {
            if (!_spinning)
            {
                return;
            }

            Transform spin = _spinRoot != null ? _spinRoot : _visualRoot;
            if (spin == null)
            {
                _spinning = false;
                return;
            }

            _spinElapsed += Time.deltaTime;
            float duration = _spinDuration > 0.01f ? _spinDuration : 0.48f;
            float u = Mathf.Clamp01(_spinElapsed / duration);
            float eased = 1f - (1f - u) * (1f - u);
            spin.localRotation = Quaternion.Euler(0f, 360f * eased, 0f);

            if (u >= 1f)
            {
                spin.localRotation = Quaternion.identity;
                _spinning = false;
            }
        }

        private void OnDestroy()
        {
            CancelSpin();
        }

        private void StartSpin()
        {
            _spinElapsed = 0f;
            _spinning = true;
        }

        private void CancelSpin()
        {
            _spinning = false;
            if (_spinRoot != null)
            {
                _spinRoot.localRotation = Quaternion.identity;
            }
        }

        private int FirstAvailableOutfit()
        {
            if (_outfits == null)
            {
                return -1;
            }

            for (int i = 0; i < _outfits.Length; i++)
            {
                if (_outfits[i] != null)
                {
                    return i;
                }
            }

            return -1;
        }

        private int ResolveOutfit(int index)
        {
            if (_outfits == null || _outfits.Length == 0)
            {
                return -1;
            }

            if (index >= 0 && index < _outfits.Length && _outfits[index] != null)
            {
                return index;
            }

            return FirstAvailableOutfit();
        }

        private void ShowOutfit(int index)
        {
            if (_outfits == null || index < 0)
            {
                return;
            }

            for (int i = 0; i < _outfits.Length; i++)
            {
                SetOutfitVisible(i, i == index);
            }

            _activeOutfit = index;
        }

        private void SetOutfitVisible(int index, bool visible)
        {
            if (_outfits == null || index < 0 || index >= _outfits.Length || _outfits[index] == null)
            {
                return;
            }

            _outfits[index].SetActive(visible);
            if (_outfitRenderers != null && index < _outfitRenderers.Length && _outfitRenderers[index] != null)
            {
                _outfitRenderers[index].enabled = visible;
            }
        }

        private void ClearLegacyVisuals()
        {
            for (int i = _visualRoot.childCount - 1; i >= 0; i--)
            {
                UnityEngine.Object.DestroyImmediate(_visualRoot.GetChild(i).gameObject);
            }
        }

        private void RebuildSpinRoot()
        {
            _spinRoot = new GameObject("SpinRoot").transform;
            _spinRoot.SetParent(_visualRoot, false);
            _spinRoot.localPosition = Vector3.zero;
            _spinRoot.localRotation = Quaternion.identity;
            _spinRoot.localScale = Vector3.one;
        }

        private static string OutfitName(int index)
        {
            switch (index)
            {
                case CowboyOutfits.Poor:
                    return "Cowboy_Poor";
                case CowboyOutfits.Middle:
                    return "Cowboy_Middle";
                case CowboyOutfits.Rich:
                    return "Cowboy_Rich";
                case CowboyOutfits.Millionaire:
                    return "Cowboy_Millionaire";
                default:
                    return "cowboy_Casual";
            }
        }
    }
}
