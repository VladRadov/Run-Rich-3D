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
                Mesh finishPlaneMesh,
                Mesh finishStarMesh,
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
                FinishPlaneMesh = finishPlaneMesh;
                FinishStarMesh = finishStarMesh;
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
            internal Mesh FinishPlaneMesh { get; }
            internal Mesh FinishStarMesh { get; }
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

        internal readonly struct BuiltLevel
        {
            internal BuiltLevel(LevelPieceView[] pickups, FlagView[] flags)
            {
                Pickups = pickups;
                Flags = flags;
            }

            internal LevelPieceView[] Pickups { get; }
            internal FlagView[] Flags { get; }
        }

        private readonly Transform _root;
        private readonly Catalog _catalog;
        private readonly LevelWorldTuning _tuning;
        private readonly PathBend _path;
        private readonly PickupPools _pickupPools;
        private Material _goldMaterial;
        private Material _bottleTintMaterial;

        private static readonly Quaternion FaceRunner = Quaternion.identity;

        internal LevelWorldBuilder(Transform root, Catalog catalog, LevelWorldTuning tuning, PathBend path)
            : this(root, catalog, tuning, path, null)
        {
        }

        internal LevelWorldBuilder(
            Transform root,
            Catalog catalog,
            LevelWorldTuning tuning,
            PathBend path,
            PickupPools pickupPools)
        {
            _root = root;
            _catalog = catalog;
            _tuning = tuning;
            _path = path ?? new PathBend(PathSegment.DefaultCourse(), 0f);
            _pickupPools = pickupPools;
        }

        internal BuiltLevel BuildRuntimePickups(LevelLayout layout)
        {
            if (_pickupPools != null)
            {
                _pickupPools.ReleaseAll();
            }
            else
            {
                ClearRuntimeChildren();
            }

            return new BuiltLevel(CreatePickupViews(_root, layout.Pickups), new FlagView[0]);
        }

        internal FlagView[] BakeStatic(LevelLayout layout)
        {
            BuildPath(layout);
            BuildObstacles(layout.Obstacles);
            FlagView[] flagViews = BuildFlags(layout.Flags);
            BuildGate(layout.Gate);
            BuildFinish(layout.Finish);
            return flagViews;
        }

        private void ClearRuntimeChildren()
        {
            for (int i = _root.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(_root.GetChild(i).gameObject);
            }
        }

        private void BuildPath(LevelLayout layout)
        {
            Transform group = CreateGroup("PathTiles");
            float tileLength = layout.PathTileLength > 0.01f ? layout.PathTileLength : 7.5f;
            PathBend.CompiledPiece[] pieces = _path.Pieces;
            for (int p = 0; p < pieces.Length; p++)
            {
                PathBend.CompiledPiece piece = pieces[p];
                if (piece.IsTurn)
                {
                    continue;
                }

                float total = piece.Length;
                if (p == pieces.Length - 1)
                {
                    total += tileLength;
                }

                float covered = 0f;
                int tileIndex = 0;
                while (covered < total - 0.02f)
                {
                    float remaining = total - covered;
                    float thisLen = remaining < tileLength ? remaining : tileLength;
                    if (thisLen < 0.05f)
                    {
                        break;
                    }

                    float mid = piece.StartDistance + covered + thisLen * 0.5f;
                    PlaceMeshOnPath(
                        "Path_" + p + "_" + tileIndex,
                        group,
                        _catalog.GroundMesh,
                        _catalog.PathMaterial,
                        0f,
                        mid,
                        0f,
                        new Vector3(1f, 1f, thisLen / tileLength),
                        Quaternion.identity);
                    covered += thisLen;
                    tileIndex++;
                }
            }

            BuildBend(group);
        }

        private void BuildBend(Transform group)
        {
            if (_catalog.GroundMesh == null)
            {
                return;
            }

            int segments = _tuning.BendSegments > 4 ? _tuning.BendSegments : 18;
            float halfWidth = _tuning.BendHalfWidth > 0.1f ? _tuning.BendHalfWidth : 3f;
            float surfaceY = _catalog.GroundMesh != null
                ? _catalog.GroundMesh.bounds.max.y
                : (_tuning.BendSurfaceY > 0.01f ? _tuning.BendSurfaceY : 0.5f);
            PathBend.CompiledPiece[] pieces = _path.Pieces;
            for (int i = 0; i < pieces.Length; i++)
            {
                PathBend.CompiledPiece piece = pieces[i];
                if (!piece.IsTurn || piece.Radius < 0.01f)
                {
                    continue;
                }

                float absAngle = piece.SignedAngle < 0f ? -piece.SignedAngle : piece.SignedAngle;
                if (absAngle <= 0.0001f)
                {
                    continue;
                }

                var go = new GameObject("PathBend_" + i);
                go.transform.SetParent(group, false);
                go.transform.localPosition = new Vector3(piece.StartX, surfaceY, piece.StartZ);
                go.transform.localRotation = Quaternion.Euler(0f, piece.StartHeading * Mathf.Rad2Deg, 0f);
                go.transform.localScale = Vector3.one;
                if (go.GetComponent<MeshFilter>() == null)
                {
                    go.AddComponent<MeshFilter>();
                }

                var renderer = go.GetComponent<MeshRenderer>();
                if (renderer == null)
                {
                    renderer = go.AddComponent<MeshRenderer>();
                }

                renderer.sharedMaterial = _catalog.PathMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                var ribbon = go.GetComponent<PathRibbonView>();
                if (ribbon == null)
                {
                    ribbon = go.AddComponent<PathRibbonView>();
                }

                ribbon.Bind(
                    piece.Radius,
                    absAngle * Mathf.Rad2Deg,
                    piece.Sign >= 0f,
                    segments,
                    halfWidth,
                    0.55f,
                    _catalog.PathMaterial);
            }
        }

        private LevelPieceView[] CreatePickupViews(Transform group, PickupSpawn[] spawns)
        {
            var views = new LevelPieceView[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                PickupSpawn spawn = spawns[i];
                GameObject prefab = spawn.IsPositive ? _catalog.DollarPrefab : _catalog.BottlePrefab;
                Material material = spawn.IsPositive
                    ? null
                    : ResolvePickupMaterial(false);
                Vector3 scale = spawn.IsPositive
                    ? Vector3.one * _tuning.MoneyScale
                    : Vector3.one * _tuning.BottleScale;
                PathPose pose = _path.Sample(spawn.Z, spawn.X);
                views[i] = CreatePrefabOrMesh(
                    spawn.IsPositive ? "Money_" + i : "Bottle_" + i,
                    group,
                    prefab,
                    null,
                    material,
                    new Vector3(pose.X, pose.Y, pose.Z),
                    scale,
                    Quaternion.Euler(0f, pose.YawDegrees, 0f),
                    spawn.IsPositive);
            }

            return views;
        }

        private void BuildObstacles(ObstacleSpawn[] spawns)
        {
            Transform group = CreateGroup("Obstacles");
            for (int i = 0; i < spawns.Length; i++)
            {
                ObstacleSpawn spawn = spawns[i];
                PlaceMeshOnPath(
                    "Obstacle_" + i,
                    group,
                    _catalog.BoxMesh,
                    _catalog.PoorDoorMaterial,
                    spawn.X,
                    spawn.Z,
                    0f,
                    Vector3.one,
                    Quaternion.identity);
            }
        }

        private FlagView[] BuildFlags(FlagSpawn[] spawns)
        {
            Transform group = CreateGroup("Flags");
            PlaceMeshOnPath(
                "FlagStrip",
                group,
                _catalog.GroundMesh,
                _catalog.FlagMaterial,
                _tuning.FlagStripPosition.x,
                _tuning.FlagStripPosition.z,
                _tuning.FlagStripPosition.y,
                _tuning.FlagStripScale,
                Quaternion.identity);

            var views = new FlagView[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                views[i] = CreateFlag(group, spawns[i], i);
            }

            return views;
        }

        private FlagView CreateFlag(Transform group, FlagSpawn spawn, int index)
        {
            bool isLeft = spawn.X < 0f;
            Quaternion up = isLeft
                ? Quaternion.Euler(_tuning.LeftFlagUpEuler)
                : Quaternion.Euler(_tuning.RightFlagUpEuler);
            Quaternion down = isLeft
                ? Quaternion.Euler(_tuning.LeftFlagDownEuler)
                : Quaternion.Euler(_tuning.RightFlagDownEuler);

            PathPose pose = _path.Sample(spawn.Z, spawn.X);
            var go = new GameObject("Flag_" + index);
            go.transform.SetParent(group, false);
            go.transform.localPosition = new Vector3(pose.X, pose.Y, pose.Z);
            go.transform.localRotation = Quaternion.Euler(0f, pose.YawDegrees, 0f) * down;
            go.transform.localScale = Vector3.one * _tuning.FlagScale;

            if (_catalog.FlagMesh != null)
            {
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = _catalog.FlagMesh;
                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = _catalog.FlagMaterial;
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }

            SnapToPath(go);
            var view = EntityViewFactory.CreateOn<FlagView>(go);
            view.BindMotion(spawn.Z, Quaternion.Euler(0f, pose.YawDegrees, 0f) * down, Quaternion.Euler(0f, pose.YawDegrees, 0f) * up);
            return view;
        }

        private void BuildGate(GateSpawn gate)
        {
            Transform group = CreateGroup("Gates");
            PlaceMeshOnPath(
                "ChoiceLeft",
                group,
                _catalog.ChoiceDoorMesh,
                _catalog.ChoiceMaterial,
                -_tuning.GateHalfX,
                gate.Z,
                _tuning.GateDoorY,
                Vector3.one * _tuning.GateDoorScale,
                FaceRunner);
            PlaceMeshOnPath(
                "ChoiceRight",
                group,
                _catalog.ChoiceDoorMesh,
                _catalog.ChoiceMaterial,
                _tuning.GateHalfX,
                gate.Z,
                _tuning.GateDoorY,
                Vector3.one * _tuning.GateDoorScale,
                FaceRunner);
            PlaceMeshOnPath(
                "PartyIcon",
                group,
                _catalog.PartyMesh,
                _catalog.GoodDoorMaterial,
                -_tuning.GateHalfX,
                gate.Z,
                _tuning.GateIconY,
                Vector3.one * _tuning.PartyIconScale,
                FaceRunner);
            PlaceMeshOnPath(
                "SchoolIcon",
                group,
                _catalog.StudyMesh,
                _catalog.PoorDoorMaterial,
                _tuning.GateHalfX,
                gate.Z,
                _tuning.GateIconY,
                Vector3.one * _tuning.SchoolIconScale,
                FaceRunner);

            CreateLabel("PartyLabel", group, gate.LeftLabel, -_tuning.GateHalfX, gate.Z, _tuning.GateLabelY, _tuning.PartyLabelColor);
            CreateLabel("SchoolLabel", group, gate.RightLabel, _tuning.GateHalfX, gate.Z, _tuning.GateLabelY, _tuning.SchoolLabelColor);
        }

        private void BuildFinish(FinishSpawn finish)
        {
            Transform group = CreateGroup("Finish");
            Mesh planeMesh = _catalog.FinishPlaneMesh != null ? _catalog.FinishPlaneMesh : _catalog.GroundMesh;
            Vector3 planeSize = _tuning.FinishPlaneSize.sqrMagnitude > 0.01f
                ? _tuning.FinishPlaneSize
                : new Vector3(5.4f, 0.05f, 3.4f);
            PlaceFittedOnPath(
                "FinishPlane",
                group,
                planeMesh,
                _catalog.FinishMaterial,
                0f,
                finish.Z + _tuning.FinishPlaneZOffset,
                _tuning.FinishPlaneY,
                planeSize);

            Vector3 starSize = _tuning.FinishStarSize.sqrMagnitude > 0.01f
                ? _tuning.FinishStarSize
                : new Vector3(2.8f, 0.04f, 1.1f);
            PlaceFittedOnPath(
                "FinishStars",
                group,
                _catalog.FinishStarMesh,
                _catalog.FinishMaterial,
                0f,
                finish.Z,
                _tuning.PathSurfaceY + 0.02f,
                starSize);

            PlaceFinishLane("LaneX2", group, _catalog.FinishYellowMesh, _catalog.DoorPoorMesh, _catalog.PoorDoorMaterial, -_tuning.FinishLaneX, finish.Z, "x2");
            PlaceFinishLane("LaneX3", group, _catalog.FinishOrangeMesh, _catalog.DoorRichMesh, _catalog.GoodDoorMaterial, 0f, finish.Z, "x3");
            PlaceFinishLane("LaneX5", group, _catalog.FinishGreenMesh, _catalog.DoorMillionMesh, _catalog.GoodDoorMaterial, _tuning.FinishLaneX, finish.Z, "x5");
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
            PlaceMeshOnPath(
                "Panel",
                lane,
                panelMesh,
                _catalog.FinishMaterial,
                x,
                z,
                _tuning.FinishPanelY,
                Vector3.one * _tuning.FinishPanelScale,
                FaceRunner);
            PlaceMeshOnPath(
                "Door",
                lane,
                doorMesh,
                doorMaterial,
                x,
                z + _tuning.FinishDoorZOffset,
                0f,
                Vector3.one,
                FaceRunner);
            CreateLabel("Multiplier", lane, label, x, z, _tuning.FinishLabelY, Color.white);
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
            Vector3 scale,
            Quaternion rotation,
            bool spin)
        {
            LevelPieceView pooled = TryGetPooled(spin, prefab);
            if (pooled != null)
            {
                return PlacePooled(pooled, name, parent, material, position, scale, rotation, spin);
            }

            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab, parent);
                instance.name = name;
                instance.transform.localPosition = position;
                instance.transform.localRotation = rotation;
                instance.transform.localScale = scale;
                Paint(instance, material);
                SnapToPath(instance);
                return BindPiece(instance, spin);
            }

            return CreateMeshPiece(name, parent, fallbackMesh, material, position, scale, rotation);
        }

        private LevelPieceView TryGetPooled(bool spin, GameObject prefab)
        {
            if (_pickupPools == null || prefab == null)
            {
                return null;
            }

            if (spin && _pickupPools.Money != null)
            {
                return _pickupPools.Money.Get();
            }

            if (!spin && _pickupPools.Bottles != null)
            {
                return _pickupPools.Bottles.Get();
            }

            return null;
        }

        private LevelPieceView PlacePooled(
            LevelPieceView view,
            string name,
            Transform parent,
            Material material,
            Vector3 position,
            Vector3 scale,
            Quaternion rotation,
            bool spin)
        {
            GameObject instance = view.gameObject;
            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = position;
            instance.transform.localRotation = rotation;
            instance.transform.localScale = scale;
            Paint(instance, material);
            SnapToPath(instance);
            if (spin)
            {
                var spinning = view as PickupSpinView;
                float speed = _tuning.MoneySpinDegreesPerSecond > 0.01f ? _tuning.MoneySpinDegreesPerSecond : 72f;
                if (spinning != null)
                {
                    spinning.BindSpin(speed);
                }
            }
            else
            {
                view.Bind();
            }

            view.SetVisible(true);
            return view;
        }

        private void PlaceMeshOnPath(
            string name,
            Transform parent,
            Mesh mesh,
            Material material,
            float lateral,
            float distance,
            float extraY,
            Vector3 scale,
            Quaternion localRotation)
        {
            PathPose pose = _path.Sample(distance, lateral);
            CreateMeshPiece(
                name,
                parent,
                mesh,
                material,
                new Vector3(pose.X, pose.Y + extraY, pose.Z),
                scale,
                Quaternion.Euler(0f, pose.YawDegrees, 0f) * localRotation);
        }

        private void PlaceFittedOnPath(
            string name,
            Transform parent,
            Mesh mesh,
            Material material,
            float lateral,
            float distance,
            float extraY,
            Vector3 targetSize)
        {
            PathPose pose = _path.Sample(distance, lateral);
            Quaternion rotation = Quaternion.Euler(0f, pose.YawDegrees, 0f);
            CreateFittedMesh(
                name,
                parent,
                mesh,
                material,
                new Vector3(pose.X, pose.Y + extraY, pose.Z),
                targetSize,
                rotation);
        }

        private void CreateFittedMesh(
            string name,
            Transform parent,
            Mesh mesh,
            Material material,
            Vector3 worldCenter,
            Vector3 targetSize,
            Quaternion rotation)
        {
            if (mesh == null)
            {
                return;
            }

            Bounds local = mesh.bounds;
            Vector3 size = local.size;
            Vector3 scale = new Vector3(
                size.x > 0.0001f ? targetSize.x / size.x : 1f,
                size.y > 0.0001f ? targetSize.y / size.y : 1f,
                size.z > 0.0001f ? targetSize.z / size.z : 1f);
            Vector3 position = worldCenter - rotation * Vector3.Scale(local.center, scale);
            CreateMeshPiece(name, parent, mesh, material, position, scale, rotation);
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

        private void CreateLabel(
            string name,
            Transform parent,
            string text,
            float lateral,
            float distance,
            float extraY,
            Color color)
        {
            PathPose pose = _path.Sample(distance, lateral);
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(pose.X, pose.Y + extraY, pose.Z);
            go.AddComponent<MeshRenderer>();
            go.AddComponent<TextMesh>();
            var view = go.AddComponent<WorldLabelView>();
            view.Bind(text, _catalog.LabelFont, color, _tuning.LabelCharacterSize, _tuning.LabelFontSize);
            go.transform.localRotation = Quaternion.Euler(0f, pose.YawDegrees, 0f);
        }

        private void SnapToPath(GameObject instance)
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

            float delta = _tuning.PathSurfaceY - bounds.min.y;
            instance.transform.position += new Vector3(0f, delta, 0f);
        }

        private Material ResolvePickupMaterial(bool isMoney)
        {
            if (isMoney)
            {
                if (_goldMaterial == null)
                {
                    _goldMaterial = CreateStandard(_tuning.MoneyTint);
                }

                return _goldMaterial;
            }

            if (_bottleTintMaterial == null)
            {
                _bottleTintMaterial = CreateStandard(_tuning.BottleTint);
            }

            return _bottleTintMaterial;
        }

        private Material CreateStandard(Color tint)
        {
            Shader shader = Shader.Find("Standard");
            var material = new Material(shader);
            material.color = tint;
            material.SetFloat("_Glossiness", _tuning.PickupGlossiness);
            material.SetFloat("_Metallic", _tuning.PickupMetallic);
            return material;
        }

        private LevelPieceView BindPiece(GameObject go, bool spin)
        {
            if (spin)
            {
                var spinning = EntityViewFactory.CreateOn<PickupSpinView>(go);
                float speed = _tuning.MoneySpinDegreesPerSecond > 0.01f ? _tuning.MoneySpinDegreesPerSecond : 72f;
                spinning.BindSpin(speed);
                return spinning;
            }

            return BindPiece(go);
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
