namespace RunRich3D.Models
{
    public readonly struct PickupSpawn
    {
        public PickupSpawn(float x, float z, int wealthDelta)
        {
            X = x;
            Z = z;
            WealthDelta = wealthDelta;
        }

        public float X { get; }
        public float Z { get; }
        public int WealthDelta { get; }
        public bool IsPositive => WealthDelta > 0;
    }

    public readonly struct ObstacleSpawn
    {
        public ObstacleSpawn(float x, float z, float halfWidth, float halfDepth, int wealthPenalty)
        {
            X = x;
            Z = z;
            HalfWidth = halfWidth;
            HalfDepth = halfDepth;
            WealthPenalty = wealthPenalty;
        }

        public float X { get; }
        public float Z { get; }
        public float HalfWidth { get; }
        public float HalfDepth { get; }
        public int WealthPenalty { get; }
    }

    public readonly struct FlagSpawn
    {
        public FlagSpawn(float x, float z)
        {
            X = x;
            Z = z;
        }

        public float X { get; }
        public float Z { get; }
    }

    public readonly struct GateSpawn
    {
        public GateSpawn(float z, int leftWealthDelta, int rightWealthDelta, string leftLabel, string rightLabel)
        {
            Z = z;
            LeftWealthDelta = leftWealthDelta;
            RightWealthDelta = rightWealthDelta;
            LeftLabel = leftLabel;
            RightLabel = rightLabel;
        }

        public float Z { get; }
        public int LeftWealthDelta { get; }
        public int RightWealthDelta { get; }
        public string LeftLabel { get; }
        public string RightLabel { get; }
    }

    public readonly struct FinishSpawn
    {
        public FinishSpawn(float z, float laneHalfWidth, int leftMultiplier, int centerMultiplier, int rightMultiplier)
        {
            Z = z;
            LaneHalfWidth = laneHalfWidth;
            LeftMultiplier = leftMultiplier;
            CenterMultiplier = centerMultiplier;
            RightMultiplier = rightMultiplier;
        }

        public float Z { get; }
        public float LaneHalfWidth { get; }
        public int LeftMultiplier { get; }
        public int CenterMultiplier { get; }
        public int RightMultiplier { get; }
    }

    public sealed class LevelLayout
    {
        public LevelLayout(
            PickupSpawn[] pickups,
            ObstacleSpawn[] obstacles,
            FlagSpawn[] flags,
            GateSpawn gate,
            FinishSpawn finish,
            int pathTileCount,
            float pathTileLength)
        {
            Pickups = pickups;
            Obstacles = obstacles;
            Flags = flags;
            Gate = gate;
            Finish = finish;
            PathTileCount = pathTileCount < 1 ? 1 : pathTileCount;
            PathTileLength = pathTileLength;
        }

        public PickupSpawn[] Pickups { get; }
        public ObstacleSpawn[] Obstacles { get; }
        public FlagSpawn[] Flags { get; }
        public GateSpawn Gate { get; }
        public FinishSpawn Finish { get; }
        public int PathTileCount { get; }
        public float PathTileLength { get; }
    }
}
