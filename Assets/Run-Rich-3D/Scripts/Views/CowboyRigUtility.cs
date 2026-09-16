using System.Collections.Generic;
using UnityEngine;

namespace RunRich3D.Views
{
    public static class CowboyRigUtility
    {
        // Ordered bone-name hashes baked into all Cowboy_*.asset meshes.
        private static readonly int[] CowboyBoneHashes =
        {
            unchecked((int)1814948745u),
            unchecked((int)3544756423u),
            unchecked((int)124217959u),
            unchecked((int)198118164u),
            unchecked((int)3363769422u),
            unchecked((int)1139793341u),
            unchecked((int)3862004990u),
            unchecked((int)4201768506u),
            unchecked((int)4146078101u),
            unchecked((int)1523800894u),
            unchecked((int)2509321217u),
            unchecked((int)33052115u),
            unchecked((int)304759308u),
            unchecked((int)456007731u),
            unchecked((int)3377282459u),
            unchecked((int)1353527623u),
            unchecked((int)993105451u),
            unchecked((int)1958296944u),
            unchecked((int)4048052966u),
            unchecked((int)819288853u),
            unchecked((int)3020740934u),
            unchecked((int)726889649u),
            unchecked((int)1201915780u),
            unchecked((int)907975319u),
            unchecked((int)3810468751u),
            unchecked((int)1538594860u),
            unchecked((int)1139265638u),
            unchecked((int)3183680222u),
            unchecked((int)367639170u),
            unchecked((int)371613643u),
            unchecked((int)1208352096u),
            unchecked((int)188935247u),
            unchecked((int)2658684444u),
            unchecked((int)927011318u),
            unchecked((int)300174519u),
            unchecked((int)2593074513u),
            unchecked((int)3199398146u),
            unchecked((int)2931060149u),
            unchecked((int)2271366537u),
            unchecked((int)882077298u),
            unchecked((int)2079976957u),
            unchecked((int)2834837122u),
            unchecked((int)2252359767u),
            unchecked((int)2445570048u),
            unchecked((int)783101196u),
            unchecked((int)1756634114u),
            unchecked((int)3463139838u),
            unchecked((int)1331026266u),
            unchecked((int)1033736893u),
            unchecked((int)2459479772u),
            unchecked((int)207841958u),
            unchecked((int)1293532111u),
            unchecked((int)761984867u),
            unchecked((int)3739607149u),
            unchecked((int)2977736641u),
            unchecked((int)1817549059u),
            unchecked((int)3214512426u),
            unchecked((int)1687653551u),
            unchecked((int)3839196625u),
            unchecked((int)2721678721u),
            unchecked((int)1994139395u),
            unchecked((int)615129746u),
            unchecked((int)4228603681u),
            unchecked((int)317692549u),
            unchecked((int)776202399u),
            unchecked((int)2121122790u),
            unchecked((int)1126126988u),
            unchecked((int)32448124u),
            unchecked((int)2756234097u),
            unchecked((int)85733673u),
            unchecked((int)3780534848u),
            unchecked((int)2833096486u),
            unchecked((int)748179792u),
            unchecked((int)2852221850u),
            unchecked((int)28713038u),
            unchecked((int)3155888341u),
            unchecked((int)3457270866u),
            unchecked((int)4064681957u)
        };

        public static SkinnedMeshRenderer FindSourceSkin(Transform rigRoot)
        {
            if (rigRoot == null)
            {
                return null;
            }

            SkinnedMeshRenderer best = null;
            SkinnedMeshRenderer[] skins = rigRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            for (int i = 0; i < skins.Length; i++)
            {
                Transform[] bones = skins[i].bones;
                if (bones == null || bones.Length == 0)
                {
                    continue;
                }

                if (best == null || bones.Length > best.bones.Length)
                {
                    best = skins[i];
                }
            }

            return best;
        }

        public static Transform[] CaptureSkeletonBones(Transform rigRoot)
        {
            SkinnedMeshRenderer source = FindSourceSkin(rigRoot);
            if (source != null && source.bones != null && source.bones.Length > 0)
            {
                return (Transform[])source.bones.Clone();
            }

            Transform hips = ResolveRootBone(rigRoot, null);
            if (hips == null)
            {
                return null;
            }

            var bones = new List<Transform>(96);
            CollectMixamoBones(hips, bones);
            return bones.Count > 0 ? bones.ToArray() : null;
        }

        public static Dictionary<int, Transform> BuildBoneLookup(Transform rigRoot)
        {
            var map = new Dictionary<int, Transform>(160);
            if (rigRoot == null)
            {
                return map;
            }

            Transform[] transforms = rigRoot.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                AddBoneAliases(map, transforms[i]);
            }

            return map;
        }

        public static Transform[] MapMeshBones(
            Mesh mesh,
            Dictionary<int, Transform> boneLookup,
            Transform[] skeletonBones,
            Transform fallback)
        {
            if (mesh == null || mesh.bindposeCount <= 0)
            {
                return null;
            }

            int count = mesh.bindposeCount;

            Transform[] byHash = MapByEmbeddedHashes(boneLookup, fallback, count);
            if (byHash != null)
            {
                return byHash;
            }

            if (skeletonBones != null && skeletonBones.Length == count)
            {
                return (Transform[])skeletonBones.Clone();
            }

            if (skeletonBones != null && skeletonBones.Length > 0)
            {
                return FitBoneArray(skeletonBones, count, fallback);
            }

            if (fallback != null)
            {
                var hipsOnly = new Transform[count];
                for (int i = 0; i < count; i++)
                {
                    hipsOnly[i] = fallback;
                }

                return hipsOnly;
            }

            return null;
        }

        public static Transform ResolveRootBone(Transform rigRoot, Transform[] skeletonBones)
        {
            Transform hips = FindNamed(rigRoot, "mixamorig:Hips");
            if (hips != null)
            {
                return hips;
            }

            hips = FindNamed(rigRoot, "Hips");
            if (hips != null)
            {
                return hips;
            }

            if (skeletonBones != null)
            {
                for (int i = 0; i < skeletonBones.Length; i++)
                {
                    if (skeletonBones[i] != null)
                    {
                        return skeletonBones[i];
                    }
                }
            }

            return rigRoot;
        }

        public static void HideEmbeddedMeshes(Transform rigRoot)
        {
            HideEmbeddedMeshesExcept(rigRoot, null);
        }

        public static void HideEmbeddedMeshesExcept(Transform rigRoot, SkinnedMeshRenderer keep)
        {
            if (rigRoot == null)
            {
                return;
            }

            Renderer[] renderers = rigRoot.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                if (keep != null && renderers[i] == keep)
                {
                    continue;
                }

                if (!IsOutfitName(renderers[i].gameObject.name))
                {
                    renderers[i].enabled = false;
                }
            }
        }

        public static void FitRigToHeight(Transform scaleRoot, Renderer sample, float targetHeight)
        {
            if (scaleRoot == null || sample == null || targetHeight < 0.01f)
            {
                return;
            }

            Bounds bounds = sample.bounds;
            if (bounds.size.y < 0.05f || bounds.size.y > 50f)
            {
                return;
            }

            float scale = targetHeight / bounds.size.y;
            scaleRoot.localScale = Vector3.one * scale;

            bounds = sample.bounds;
            if (scaleRoot.parent == null)
            {
                return;
            }

            float feetOffset = bounds.min.y - scaleRoot.parent.position.y;
            scaleRoot.localPosition = new Vector3(0f, scaleRoot.localPosition.y - feetOffset, 0f);
        }

        private static Transform[] MapByEmbeddedHashes(
            Dictionary<int, Transform> boneLookup,
            Transform fallback,
            int bindPoseCount)
        {
            if (boneLookup == null || boneLookup.Count == 0)
            {
                return null;
            }

            if (bindPoseCount != CowboyBoneHashes.Length)
            {
                return null;
            }

            var mapped = new Transform[CowboyBoneHashes.Length];
            int matched = 0;
            for (int i = 0; i < CowboyBoneHashes.Length; i++)
            {
                if (boneLookup.TryGetValue(CowboyBoneHashes[i], out Transform bone) && bone != null)
                {
                    mapped[i] = bone;
                    matched++;
                }
                else
                {
                    mapped[i] = fallback;
                }
            }

            // Prefer named matches; otherwise order/pad still keeps a skinned deforming mesh.
            return matched > 0 ? mapped : null;
        }

        private static Transform[] FitBoneArray(Transform[] source, int count, Transform fallback)
        {
            var mapped = new Transform[count];
            for (int i = 0; i < count; i++)
            {
                if (i < source.Length && source[i] != null)
                {
                    mapped[i] = source[i];
                }
                else
                {
                    mapped[i] = fallback != null ? fallback : source[0];
                }
            }

            return mapped;
        }

        private static void CollectMixamoBones(Transform node, List<Transform> bones)
        {
            if (node == null)
            {
                return;
            }

            if (IsLikelyDeformBone(node.name))
            {
                bones.Add(node);
            }

            for (int i = 0; i < node.childCount; i++)
            {
                CollectMixamoBones(node.GetChild(i), bones);
            }
        }

        private static bool IsLikelyDeformBone(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return false;
            }

            if (name.IndexOf("mixamorig", System.StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return !name.EndsWith("_end", System.StringComparison.Ordinal)
                       || name.IndexOf("Toe", System.StringComparison.OrdinalIgnoreCase) >= 0;
            }

            return name == "Hips" || name == "Spine" || name == "Head";
        }

        private static void AddBoneAliases(Dictionary<int, Transform> map, Transform bone)
        {
            if (bone == null)
            {
                return;
            }

            map[Animator.StringToHash(bone.name)] = bone;

            string name = bone.name;
            int colon = name.LastIndexOf(':');
            if (colon >= 0 && colon + 1 < name.Length)
            {
                map[Animator.StringToHash(name.Substring(colon + 1))] = bone;
            }

            const string mixamo = "mixamorig:";
            if (name.StartsWith(mixamo, System.StringComparison.Ordinal))
            {
                map[Animator.StringToHash(name.Substring(mixamo.Length))] = bone;
            }
            else if (name.IndexOf(':') < 0)
            {
                map[Animator.StringToHash(mixamo + name)] = bone;
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

        private static bool IsOutfitName(string name)
        {
            return name == "Cowboy_Poor"
                   || name == "Cowboy_Middle"
                   || name == "Cowboy_Rich"
                   || name == "Cowboy_Millionaire"
                   || name == "cowboy_Casual";
        }
    }
}
