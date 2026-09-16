using UnityEngine;

namespace RunRich3D.Views
{
    public static class OutfitMeshFitter
    {
        public static Renderer Build(Transform node, Mesh mesh, Material material, float targetHeight, Quaternion standUp)
        {
            if (node == null || mesh == null)
            {
                return null;
            }

            StripSkin(node);

            var filter = node.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = node.gameObject.AddComponent<MeshFilter>();
            }

            var renderer = node.GetComponent<MeshRenderer>();
            if (renderer == null)
            {
                renderer = node.gameObject.AddComponent<MeshRenderer>();
            }

            filter.sharedMesh = mesh;
            renderer.sharedMaterial = material;
            Fit(node, mesh, targetHeight, standUp);
            return renderer;
        }

        private static void StripSkin(Transform node)
        {
            var skinned = node.GetComponent<SkinnedMeshRenderer>();
            if (skinned != null)
            {
                Object.DestroyImmediate(skinned);
            }
        }

        private static void Fit(Transform node, Mesh mesh, float targetHeight, Quaternion standUp)
        {
            Bounds rotated = TransformBounds(mesh.bounds, standUp);
            float height = rotated.size.y;
            if (height < 0.01f)
            {
                height = targetHeight;
            }

            float scale = targetHeight / height;
            node.localRotation = standUp;
            node.localScale = Vector3.one * scale;
            node.localPosition = new Vector3(
                -rotated.center.x * scale,
                -rotated.min.y * scale,
                -rotated.center.z * scale);
        }

        private static Bounds TransformBounds(Bounds bounds, Quaternion rotation)
        {
            Vector3 center = rotation * bounds.center;
            Vector3 extents = bounds.extents;
            Vector3 axisX = rotation * Vector3.right;
            Vector3 axisY = rotation * Vector3.up;
            Vector3 axisZ = rotation * Vector3.forward;
            Vector3 abs = new Vector3(
                Mathf.Abs(axisX.x) * extents.x + Mathf.Abs(axisY.x) * extents.y + Mathf.Abs(axisZ.x) * extents.z,
                Mathf.Abs(axisX.y) * extents.x + Mathf.Abs(axisY.y) * extents.y + Mathf.Abs(axisZ.y) * extents.z,
                Mathf.Abs(axisX.z) * extents.x + Mathf.Abs(axisY.z) * extents.y + Mathf.Abs(axisZ.z) * extents.z);
            return new Bounds(center, abs * 2f);
        }
    }
}
