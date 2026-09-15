using UnityEngine;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class LevelService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Transform _levelRoot;
        [SerializeField] private Transform _pickupRoot;
        [SerializeField] private FlagView[] _flagViews;
        [SerializeField] private PlayerService _playerService;
        [SerializeField] private GameObject _legacyPath;

        [Header("Prefabs")]
        [SerializeField] private GameObject _dollarPrefab;
        [SerializeField] private GameObject _bottlePrefab;

        [Header("Pickup Materials")]
        [SerializeField] private Material _moneyMaterial;
        [SerializeField] private Material _bottleMaterial;
        [SerializeField] private Texture2D _plusSignTexture;

        [Header("World Tuning")]
        [SerializeField] private LevelWorldTuning _world = new LevelWorldTuning();

        [Header("Level Layout")]
        [SerializeField] private PickupPlacement[] _pickups =
        {
            new PickupPlacement { X = 0f, Z = 6.5f, WealthDelta = 10 },
            new PickupPlacement { X = -1.5f, Z = 8.2f, WealthDelta = 10 },
            new PickupPlacement { X = 1.5f, Z = 8.2f, WealthDelta = 10 },
            new PickupPlacement { X = 1.55f, Z = 11.2f, WealthDelta = -20 },
            new PickupPlacement { X = -1.55f, Z = 16.4f, WealthDelta = 10 },
            new PickupPlacement { X = 0f, Z = 18.1f, WealthDelta = 10 },
            new PickupPlacement { X = 1.5f, Z = 48f, WealthDelta = 10 },
            new PickupPlacement { X = -1.5f, Z = 50f, WealthDelta = -20 },
            new PickupPlacement { X = 0f, Z = 58f, WealthDelta = 10 },
            new PickupPlacement { X = 1.5f, Z = 83f, WealthDelta = 10 },
            new PickupPlacement { X = -1.5f, Z = 86f, WealthDelta = 10 },
            new PickupPlacement { X = 0f, Z = 89f, WealthDelta = 10 }
        };

        [Header("Obstacles")]
        [SerializeField] private ObstaclePlacement[] _obstacles =
        {
            new ObstaclePlacement { X = 1.45f, Z = 13.4f, HalfWidth = 0.55f, HalfDepth = 0.55f, WealthPenalty = -10 }
        };

        [Header("Flags")]
        [SerializeField] private FlagPlacement[] _flags =
        {
            new FlagPlacement { X = -2.55f, Z = 21.2f },
            new FlagPlacement { X = 2.55f, Z = 21.2f }
        };

        [Header("Gate Layout")]
        [SerializeField] private float _gateZ = 54f;
        [SerializeField] private int _gateLeftWealth = -15;
        [SerializeField] private int _gateRightWealth = 20;
        [SerializeField] private string _gateLeftLabel = "Вечеринка";
        [SerializeField] private string _gateRightLabel = "Школа";

        [Header("Finish Layout")]
        [SerializeField] private float _finishZ = 118f;
        [SerializeField] private float _finishLaneHalfWidth = 0.85f;
        [SerializeField] private int _finishLeftMultiplier = 2;
        [SerializeField] private int _finishCenterMultiplier = 3;
        [SerializeField] private int _finishRightMultiplier = 5;

        private LevelModel _model;
        private LevelController _controller;
        private PickupPools _pickupPools;

        internal ILevelEvents Events => _controller;

        public void Initialize()
        {
            if (_legacyPath != null)
            {
                _legacyPath.SetActive(false);
            }

            LevelWorldTuning world = _world != null ? _world : new LevelWorldTuning();
            var path = new PathBend(ToSegments(world.Pieces), 0f);
            LevelLayout layout = CreateLayout(world, path);
            float pickupHalfWidth = world.PickupHalfWidth > 0.01f ? world.PickupHalfWidth : 0.7f;
            float pickupHalfDepth = world.PickupHalfDepth > 0.01f ? world.PickupHalfDepth : 0.65f;
            float gateDepth = world.GateDepth > 0.01f ? world.GateDepth : 0.8f;
            _model = new LevelModel(layout, pickupHalfWidth, pickupHalfDepth, gateDepth);

            _playerService.BindPath(path);

            Transform pickupRoot = _pickupRoot != null ? _pickupRoot : _levelRoot;
            float spin = world.MoneySpinDegreesPerSecond > 0.01f ? world.MoneySpinDegreesPerSecond : 72f;
            _pickupPools?.Dispose();
            _pickupPools = new PickupPools(pickupRoot, _dollarPrefab, _bottlePrefab, spin);
            var builder = new LevelWorldBuilder(pickupRoot, CreateCatalog(), world, path, _pickupPools, _plusSignTexture);
            LevelWorldBuilder.BuiltLevel visuals = builder.BuildRuntimePickups(layout);
            FlagView[] flags = _flagViews != null ? _flagViews : new FlagView[0];

            _controller = new LevelController(
                _model,
                _playerService.Model,
                visuals.Pickups,
                flags,
                world.FlagRaiseStart > 0.01f ? world.FlagRaiseStart : 5.5f,
                world.FlagRaiseEnd > 0.01f ? world.FlagRaiseEnd : 0.35f);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
            _pickupPools?.Dispose();
            _pickupPools = null;
        }

        internal FlagView[] BakeStaticWorld(Transform staticRoot, LevelWorldBuilder.Catalog catalog)
        {
            LevelWorldTuning world = _world != null ? _world : new LevelWorldTuning();
            var path = new PathBend(ToSegments(world.Pieces), 0f);
            LevelLayout layout = CreateLayout(world, path);
            var builder = new LevelWorldBuilder(staticRoot, catalog, world, path);
            return builder.BakeStatic(layout);
        }

        internal Transform LevelRoot => _levelRoot;

        private LevelLayout CreateLayout(LevelWorldTuning world, PathBend path)
        {
            float gateZ = _gateZ > 0.01f ? _gateZ : 54f;
            float tileLength = world.PathTileLength > 0.01f ? world.PathTileLength : 7.5f;
            float courseLength = path != null && path.TotalLength > tileLength ? path.TotalLength : tileLength;
            int pathTiles = Mathf.CeilToInt(courseLength / tileLength) + 1;
            return new LevelLayout(
                ToPickups(_pickups),
                ToObstacles(_obstacles),
                ToFlags(_flags),
                new GateSpawn(gateZ, _gateLeftWealth, _gateRightWealth, _gateLeftLabel, _gateRightLabel),
                new FinishSpawn(
                    _finishZ,
                    _finishLaneHalfWidth,
                    _finishLeftMultiplier,
                    _finishCenterMultiplier,
                    _finishRightMultiplier),
                pathTiles,
                tileLength);
        }

        private static PickupSpawn[] ToPickups(PickupPlacement[] placements)
        {
            if (placements == null)
            {
                return new PickupSpawn[0];
            }

            var result = new PickupSpawn[placements.Length];
            for (int i = 0; i < placements.Length; i++)
            {
                PickupPlacement p = placements[i];
                result[i] = new PickupSpawn(p.X, p.Z, p.WealthDelta);
            }

            return result;
        }

        private static ObstacleSpawn[] ToObstacles(ObstaclePlacement[] placements)
        {
            if (placements == null)
            {
                return new ObstacleSpawn[0];
            }

            var result = new ObstacleSpawn[placements.Length];
            for (int i = 0; i < placements.Length; i++)
            {
                ObstaclePlacement p = placements[i];
                result[i] = new ObstacleSpawn(p.X, p.Z, p.HalfWidth, p.HalfDepth, p.WealthPenalty);
            }

            return result;
        }

        private static FlagSpawn[] ToFlags(FlagPlacement[] placements)
        {
            if (placements == null)
            {
                return new FlagSpawn[0];
            }

            var result = new FlagSpawn[placements.Length];
            for (int i = 0; i < placements.Length; i++)
            {
                FlagPlacement p = placements[i];
                result[i] = new FlagSpawn(p.X, p.Z);
            }

            return result;
        }

        private static PathSegment[] ToSegments(PathPieceTuning[] pieces)
        {
            if (pieces == null || pieces.Length == 0)
            {
                return PathSegment.DefaultCourse();
            }

            var result = new PathSegment[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                PathPieceTuning piece = pieces[i];
                float angle = piece.Angle > 0.01f ? piece.Angle : 90f;
                float radius = piece.Radius > 0.01f ? piece.Radius : 12f;
                if (piece.Kind == PathPieceKind.TurnRight)
                {
                    result[i] = PathSegment.Turn(radius, angle, true);
                    continue;
                }

                if (piece.Kind == PathPieceKind.TurnLeft)
                {
                    result[i] = PathSegment.Turn(radius, angle, false);
                    continue;
                }

                result[i] = PathSegment.Straight(piece.Length);
            }

            return result;
        }

        private LevelWorldBuilder.Catalog CreateCatalog()
        {
            return new LevelWorldBuilder.Catalog(
                _dollarPrefab,
                _bottlePrefab,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                _moneyMaterial,
                _bottleMaterial,
                null,
                null,
                null,
                null,
                null,
                null);
        }
    }
}
