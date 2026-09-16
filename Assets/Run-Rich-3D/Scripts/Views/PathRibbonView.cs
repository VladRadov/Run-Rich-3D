using UnityEngine;

namespace RunRich3D.Views
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public sealed class PathRibbonView : MonoBehaviour
    {
        [SerializeField] private float _radius = 14f;
        [SerializeField] private float _angleDegrees = 90f;
        [SerializeField] private bool _turnRight = true;
        [SerializeField] private int _segments = 18;
        [SerializeField] private float _halfWidth = 3f;
        [SerializeField] private float _overlap = 0.55f;
        [SerializeField] private Material _material;

        private Mesh _mesh;
        private MeshFilter _filter;
        private MeshRenderer _meshRenderer;

        public void Bind(
            float radius,
            float angleDegrees,
            bool turnRight,
            int segments,
            float halfWidth,
            float overlap,
            Material material)
        {
            _radius = radius;
            _angleDegrees = angleDegrees;
            _turnRight = turnRight;
            _segments = segments;
            _halfWidth = halfWidth;
            _overlap = overlap;
            _material = material;
            Rebuild();
        }

        private void OnEnable()
        {
            Rebuild();
        }

        private void OnValidate()
        {
            Rebuild();
        }

        private void OnDisable()
        {
            ReleaseMesh();
        }

        private void Rebuild()
        {
            if (_filter == null)
            {
                _filter = GetComponent<MeshFilter>();
            }

            if (_meshRenderer == null)
            {
                _meshRenderer = GetComponent<MeshRenderer>();
            }

            if (_filter == null || _meshRenderer == null)
            {
                return;
            }

            if (_mesh == null)
            {
                _mesh = new Mesh();
                _mesh.name = "PathRibbon";
                _mesh.hideFlags = HideFlags.DontSave;
            }

            BuildRibbon(_mesh, _radius, _angleDegrees, _turnRight, _halfWidth, _segments, _overlap);
            _filter.sharedMesh = _mesh;
            if (_material != null)
            {
                _meshRenderer.sharedMaterial = _material;
            }

            _meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void ReleaseMesh()
        {
            if (_mesh == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(_mesh);
            }
            else
            {
                DestroyImmediate(_mesh);
            }

            _mesh = null;
        }

        public static void BuildRibbon(
            Mesh mesh,
            float radius,
            float angleDegrees,
            bool turnRight,
            float halfWidth,
            int segments,
            float overlap)
        {
            float safeRadius = radius < 1f ? 1f : radius;
            float absDegrees = angleDegrees < 1f ? 90f : angleDegrees;
            float absAngle = absDegrees * Mathf.Deg2Rad;
            float sign = turnRight ? 1f : -1f;
            if (overlap < 0f)
            {
                overlap = 0f;
            }

            int arcRings = (segments > 4 ? segments : 18) + 1;
            int rings = arcRings + 2;
            int arcSegments = arcRings - 1;
            var vertices = new Vector3[rings * 2];
            var uvs = new Vector2[rings * 2];
            var triangles = new int[(rings - 1) * 6];
            float uvLength = safeRadius * absAngle;
            for (int i = 0; i < rings; i++)
            {
                float cx;
                float cz;
                float rightX;
                float rightZ;
                float v;
                if (i == 0)
                {
                    cx = 0f;
                    cz = -overlap;
                    rightX = 1f;
                    rightZ = 0f;
                    v = 0f;
                }
                else if (i == rings - 1)
                {
                    float theta = absAngle;
                    float ecx = sign * safeRadius * (1f - Mathf.Cos(theta));
                    float ecz = safeRadius * Mathf.Sin(theta);
                    float fx = sign * Mathf.Sin(theta);
                    float fz = Mathf.Cos(theta);
                    cx = ecx + fx * overlap;
                    cz = ecz + fz * overlap;
                    rightX = Mathf.Cos(theta);
                    rightZ = -sign * Mathf.Sin(theta);
                    v = (overlap + uvLength + overlap) / 7.5f;
                }
                else
                {
                    float t = (i - 1) / (float)arcSegments;
                    float theta = t * absAngle;
                    cx = sign * safeRadius * (1f - Mathf.Cos(theta));
                    cz = safeRadius * Mathf.Sin(theta);
                    rightX = Mathf.Cos(theta);
                    rightZ = -sign * Mathf.Sin(theta);
                    v = (overlap + t * uvLength) / 7.5f;
                }

                vertices[i * 2] = new Vector3(cx - rightX * halfWidth, 0f, cz - rightZ * halfWidth);
                vertices[i * 2 + 1] = new Vector3(cx + rightX * halfWidth, 0f, cz + rightZ * halfWidth);
                uvs[i * 2] = new Vector2(0f, v);
                uvs[i * 2 + 1] = new Vector2(1f, v);
            }

            int tri = 0;
            for (int i = 0; i < rings - 1; i++)
            {
                int a = i * 2;
                int b = a + 1;
                int c = a + 2;
                int d = a + 3;
                triangles[tri++] = a;
                triangles[tri++] = c;
                triangles[tri++] = b;
                triangles[tri++] = b;
                triangles[tri++] = c;
                triangles[tri++] = d;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        }
    }
}
