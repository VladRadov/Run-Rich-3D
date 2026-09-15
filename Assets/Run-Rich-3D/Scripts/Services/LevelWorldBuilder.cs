using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    internal sealed class LevelWorldBuilder
    {
        internal readonly struct Catalog
        {
            internal Catalog(
                GameObject dollarPrefab,
                GameObject bottlePrefab,
                Mesh groundMesh,
                Mesh boxMesh,
                Mesh flagMesh,
                Mesh choiceDoorMesh,
                Mesh partyMesh,
                Mesh studyMesh,
                Mesh finishBlueMesh,
                Mesh finishGreenMesh,
                Mesh finishOrangeMesh,
                Mesh finishYellowMesh,
                Mesh doorPoorMesh,
                Mesh doorRichMesh,
                Mesh doorMillionMesh,
                Material pathMaterial,
                Material moneyMaterial,
                Material bottleMaterial,
                Material flagMaterial,
                Material choiceMaterial,
                Material finishMaterial,
                Material goodDoorMaterial,
                Material poorDoorMaterial,
                Font labelFont)
            {
                DollarPrefab = dollarPrefab;
                BottlePrefab = bottlePrefab;
                GroundMesh = groundMesh;
                BoxMesh = boxMesh;
                FlagMesh = flagMesh;
                ChoiceDoorMesh = choiceDoorMesh;
                PartyMesh = partyMesh;
                StudyMesh = studyMesh;
                FinishBlueMesh = finishBlueMesh;
                FinishGreenMesh = finishGreenMesh;
                FinishOrangeMesh = finishOrangeMesh;
                FinishYellowMesh = finishYellowMesh;
                DoorPoorMesh = doorPoorMesh;
                DoorRichMesh = doorRichMesh;
                DoorMillionMesh = doorMillionMesh;
                PathMaterial = pathMaterial;
                MoneyMaterial = moneyMaterial;
                BottleMaterial = bottleMaterial;
                FlagMaterial = flagMaterial;
                ChoiceMaterial = choiceMaterial;
                FinishMaterial = finishMaterial;
                GoodDoorMaterial = goodDoorMaterial;
                PoorDoorMaterial = poorDoorMaterial;
                LabelFont = labelFont;
            }

            internal GameObject DollarPrefab { get; }
            internal GameObject BottlePrefab { get; }
            internal Mesh GroundMesh { get; }
            internal Mesh BoxMesh { get; }
            internal Mesh FlagMesh { get; }
            internal Mesh ChoiceDoorMesh { get; }
            internal Mesh PartyMesh { get; }
            internal Mesh StudyMesh { get; }
            internal Mesh FinishBlueMesh { get; }
            internal Mesh FinishGreenMesh { get; }
            internal Mesh FinishOrangeMesh { get; }
            internal Mesh FinishYellowMesh { get; }
            internal Mesh DoorPoorMesh { get; }
            internal Mesh DoorRichMesh { get; }
            internal Mesh DoorMillionMesh { get; }
            internal Material PathMaterial { get; }
            internal Material MoneyMaterial { get; }
            internal Material BottleMaterial { get; }
            internal Material FlagMaterial { get; }
            internal Material ChoiceMaterial { get; }
            internal Material FinishMaterial { get; }
            internal Material GoodDoorMaterial { get; }
            internal Material PoorDoorMaterial { get; }
            internal Font LabelFont { get; }
        }

        private readonly Transform _root;
        private readonly Catalog _catalog;
        private Material _goldMaterial;
        private Material _bottleTintMaterial;

        private static readonly Quaternion FaceRunner = Quaternion.identity;
        private const float PathSurfaceY = 0.5f;

        internal LevelWorldBuilder(Transform root, Catalog catalog)
        {
            _root = root;
            _catalog = catalog;
        }

        internal LevelPieceView[] Build(LevelLayout layout)
        {
            ClearRuntimeChildren();
            BuildPath();
            LevelPieceView[] pickupViews = BuildPickups(layout.Pickups);
            BuildObstacles(layout.Obstacles);
            BuildFlags(layout.Flags);
            BuildGate(layout.Gate);
            BuildFinish(layout.Finish);
            return pickupViews;
        }

        private void ClearRuntimeChildren()
        {
            for (int i = _root.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(_root.GetChild(i).gameObject);
            }
        }

        private void BuildPath()
        {
            Transform group = CreateGroup("PathTiles");
            for (int i = 0; i < LevelLayout.PathTileCount; i++)
            {
                float z = LevelLayout.PathTileLength * 0.5f + i * LevelLayout.PathTileLength;
                CreateMeshPiece(
                    "Path_" + i,
                    group,
                    _catalog.GroundMesh,
                    _catalog.PathMaterial,
                    new Vector3(0f, 0f, z),
                    Vector3.one,
                    Quaternion.identity);
            }
        }

        private LevelPieceView[] BuildPickups(PickupSpawn[] spawns)
        {
            Transform group = CreateGroup("Pickups");
            var views = new LevelPieceView[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                PickupSpawn spawn = spawns[i];
                GameObject prefab = spawn.IsPositive ? _catalog.DollarPrefab : _catalog.BottlePrefab;
                Material material = spawn.IsPositive
                    ? ResolvePickupMaterial(true)
                    : ResolvePickupMaterial(false);
                Vector3 scale = spawn.IsPositive ? Vector3.one * 1.4f : Vector3.one * 1.15f;
                views[i] = CreatePrefabOrMesh(
                    spawn.IsPositive ? "Money_" + i : "Bottle_" + i,
                    group,
                    prefab,
                    null,
                    material,
                    new Vector3(spawn.X, PathSurfaceY, spawn.Z),
                    scale);
            }

            return views;
        }

        private void BuildObstacles(ObstacleSpawn[] spawns)
        {
            Transform group = CreateGroup("Obstacles");
            for (int i = 0; i < spawns.Length; i++)
            {
                ObstacleSpawn spawn = spawns[i];
                CreateMeshPiece(
                    "Obstacle_" + i,
                    group,
                    _catalog.BoxMesh,
                    _catalog.PoorDoorMaterial,
                    new Vector3(spawn.X, 0f, spawn.Z),
                    Vector3.one,
                    Quaternion.identity);
            }
        }

        private void BuildFlags(FlagSpawn[] spawns)
        {
            Transform group = CreateGroup("Flags");
            CreateMeshPiece(
                "FlagStrip",
                group,
                _catalog.GroundMesh,
                _catalog.FlagMaterial,
                new Vector3(0f, 0.02f, 21.2f),
                new Vector3(1f, 0.18f, 0.42f),
                Quaternion.identity);

            for (int i = 0; i < spawns.Length; i++)
            {
                FlagSpawn spawn = spawns[i];
                Quaternion rotation = spawn.X < 0f
                    ? Quaternion.Euler(0f, 90f, 0f)
                    : Quaternion.Euler(0f, -90f, 0f);
                CreateMeshPiece(
                    "Flag_" + i,
                    group,
                    _catalog.FlagMesh,
                    _catalog.FlagMaterial,
                    new Vector3(spawn.X, 0f, spawn.Z),
                    Vector3.one * 0.28f,
                    rotation);
            }
        }

        private void BuildGate(GateSpawn gate)
        {
            Transform group = CreateGroup("Gates");
            CreateMeshPiece(
                "ChoiceLeft",
                group,
                _catalog.ChoiceDoorMesh,
                _catalog.ChoiceMaterial,
                new Vector3(-1.45f, 1.55f, gate.Z),
                Vector3.one * 5.2f,
                FaceRunner);
            CreateMeshPiece(
                "ChoiceRight",
                group,
                _catalog.ChoiceDoorMesh,
                _catalog.ChoiceMaterial,
                new Vector3(1.45f, 1.55f, gate.Z),
                Vector3.one * 5.2f,
                FaceRunner);
            CreateMeshPiece(
                "PartyIcon",
                group,
                _catalog.PartyMesh,
                _catalog.GoodDoorMaterial,
                new Vector3(-1.45f, 0.85f, gate.Z),
                Vector3.one * 4.2f,
                FaceRunner);
            CreateMeshPiece(
                "SchoolIcon",
                group,
                _catalog.StudyMesh,
                _catalog.PoorDoorMaterial,
                new Vector3(1.45f, 0.85f, gate.Z),
                Vector3.one * 6.5f,
                FaceRunner);

            CreateLabel("PartyLabel", group, gate.LeftLabel, new Vector3(-1.45f, 2.45f, gate.Z), new Color(0.35f, 0.9f, 0.4f));
            CreateLabel("SchoolLabel", group, gate.RightLabel, new Vector3(1.45f, 2.45f, gate.Z), new Color(1f, 0.45f, 0.3f));
        }

        private void BuildFinish(FinishSpawn finish)
        {
            Transform group = CreateGroup("Finish");
            CreateMeshPiece(
                "FinishPlane",
                group,
                _catalog.GroundMesh,
                _catalog.FinishMaterial,
                new Vector3(0f, 0.03f, finish.Z + 0.6f),
                new Vector3(1f, 0.16f, 0.55f),
                Quaternion.identity);

            PlaceFinishLane("LaneX2", group, _catalog.FinishYellowMesh, _catalog.DoorPoorMesh, _catalog.PoorDoorMaterial, -1.7f, finish.Z, "x2");
            PlaceFinishLane("LaneX3", group, _catalog.FinishOrangeMesh, _catalog.DoorRichMesh, _catalog.GoodDoorMaterial, 0f, finish.Z, "x3");
            PlaceFinishLane("LaneX5", group, _catalog.FinishGreenMesh, _catalog.DoorMillionMesh, _catalog.GoodDoorMaterial, 1.7f, finish.Z, "x5");
        }

        private void PlaceFinishLane(
            string name,
            Transform parent,
            Mesh panelMesh,
            Mesh doorMesh,
            Material doorMaterial,
            float x,
            float z,
            string label)
        {
            Transform lane = CreateGroup(name, parent);
            CreateMeshPiece(
                "Panel",
                lane,
                panelMesh,
                _catalog.FinishMaterial,
                new Vector3(x, 0.9f, z),
                Vector3.one * 3.6f,
                FaceRunner);
            CreateMeshPiece(
                "Door",
                lane,
                doorMesh,
                doorMaterial,
                new Vector3(x, 0f, z + 1.15f),
                Vector3.one,
                FaceRunner);
            CreateLabel("Multiplier", lane, label, new Vector3(x, 2.15f, z), Color.white);
        }

        private Transform CreateGroup(string name)
        {
            return CreateGroup(name, _root);
        }

        private static Transform CreateGroup(string name, Transform parent)
        {
            var group = new GameObject(name);
            group.transform.SetParent(parent, false);
            return group.transform;
        }

        private LevelPieceView CreatePrefabOrMesh(
            string name,
            Transform parent,
            GameObject prefab,
            Mesh fallbackMesh,
            Material material,
            Vector3 position,
            Vector3 scale)
        {
            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab, parent);
                instance.name = name;
                instance.transform.localPosition = position;
                instance.transform.localRotation = Quaternion.identity;
                instance.transform.localScale = scale;
                Paint(instance, material);
                SnapToPath(instance);
                return BindPiece(instance);
            }

            return CreateMeshPiece(name, parent, fallbackMesh, material, position, scale, Quaternion.identity);
        }

        private static LevelPieceView CreateMeshPiece(
            string name,
            Transform parent,
            Mesh mesh,
            Material material,
            Vector3 position,
            Vector3 scale,
            Quaternion rotation)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = rotation;
            go.transform.localScale = scale;

            if (mesh != null)
            {
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;
                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            return BindPiece(go);
        }

        private void CreateLabel(string name, Transform parent, string text, Vector3 position, Color color)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = FaceRunner;
            go.AddComponent<MeshRenderer>();
            go.AddComponent<TextMesh>();
            var view = go.AddComponent<WorldLabelView>();
            view.Bind(text, _catalog.LabelFont, color, 0.06f);
        }

        private static void SnapToPath(GameObject instance)
        {
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return;
            }

            Bounds bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            float delta = PathSurfaceY - bounds.min.y;
            instance.transform.position += new Vector3(0f, delta, 0f);
        }

        private Material ResolvePickupMaterial(bool isMoney)
        {
            if (isMoney)
            {
                if (_goldMaterial == null)
                {
                    _goldMaterial = CreateStandard(new Color(0.93f, 0.74f, 0.12f));
                }

                return _goldMaterial;
            }

            if (_bottleTintMaterial == null)
            {
                _bottleTintMaterial = CreateStandard(new Color(0.18f, 0.52f, 0.22f));
            }

            return _bottleTintMaterial;
        }

        private static Material CreateStandard(Color tint)
        {
            Shader shader = Shader.Find("Standard");
            var material = new Material(shader);
            material.color = tint;
            material.SetFloat("_Glossiness", 0.42f);
            material.SetFloat("_Metallic", 0.15f);
            return material;
        }

        private static LevelPieceView BindPiece(GameObject go)
        {
            var view = EntityViewFactory.CreateOn<LevelPieceView>(go);
            view.Bind();
            return view;
        }

        private static void Paint(GameObject root, Material material)
        {
            if (material == null)
            {
                return;
            }

            MeshRenderer[] renderers = root.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].sharedMaterial = material;
            }
        }
    }
}
