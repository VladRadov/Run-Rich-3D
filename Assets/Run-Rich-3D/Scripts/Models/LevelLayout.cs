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
        internal LevelLayout(
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

        internal PickupSpawn[] Pickups { get; }
        internal ObstacleSpawn[] Obstacles { get; }
        internal FlagSpawn[] Flags { get; }
        internal GateSpawn Gate { get; }
        internal FinishSpawn Finish { get; }
        internal int PathTileCount { get; }
        internal float PathTileLength { get; }
    }
}
