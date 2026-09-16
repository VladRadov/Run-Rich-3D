using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class StatusBannerFactory
    {
        private readonly PlayerSettings _settings;
        private readonly Font _font;

        public StatusBannerFactory(PlayerSettings settings, Font font)
        {
            _settings = settings;
            _font = font;
        }

        public StatusBannerView Create(Transform playerRoot)
        {
            Transform root = CreateChild(playerRoot, "StatusBanner");
            root.localPosition = _settings.BannerLocalPosition;
            root.localRotation = Quaternion.identity;
            root.localScale = Vector3.one;

            Transform track = CreateCube(root, "Track", _settings.BannerTrackScale);
            Transform fill = CreateCube(root, "Fill", _settings.BannerFillScale);
            TextMesh label = CreateLabel(root, _settings.BannerLabelOffset);

            var trackRenderer = track.GetComponent<MeshRenderer>();
            var fillRenderer = fill.GetComponent<MeshRenderer>();
            Material trackMaterial = CreateTint(_settings.BannerTrackColor, _settings.BannerTrackGlossiness);
            Material fillMaterial = CreateTint(_settings.StatusBarColor(WealthTier.Poor), _settings.BannerTrackGlossiness);
            if (trackRenderer != null)
            {
                trackRenderer.sharedMaterial = trackMaterial;
            }

            var banner = EntityViewFactory.CreateOn<StatusBannerView>(root.gameObject);
            banner.Bind(
                label,
                fill,
                fillRenderer,
                fillMaterial,
                _font,
                _settings.BannerLabelCharacterSize,
                _settings.BannerLabelFontSize,
                _settings.BannerMinFill,
                _settings);
            return banner;
        }

        private static Transform CreateChild(Transform parent, string name)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }

            var created = new GameObject(name);
            created.transform.SetParent(parent, false);
            return created.transform;
        }

        private static Transform CreateCube(Transform parent, string name, Vector3 scale)
        {
            Transform existing = CreateChild(parent, name);
            if (existing.GetComponent<MeshFilter>() == null)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = name;
                cube.transform.SetParent(parent, false);
                Object.Destroy(existing.gameObject);
                existing = cube.transform;
            }

            existing.localPosition = Vector3.zero;
            existing.localRotation = Quaternion.identity;
            existing.localScale = scale;
            var collider = existing.GetComponent<Collider>();
            if (collider != null)
            {
                Object.Destroy(collider);
            }

            return existing;
        }

        private static TextMesh CreateLabel(Transform parent, Vector3 localPosition)
        {
            Transform existing = CreateChild(parent, "Label");
            existing.localPosition = localPosition;
            existing.localRotation = Quaternion.identity;
            existing.localScale = Vector3.one;
            var label = existing.GetComponent<TextMesh>();
            if (label == null)
            {
                label = existing.gameObject.AddComponent<TextMesh>();
            }

            return label;
        }

        private Material CreateTint(Color color, float glossiness)
        {
            string shaderName = _settings != null ? _settings.StandardShaderName : "Standard";
            var material = new Material(Shader.Find(shaderName));
            material.color = color;
            material.SetFloat("_Glossiness", glossiness);
            return material;
        }
    }
}
