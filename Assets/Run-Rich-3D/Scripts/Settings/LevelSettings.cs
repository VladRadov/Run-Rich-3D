using UnityEngine;
using RunRich3D.Models;
using RunRich3D.Services;

namespace RunRich3D.Settings
{
    [CreateAssetMenu(menuName = "Run Rich 3D/Settings/Level", fileName = "LevelSettings")]
    public sealed class LevelSettings : ScriptableObject
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject _dollarPrefab;
        [SerializeField] private GameObject _bottlePrefab;
        [SerializeField] private Material _moneyMaterial;
        [SerializeField] private Material _bottleMaterial;
        [SerializeField] private Texture2D _plusSignTexture;

        [Header("World")]
        [SerializeField] private LevelWorldTuning _world = new LevelWorldTuning();

        [Header("Layout")]
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

        [SerializeField] private ObstaclePlacement[] _obstacles =
        {
            new ObstaclePlacement { X = 1.45f, Z = 13.4f, HalfWidth = 0.55f, HalfDepth = 0.55f, WealthPenalty = -10 }
        };

        [SerializeField] private FlagPlacement[] _flags =
        {
            new FlagPlacement { X = -2.55f, Z = 21.2f },
            new FlagPlacement { X = 2.55f, Z = 21.2f }
        };

        [Header("Gate")]
        [SerializeField] private float _gateZ = 54f;
        [SerializeField] private int _gateLeftWealth = -15;
        [SerializeField] private int _gateRightWealth = 20;
        [SerializeField] private string _gateLeftLabel = "Вечеринка";
        [SerializeField] private string _gateRightLabel = "Школа";

        [Header("Finish")]
        [SerializeField] private float _finishZ = 118f;
        [SerializeField] private float _finishLaneHalfWidth = 0.85f;
        [SerializeField] private int _finishLeftMultiplier = 2;
        [SerializeField] private int _finishCenterMultiplier = 3;
        [SerializeField] private int _finishRightMultiplier = 5;
        [SerializeField] private float _doorOpenStart = 5.5f;
        [SerializeField] private float _doorOpenEnd = 0.35f;
        [SerializeField] private float _doorOpenAngle = 95f;
        [SerializeField] private float _doorStopGap = 2.8f;
        [SerializeField] private float _afterLastOpenPadding = 1.2f;
        [SerializeField] private float _closedLaneClamp = 0.8f;
        [SerializeField] private float _defaultFallbackAngle = 90f;
        [SerializeField] private float _defaultFallbackRadius = 12f;
        [SerializeField] private float _defaultPathSurfaceY = 0.5f;
        [SerializeField] private float _defaultPickupHalfWidth = 0.7f;
        [SerializeField] private float _defaultPickupHalfDepth = 0.65f;
        [SerializeField] private float _defaultGateDepth = 0.8f;
        [SerializeField] private float _defaultMoneySpin = 72f;
        [SerializeField] private int _minDoorMultiplier = 2;
        [SerializeField] private float _doorPassEpsilon = 0.05f;
        [SerializeField] private float _flagRaiseDetectThreshold = 0.01f;
        [SerializeField] private float _effectFallbackSeconds = 1.5f;
        [SerializeField] private string _closedLaneName = "LaneX5";

        public GameObject DollarPrefab => _dollarPrefab;
        public GameObject BottlePrefab => _bottlePrefab;
        public Material MoneyMaterial => _moneyMaterial;
        public Material BottleMaterial => _bottleMaterial;
        public Texture2D PlusSignTexture => _plusSignTexture;
        public LevelWorldTuning World => _world != null ? _world : new LevelWorldTuning();
        public float FinishZ => _finishZ > 0.01f ? _finishZ : 118f;
        public float DoorOpenStart => _doorOpenStart;
        public float DoorOpenEnd => _doorOpenEnd;
        public float DoorOpenAngle => _doorOpenAngle;
        public float DoorStopGap => _doorStopGap;
        public float AfterLastOpenPadding => _afterLastOpenPadding;
        public float ClosedLaneClamp => _closedLaneClamp;
        public float DefaultFallbackAngle => _defaultFallbackAngle;
        public float DefaultFallbackRadius => _defaultFallbackRadius;
        public int MinDoorMultiplier => _minDoorMultiplier < 2 ? 2 : _minDoorMultiplier;
        public float DoorPassEpsilon => _doorPassEpsilon;
        public float FlagRaiseDetectThreshold => _flagRaiseDetectThreshold > 0f ? _flagRaiseDetectThreshold : 0.01f;
        public float EffectFallbackSeconds => _effectFallbackSeconds > 0.05f ? _effectFallbackSeconds : 1.5f;
        public string ClosedLaneName => string.IsNullOrEmpty(_closedLaneName) ? "LaneX5" : _closedLaneName;

        public float PathSurfaceY
        {
            get
            {
                LevelWorldTuning world = World;
                return world.PathSurfaceY > 0.01f ? world.PathSurfaceY : _defaultPathSurfaceY;
            }
        }

        public float PickupHalfWidth
        {
            get
            {
                LevelWorldTuning world = World;
                return world.PickupHalfWidth > 0.01f ? world.PickupHalfWidth : _defaultPickupHalfWidth;
            }
        }

        public float PickupHalfDepth
        {
            get
            {
                LevelWorldTuning world = World;
                return world.PickupHalfDepth > 0.01f ? world.PickupHalfDepth : _defaultPickupHalfDepth;
            }
        }

        public float GateDepth
        {
            get
            {
                LevelWorldTuning world = World;
                return world.GateDepth > 0.01f ? world.GateDepth : _defaultGateDepth;
            }
        }

        public float MoneySpinDegreesPerSecond
        {
            get
            {
                LevelWorldTuning world = World;
                return world.MoneySpinDegreesPerSecond > 0.01f ? world.MoneySpinDegreesPerSecond : _defaultMoneySpin;
            }
        }

        public float FlagRaiseStart
        {
            get
            {
                LevelWorldTuning world = World;
                return world.FlagRaiseStart > 0.01f ? world.FlagRaiseStart : _doorOpenStart;
            }
        }

        public float FlagRaiseEnd
        {
            get
            {
                LevelWorldTuning world = World;
                return world.FlagRaiseEnd > 0.01f ? world.FlagRaiseEnd : _doorOpenEnd;
            }
        }

        public LevelLayout CreateLayout(PathBend path)
        {
            LevelWorldTuning world = World;
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
                    FinishZ,
                    _finishLaneHalfWidth,
                    _finishLeftMultiplier,
                    _finishCenterMultiplier,
                    _finishRightMultiplier),
                pathTiles,
                tileLength);
        }

        public PathSegment[] CreatePathSegments()
        {
            PathPieceTuning[] pieces = World.Pieces;
            if (pieces == null || pieces.Length == 0)
            {
                return PathSegment.DefaultCourse();
            }

            var result = new PathSegment[pieces.Length];
            for (int i = 0; i < pieces.Length; i++)
            {
                PathPieceTuning piece = pieces[i];
                float angle = piece.Angle > 0.01f ? piece.Angle : _defaultFallbackAngle;
                float radius = piece.Radius > 0.01f ? piece.Radius : _defaultFallbackRadius;
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
    }
}
