using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LevelWorldBuilder
    {
        public readonly struct Catalog
        {
            public Catalog(
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
                Material propsMaterial,
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
                PropsMaterial = propsMaterial;
                LabelFont = labelFont;
            }

            public GameObject DollarPrefab { get; }
            public GameObject BottlePrefab { get; }
            public Mesh GroundMesh { get; }
            public Mesh BoxMesh { get; }
            public Mesh FlagMesh { get; }
            public Mesh ChoiceDoorMesh { get; }
            public Mesh PartyMesh { get; }
            public Mesh StudyMesh { get; }
            public Mesh FinishBlueMesh { get; }
            public Mesh FinishGreenMesh { get; }
            public Mesh FinishOrangeMesh { get; }
            public Mesh FinishYellowMesh { get; }
            public Mesh DoorPoorMesh { get; }
            public Mesh DoorRichMesh { get; }
            public Mesh DoorMillionMesh { get; }
            public Mesh FinishPlaneMesh { get; }
            public Mesh FinishStarMesh { get; }
            public Material PathMaterial { get; }
            public Material MoneyMaterial { get; }
            public Material BottleMaterial { get; }
            public Material FlagMaterial { get; }
            public Material ChoiceMaterial { get; }
            public Material FinishMaterial { get; }
            public Material GoodDoorMaterial { get; }
            public Material PoorDoorMaterial { get; }
            public Material PropsMaterial { get; }
            public Font LabelFont { get; }
        }

        public readonly struct BuiltLevel
        {
            public BuiltLevel(LevelPieceView[] pickups, FlagView[] flags)
            {
                Pickups = pickups;
                Flags = flags;
            }

            public LevelPieceView[] Pickups { get; }
            public FlagView[] Flags { get; }
        }

        private readonly Transform _root;
        private readonly Catalog _catalog;
        private readonly LevelWorldTuning _tuning;
        private readonly PathBend _path;
        private readonly PickupPools _pickupPools;
        private readonly Texture2D _plusSignTexture;
        private Material _goldMaterial;
        private Material _bottleTintMaterial;
        private Material _gateLabelFill;
        private Material _gateLabelOutline;
        private Material _plusSignMaterial;
        private Material _minusSignMaterial;
        private Mesh _signQuad;
        private Texture2D _generatedMinusTexture;

        private const string SignChildName = "Sign";
        private static readonly Quaternion FaceRunner = Quaternion.identity;

        public LevelWorldBuilder(Transform root, Catalog catalog, LevelWorldTuning tuning, PathBend path)
            : this(root, catalog, tuning, path, null, null)
        {
        }

        public LevelWorldBuilder(
            Transform root,
            Catalog catalog,
            LevelWorldTuning tuning,
            PathBend path,
            PickupPools pickupPools)
            : this(root, catalog, tuning, path, pickupPools, null)
        {
        }

        public LevelWorldBuilder(
            Transform root,
            Catalog catalog,
            LevelWorldTuning tuning,
            PathBend path,
            PickupPools pickupPools,
            Texture2D plusSignTexture)
        {
            _root = root;
            _catalog = catalog;
            _tuning = tuning;
            _path = path ?? new PathBend(PathSegment.DefaultCourse(), 0f);
            _pickupPools = pickupPools;
            _plusSignTexture = plusSignTexture;
        }

        public BuiltLevel BuildRuntimePickups(LevelLayout layout)
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

        public FlagView[] BakeStatic(LevelLayout layout)
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
                    : ResolveBottleMaterial();
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
                AttachPickupSign(views[i], spawn.IsPositive, pose);
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
                _catalog.PoorDoorMaterial,
                -_tuning.GateHalfX,
                gate.Z,
                _tuning.GateDoorY,
                Vector3.one * _tuning.GateDoorScale,
                FaceRunner);
            PlaceMeshOnPath(
                "ChoiceRight",
                group,
                _catalog.ChoiceDoorMesh,
                _catalog.GoodDoorMaterial,
                _tuning.GateHalfX,
                gate.Z,
                _tuning.GateDoorY,
                Vector3.one * _tuning.GateDoorScale,
                FaceRunner);
            PlaceCenteredOnPath(
                "PartyIcon",
                group,
                _catalog.PartyMesh,
                _catalog.PropsMaterial,
                -_tuning.GateHalfX,
                gate.Z,
                _tuning.GateIconY,
                ResolveIconSize(_tuning.PartyIconSize));
            PlaceCenteredOnPath(
                "SchoolIcon",
                group,
                _catalog.StudyMesh,
                _catalog.PropsMaterial,
                _tuning.GateHalfX,
                gate.Z,
                _tuning.GateIconY,
                ResolveIconSize(_tuning.SchoolIconSize));

            CreateGateLabelBackdrop("PartyLabelBg", group, -_tuning.GateHalfX, gate.Z, _tuning.PartyLabelBgSize);
            CreateGateLabelBackdrop("SchoolLabelBg", group, _tuning.GateHalfX, gate.Z, _tuning.SchoolLabelBgSize);
            CreateLabel("PartyLabel", group, gate.LeftLabel, -_tuning.GateHalfX, gate.Z, _tuning.GateLabelY, _tuning.PartyLabelColor);
            CreateLabel("SchoolLabel", group, gate.RightLabel, _tuning.GateHalfX, gate.Z, _tuning.GateLabelY, _tuning.SchoolLabelColor);

            var view = EntityViewFactory.CreateOn<GateView>(group.gameObject);
            view.Bind(
                new[]
                {
                    ChildObject(group, "ChoiceLeft"),
                    ChildObject(group, "PartyIcon"),
                    ChildObject(group, "PartyLabelBgOutline"),
                    ChildObject(group, "PartyLabelBgFill"),
                    ChildObject(group, "PartyLabel")
                },
                new[]
                {
                    ChildObject(group, "ChoiceRight"),
                    ChildObject(group, "SchoolIcon"),
                    ChildObject(group, "SchoolLabelBgOutline"),
                    ChildObject(group, "SchoolLabelBgFill"),
                    ChildObject(group, "SchoolLabel")
                });
        }

        private static GameObject ChildObject(Transform parent, string name)
        {
            Transform child = parent.Find(name);
            return child != null ? child.gameObject : null;
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
                _catalog.PropsMaterial,
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
                _catalog.PropsMaterial,
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

        private static Vector3 ResolveIconSize(Vector3 size)
        {
            if (size.sqrMagnitude > 0.01f)
            {
                return size;
            }

            return new Vector3(1.15f, 1.15f, 0.45f);
        }

        private void PlaceCenteredOnPath(
            string name,
            Transform parent,
            Mesh mesh,
            Material material,
            float lateral,
            float distance,
            float extraY,
            Vector3 targetSize)
        {
            if (mesh == null)
            {
                return;
            }

            PathPose pose = _path.Sample(distance, lateral);
            Quaternion rotation = Quaternion.Euler(0f, pose.YawDegrees, 0f);
            Bounds local = mesh.bounds;
            float sx = local.size.x > 0.0001f ? targetSize.x / local.size.x : 1f;
            float sy = local.size.y > 0.0001f ? targetSize.y / local.size.y : 1f;
            float s = Mathf.Min(sx, sy);
            Vector3 scale = Vector3.one * s;
            Vector3 worldCenter = new Vector3(pose.X, pose.Y + extraY, pose.Z);
            Vector3 position = worldCenter - rotation * Vector3.Scale(local.center, scale);
            CreateMeshPiece(name, parent, mesh, material, position, scale, rotation);
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

        private void CreateGateLabelBackdrop(
            string name,
            Transform parent,
            float lateral,
            float distance,
            Vector3 size)
        {
            Vector3 pad = _tuning.GateLabelOutlinePad;
            if (pad.sqrMagnitude < 0.0001f)
            {
                pad = new Vector3(0.22f, 0.12f, 0.02f);
            }

            PlaceFittedOnPath(
                name + "Outline",
                parent,
                _catalog.FinishBlueMesh,
                EnsureGateLabelOutline(),
                lateral,
                distance + 0.04f,
                _tuning.GateLabelY,
                size + pad);
            PlaceFittedOnPath(
                name + "Fill",
                parent,
                _catalog.FinishBlueMesh,
                EnsureGateLabelFill(),
                lateral,
                distance + 0.02f,
                _tuning.GateLabelY,
                size);
        }

        private Material EnsureGateLabelFill()
        {
            if (_gateLabelFill == null)
            {
                _gateLabelFill = CreateStandard(new Color(0.18f, 0.18f, 0.2f));
                _gateLabelFill.SetFloat("_Glossiness", 0.08f);
                _gateLabelFill.SetFloat("_Metallic", 0f);
            }

            return _gateLabelFill;
        }

        private Material EnsureGateLabelOutline()
        {
            if (_gateLabelOutline == null)
            {
                _gateLabelOutline = CreateStandard(Color.black);
                _gateLabelOutline.SetFloat("_Glossiness", 0.05f);
                _gateLabelOutline.SetFloat("_Metallic", 0f);
            }

            return _gateLabelOutline;
        }

        private void SnapToPath(GameObject instance)
        {
            Renderer[] renderers = instance.GetComponentsInChildren<Renderer>(true);
            Renderer first = null;
            Bounds bounds = new Bounds();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (IsPickupSign(renderers[i]))
                {
                    continue;
                }

                if (first == null)
                {
                    first = renderers[i];
                    bounds = renderers[i].bounds;
                    continue;
                }

                bounds.Encapsulate(renderers[i].bounds);
            }

            if (first == null)
            {
                return;
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

            return ResolveBottleMaterial();
        }

        private Material ResolveBottleMaterial()
        {
            if (_catalog.BottleMaterial != null)
            {
                return _catalog.BottleMaterial;
            }

            if (_bottleTintMaterial == null)
            {
                _bottleTintMaterial = CreateStandard(_tuning.BottleTint);
            }

            return _bottleTintMaterial;
        }

        private Material CreateStandard(Color tint)
        {
            string shaderName = string.IsNullOrEmpty(_tuning.StandardShaderName) ? "Standard" : _tuning.StandardShaderName;
            Shader shader = Shader.Find(shaderName);
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

        private void AttachPickupSign(LevelPieceView piece, bool isPositive, PathPose pose)
        {
            if (piece == null)
            {
                return;
            }

            Transform parent = piece.transform;
            Transform existing = parent.Find(SignChildName);
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
            }
            else
            {
                go = new GameObject(SignChildName);
                go.transform.SetParent(parent, false);
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = EnsureSignQuad();
                var renderer = go.AddComponent<MeshRenderer>();
                renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
                renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
            }

            MeshRenderer signRenderer = go.GetComponent<MeshRenderer>();
            if (signRenderer != null)
            {
                signRenderer.sharedMaterial = isPositive ? EnsurePlusSignMaterial() : EnsureMinusSignMaterial();
                signRenderer.enabled = true;
            }

            Bounds bounds = PickupBounds(parent);
            Quaternion yaw = Quaternion.Euler(0f, pose.YawDegrees, 0f);
            Vector3 right = yaw * Vector3.right;
            Vector3 towardCamera = yaw * Vector3.back;
            float size = _tuning.PickupSignScale > 0.01f ? _tuning.PickupSignScale : 0.7f;
            Vector3 extra = _tuning.PickupSignOffset;
            if (extra.sqrMagnitude < 0.0001f)
            {
                extra = new Vector3(0.35f, 0.28f, 0f);
            }

            Vector3 worldPos = bounds.center
                + Vector3.up * (bounds.extents.y + extra.y)
                + right * extra.x
                + towardCamera * extra.z;
            Vector3 worldOffset = worldPos - parent.position;
            Quaternion facing = Quaternion.LookRotation(towardCamera, Vector3.up);
            var view = EntityViewFactory.CreateOn<PickupSignView>(go);
            view.Bind(parent, worldOffset, facing, size);
        }

        private static Bounds PickupBounds(Transform parent)
        {
            Renderer[] renderers = parent.GetComponentsInChildren<Renderer>(true);
            Renderer first = null;
            Bounds bounds = new Bounds();
            for (int i = 0; i < renderers.Length; i++)
            {
                if (IsPickupSign(renderers[i]) || !renderers[i].enabled)
                {
                    continue;
                }

                if (first == null)
                {
                    first = renderers[i];
                    bounds = renderers[i].bounds;
                    continue;
                }

                bounds.Encapsulate(renderers[i].bounds);
            }

            if (first != null)
            {
                return bounds;
            }

            return new Bounds(parent.position + Vector3.up * 0.6f, Vector3.one);
        }

        private Mesh EnsureSignQuad()
        {
            if (_signQuad != null)
            {
                return _signQuad;
            }

            _signQuad = new Mesh { name = "PickupSignQuad" };
            _signQuad.vertices = new[]
            {
                new Vector3(-0.5f, -0.5f, 0f),
                new Vector3(0.5f, -0.5f, 0f),
                new Vector3(-0.5f, 0.5f, 0f),
                new Vector3(0.5f, 0.5f, 0f)
            };
            _signQuad.uv = new[]
            {
                new Vector2(0f, 0f),
                new Vector2(1f, 0f),
                new Vector2(0f, 1f),
                new Vector2(1f, 1f)
            };
            _signQuad.triangles = new[]
            {
                0, 2, 1, 2, 3, 1,
                0, 1, 2, 2, 1, 3
            };
            _signQuad.RecalculateNormals();
            _signQuad.RecalculateBounds();
            return _signQuad;
        }

        private Material EnsurePlusSignMaterial()
        {
            if (_plusSignMaterial != null)
            {
                return _plusSignMaterial;
            }

            Texture2D texture = _plusSignTexture != null ? _plusSignTexture : CreateSignTexture(true);
            _plusSignMaterial = CreateUnlitTexture(texture);
            return _plusSignMaterial;
        }

        private Material EnsureMinusSignMaterial()
        {
            if (_minusSignMaterial != null)
            {
                return _minusSignMaterial;
            }

            _generatedMinusTexture = CreateSignTexture(false);
            _minusSignMaterial = CreateUnlitTexture(_generatedMinusTexture);
            return _minusSignMaterial;
        }

        private Material CreateUnlitTexture(Texture2D texture)
        {
            string signName = string.IsNullOrEmpty(_tuning.PickupSignShaderName) ? "RunRich3D/PickupSign" : _tuning.PickupSignShaderName;
            Shader shader = Shader.Find(signName);
            if (shader == null)
            {
                string unlitName = string.IsNullOrEmpty(_tuning.UnlitTextureShaderName) ? "Unlit/Texture" : _tuning.UnlitTextureShaderName;
                shader = Shader.Find(unlitName);
            }

            if (shader == null)
            {
                string standardName = string.IsNullOrEmpty(_tuning.StandardShaderName) ? "Standard" : _tuning.StandardShaderName;
                shader = Shader.Find(standardName);
            }

            var material = new Material(shader);
            material.mainTexture = texture;
            material.color = Color.white;
            if (material.HasProperty("_Color"))
            {
                material.SetColor("_Color", Color.white);
            }

            if (material.HasProperty("_Cutoff"))
            {
                material.SetFloat("_Cutoff", _tuning.PickupSignCutoff);
            }

            if (material.HasProperty("_EmissionColor"))
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", Color.white);
            }

            return material;
        }

        private static Texture2D CreateSignTexture(bool plus)
        {
            const int size = 256;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.name = plus ? "PickupPlus" : "PickupMinus";
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Bilinear;
            Color accent = plus ? new Color(0.2f, 1f, 0.28f) : new Color(1f, 0.12f, 0.12f);
            Color core = plus ? new Color(0.92f, 1f, 0.92f) : new Color(1f, 0.92f, 0.92f);
            var pixels = new Color[size * size];
            float inv = 2f / (size - 1);
            for (int y = 0; y < size; y++)
            {
                float py = y * inv - 1f;
                for (int x = 0; x < size; x++)
                {
                    float px = x * inv - 1f;
                    float symbol = plus ? PlusSdf(px, py) : MinusSdf(px, py);
                    float inner = Mathf.Clamp01(-symbol / 0.16f);
                    float glow = Mathf.Clamp01(1f - (symbol + 0.1f) / 0.16f);
                    Color color = Color.Lerp(accent, core, inner);
                    color.a = Mathf.Max(inner, glow * 0.85f);
                    if (color.a < 0.02f)
                    {
                        color = Color.clear;
                    }

                    pixels[y * size + x] = color;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply(false, true);
            return texture;
        }

        private static float PlusSdf(float x, float y)
        {
            float bar = CapsuleSdf(x, y, 0.62f, 0.2f);
            float upright = CapsuleSdf(y, x, 0.62f, 0.2f);
            return Mathf.Min(bar, upright);
        }

        private static float MinusSdf(float x, float y)
        {
            return CapsuleSdf(x, y, 0.62f, 0.2f);
        }

        private static float CapsuleSdf(float x, float y, float halfLength, float radius)
        {
            float ax = Mathf.Abs(x) - halfLength;
            if (ax < 0f)
            {
                ax = 0f;
            }

            return Mathf.Sqrt(ax * ax + y * y) - radius;
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
                if (IsPickupSign(renderers[i]))
                {
                    continue;
                }

                renderers[i].sharedMaterial = material;
            }
        }

        private static bool IsPickupSign(Component component)
        {
            return component != null && component.gameObject.name == SignChildName;
        }
    }
}
