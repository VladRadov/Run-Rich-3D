namespace RunRich3D.Models
{
    internal readonly struct PickupSpawn
    {
        internal PickupSpawn(float x, float z, int wealthDelta)
        {
            X = x;
            Z = z;
            WealthDelta = wealthDelta;
        }

        internal float X { get; }
        internal float Z { get; }
        internal int WealthDelta { get; }
        internal bool IsPositive => WealthDelta > 0;
    }

    internal readonly struct ObstacleSpawn
    {
        internal ObstacleSpawn(float x, float z, float halfWidth, float halfDepth, int wealthPenalty)
        {
            X = x;
            Z = z;
            HalfWidth = halfWidth;
            HalfDepth = halfDepth;
            WealthPenalty = wealthPenalty;
        }

        internal float X { get; }
        internal float Z { get; }
        internal float HalfWidth { get; }
        internal float HalfDepth { get; }
        internal int WealthPenalty { get; }
    }

    internal readonly struct FlagSpawn
    {
        internal FlagSpawn(float x, float z)
        {
            X = x;
            Z = z;
        }

        internal float X { get; }
        internal float Z { get; }
    }

    internal readonly struct GateSpawn
    {
        internal GateSpawn(float z, int leftWealthDelta, int rightWealthDelta, string leftLabel, string rightLabel)
        {
            Z = z;
            LeftWealthDelta = leftWealthDelta;
            RightWealthDelta = rightWealthDelta;
            LeftLabel = leftLabel;
            RightLabel = rightLabel;
        }

        internal float Z { get; }
        internal int LeftWealthDelta { get; }
        internal int RightWealthDelta { get; }
        internal string LeftLabel { get; }
        internal string RightLabel { get; }
    }

    internal readonly struct FinishSpawn
    {
        internal FinishSpawn(float z, float laneHalfWidth, int leftMultiplier, int centerMultiplier, int rightMultiplier)
        {
            Z = z;
            LaneHalfWidth = laneHalfWidth;
            LeftMultiplier = leftMultiplier;
            CenterMultiplier = centerMultiplier;
            RightMultiplier = rightMultiplier;
        }

        internal float Z { get; }
        internal float LaneHalfWidth { get; }
        internal int LeftMultiplier { get; }
        internal int CenterMultiplier { get; }
        internal int RightMultiplier { get; }
    }

    internal sealed class LevelLayout
    {
        internal const int PathTileCount = 7;
        internal const float PathTileLength = 7.5f;

        private LevelLayout(
            PickupSpawn[] pickups,
            ObstacleSpawn[] obstacles,
            FlagSpawn[] flags,
            GateSpawn gate,
            FinishSpawn finish)
        {
            Pickups = pickups;
            Obstacles = obstacles;
            Flags = flags;
            Gate = gate;
            Finish = finish;
        }

        internal PickupSpawn[] Pickups { get; }
        internal ObstacleSpawn[] Obstacles { get; }
        internal FlagSpawn[] Flags { get; }
        internal GateSpawn Gate { get; }
        internal FinishSpawn Finish { get; }

        internal static LevelLayout CreateFirst()
        {
            return new LevelLayout(
                new[]
                {
                    new PickupSpawn(0f, 6.5f, 10),
                    new PickupSpawn(-1.5f, 8.2f, 10),
                    new PickupSpawn(1.5f, 8.2f, 10),
                    new PickupSpawn(1.55f, 11.2f, -20),
                    new PickupSpawn(-1.55f, 16.4f, 10),
                    new PickupSpawn(0f, 18.1f, 10),
                    new PickupSpawn(1.5f, 22.4f, 10),
                    new PickupSpawn(-1.5f, 26.2f, -20),
                    new PickupSpawn(0f, 28.4f, 10),
                    new PickupSpawn(1.5f, 36.2f, 10),
                    new PickupSpawn(-1.5f, 38.1f, 10),
                    new PickupSpawn(0f, 40.2f, 10)
                },
                new[]
                {
                    new ObstacleSpawn(1.45f, 13.4f, 0.55f, 0.55f, -10)
                },
                new[]
                {
                    new FlagSpawn(-2.55f, 21.2f),
                    new FlagSpawn(2.55f, 21.2f)
                },
                new GateSpawn(31.5f, 20, -15, "Вечеринка", "Школа"),
                new FinishSpawn(46f, 0.85f, 2, 3, 5));
        }
    }
}
