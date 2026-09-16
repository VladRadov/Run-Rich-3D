using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Views
{
    public sealed class PlayerView : MonoBehaviour
    {
        private static readonly int IsWalkingId = Animator.StringToHash("IsWalking");

        [Header("Visual")]
        [SerializeField] private Transform _visualRoot;

        private float _spinDuration = 0.48f;

        private Transform _cachedTransform;
        private Transform _spinRoot;
        private Transform _rigRoot;
        private Animator _animator;
        private float _outfitHeight = 1.85f;
        private GameObject[] _outfits;
        private Renderer[] _outfitRenderers;
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

        internal void BindPlayerSkins(RuntimeAnimatorController animatorController, float outfitHeight)
        {
            if (_visualRoot == null)
            {
                return;
            }

            _outfitHeight = outfitHeight > 0.1f ? outfitHeight : 1.85f;
            ClearGeneratedVisuals();
            EnsureSpinRoot();
            _rigRoot = FindPlayerRig();
            if (_rigRoot == null)
            {
                return;
            }

            _rigRoot.SetParent(_spinRoot, false);
            _rigRoot.localPosition = Vector3.zero;
            _rigRoot.localRotation = Quaternion.identity;
            _rigRoot.localScale = Vector3.one;
            _rigRoot.gameObject.SetActive(true);
            BindOutfitRenderers(_rigRoot);
            HideEndLevelSkins(_rigRoot);
            SetupAnimator(animatorController);
            _activeOutfit = -1;
            ShowOutfit(FirstAvailableOutfit());
            SetWalking(false);
            AlignRigToSurface();
        }

        internal void AlignToSurface()
        {
            AlignRigToSurface();
        }

        internal void SetWalking(bool walking)
        {
            if (_animator == null)
            {
                return;
            }

            _animator.SetBool(IsWalkingId, walking);
        }

        internal void SetOutfit(int index, bool spin)
        {
            int resolved = ResolveOutfit(index);
            if (resolved < 0)
            {
                return;
            }

            ShowOutfit(resolved);
            AlignRigToSurface();
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
            SetPose(lateralOffset, forwardPosition, 0f);
        }

        internal void SetPose(float lateralOffset, float forwardPosition, float steerYaw)
        {
            SetPose(new Vector3(lateralOffset, 0f, forwardPosition), 0f, steerYaw);
        }

        internal void SetPose(Vector3 worldPosition, float yawDegrees)
        {
            SetPose(worldPosition, yawDegrees, 0f);
        }

        internal void SetPose(Vector3 worldPosition, float yawDegrees, float steerYaw)
        {
            MovementRoot.position = worldPosition;
            MovementRoot.rotation = Quaternion.Euler(0f, yawDegrees, 0f);
            ApplySteerTilt(steerYaw);
        }

        private void ApplySteerTilt(float steerYaw)
        {
            if (_visualRoot == null)
            {
                return;
            }

            _visualRoot.localRotation = Quaternion.Euler(0f, steerYaw, -steerYaw * 0.38f);
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

        private void BindOutfitRenderers(Transform rig)
        {
            string[] names = PlayerOutfits.MeshNames;
            _outfits = new GameObject[names.Length];
            _outfitRenderers = new Renderer[names.Length];
            Renderer[] renderers = rig.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < names.Length; i++)
            {
                Renderer found = FindRendererByName(renderers, names[i]);
                if (found == null)
                {
                    continue;
                }

                _outfitRenderers[i] = found;
                _outfits[i] = found.gameObject;
            }
        }

        private static void HideEndLevelSkins(Transform rig)
        {
            Renderer[] renderers = rig.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                string name = renderers[i].gameObject.name;
                if (name.IndexOf("EndLevel", System.StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    renderers[i].enabled = false;
                    renderers[i].gameObject.SetActive(false);
                }
            }
        }

        private static Renderer FindRendererByName(Renderer[] renderers, string name)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                if (string.Equals(renderers[i].gameObject.name, name, System.StringComparison.OrdinalIgnoreCase))
                {
                    return renderers[i];
                }
            }

            return null;
        }

        private Transform FindPlayerRig()
        {
            Transform named = FindNamed(_visualRoot, "player");
            if (named != null)
            {
                return named;
            }

            SkinnedMeshRenderer skin = _visualRoot.GetComponentInChildren<SkinnedMeshRenderer>(true);
            return skin != null ? FindRigRoot(skin.transform) : null;
        }

        private static Transform FindRigRoot(Transform from)
        {
            Transform current = from;
            while (current.parent != null && current.parent.name != "Visual" && current.parent.name != "SpinRoot")
            {
                current = current.parent;
            }

            return current;
        }

        private void SetupAnimator(RuntimeAnimatorController controller)
        {
            if (_rigRoot == null || controller == null)
            {
                _animator = null;
                return;
            }

            _animator = _rigRoot.GetComponent<Animator>();
            if (_animator == null)
            {
                _animator = _rigRoot.gameObject.AddComponent<Animator>();
            }

            Animator[] nested = _rigRoot.GetComponentsInChildren<Animator>(true);
            for (int i = 0; i < nested.Length; i++)
            {
                if (nested[i] != null && nested[i] != _animator)
                {
                    nested[i].enabled = false;
                }
            }

            Avatar avatar = FindAvatarOnRig(_rigRoot);
            if (avatar != null)
            {
                _animator.avatar = avatar;
            }

            _animator.runtimeAnimatorController = controller;
            _animator.applyRootMotion = false;
            _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            _animator.enabled = true;
            _animator.Rebind();
            _animator.Play("Idle", 0, 0f);
            _animator.SetBool(IsWalkingId, false);
        }

        private static Avatar FindAvatarOnRig(Transform rigRoot)
        {
            Animator[] animators = rigRoot.GetComponentsInChildren<Animator>(true);
            for (int i = 0; i < animators.Length; i++)
            {
                if (animators[i] != null && animators[i].avatar != null)
                {
                    return animators[i].avatar;
                }
            }

            return null;
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

            int nearest = -1;
            int nearestDelta = int.MaxValue;
            for (int i = 0; i < _outfits.Length; i++)
            {
                if (_outfits[i] == null)
                {
                    continue;
                }

                int delta = Mathf.Abs(i - index);
                if (delta < nearestDelta)
                {
                    nearestDelta = delta;
                    nearest = i;
                }
            }

            return nearest >= 0 ? nearest : FirstAvailableOutfit();
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

        private void AlignRigToSurface()
        {
            if (_rigRoot == null || _spinRoot == null)
            {
                return;
            }

            Renderer sample = ActiveOutfitRenderer();
            if (sample == null)
            {
                return;
            }

            _rigRoot.localScale = Vector3.one;
            Vector3 local = _rigRoot.localPosition;
            _rigRoot.localPosition = new Vector3(local.x, 0f, local.z);

            if (_animator != null && _animator.enabled)
            {
                _animator.Update(0f);
            }

            Bounds bounds = sample.bounds;
            float height = bounds.size.y;
            if (height < 0.01f)
            {
                return;
            }

            float scale = _outfitHeight / height;
            _rigRoot.localScale = Vector3.one * scale;
            if (_animator != null && _animator.enabled)
            {
                _animator.Update(0f);
            }

            bounds = sample.bounds;
            float surfaceY = _spinRoot.position.y;
            float feetOffset = bounds.min.y - surfaceY;
            _rigRoot.localPosition = new Vector3(local.x, -feetOffset, local.z);
        }

        private Renderer ActiveOutfitRenderer()
        {
            if (_outfitRenderers == null)
            {
                return null;
            }

            if (_activeOutfit >= 0 && _activeOutfit < _outfitRenderers.Length && _outfitRenderers[_activeOutfit] != null)
            {
                return _outfitRenderers[_activeOutfit];
            }

            for (int i = 0; i < _outfitRenderers.Length; i++)
            {
                if (_outfitRenderers[i] != null && _outfitRenderers[i].enabled)
                {
                    return _outfitRenderers[i];
                }
            }

            return null;
        }

        private void EnsureSpinRoot()
        {
            if (_spinRoot != null)
            {
                return;
            }

            Transform existing = _visualRoot.Find("SpinRoot");
            if (existing != null)
            {
                _spinRoot = existing;
                return;
            }

            _spinRoot = new GameObject("SpinRoot").transform;
            _spinRoot.SetParent(_visualRoot, false);
            _spinRoot.localPosition = Vector3.zero;
            _spinRoot.localRotation = Quaternion.identity;
            _spinRoot.localScale = Vector3.one;
        }

        private void ClearGeneratedVisuals()
        {
            for (int i = _visualRoot.childCount - 1; i >= 0; i--)
            {
                Transform child = _visualRoot.GetChild(i);
                if (child.name == "CowboyRig" || child.name.StartsWith("Cowboy_"))
                {
                    if (Application.isPlaying)
                    {
                        Object.Destroy(child.gameObject);
                    }
                    else
                    {
                        Object.DestroyImmediate(child.gameObject);
                    }
                }
            }
        }

        private static Transform FindNamed(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform found = FindNamed(root.GetChild(i), name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }
    }
}
