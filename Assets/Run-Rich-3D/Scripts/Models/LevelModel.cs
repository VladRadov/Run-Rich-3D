namespace RunRich3D.Models
{
    internal abstract class ConsumableRecord
    {
        protected ConsumableRecord(float x, float z, float halfWidth, float halfDepth)
        {
            X = x;
            Z = z;
            HalfWidth = halfWidth;
            HalfDepth = halfDepth;
        }

        internal float X { get; }
        internal float Z { get; }
        internal float HalfWidth { get; }
        internal float HalfDepth { get; }
        internal bool IsConsumed { get; private set; }

        internal bool Overlaps(float x, float z)
        {
            return !IsConsumed
                   && x <= X + HalfWidth
                   && x >= X - HalfWidth
                   && z <= Z + HalfDepth
                   && z >= Z - HalfDepth;
        }

        internal void Consume()
        {
            IsConsumed = true;
        }

        internal void Reset()
        {
            IsConsumed = false;
        }
    }

    internal sealed class PickupRecord : ConsumableRecord
    {
        internal PickupRecord(PickupSpawn spawn)
            : base(spawn.X, spawn.Z, 0.7f, 0.65f)
        {
            WealthDelta = spawn.WealthDelta;
        }

        internal int WealthDelta { get; }
    }

    internal sealed class ObstacleRecord : ConsumableRecord
    {
        internal ObstacleRecord(ObstacleSpawn spawn)
            : base(spawn.X, spawn.Z, spawn.HalfWidth, spawn.HalfDepth)
        {
            WealthPenalty = spawn.WealthPenalty;
        }

        internal int WealthPenalty { get; }
    }

    internal sealed class GateRecord
    {
        internal GateRecord(GateSpawn spawn)
        {
            Spawn = spawn;
            Depth = 0.8f;
        }

        internal GateSpawn Spawn { get; }
        internal float Depth { get; }
        internal bool IsConsumed { get; private set; }

        internal bool Overlaps(float z)
        {
            return !IsConsumed
                   && z >= Spawn.Z - Depth
                   && z <= Spawn.Z + Depth;
        }

        internal int WealthDeltaFor(float x)
        {
            return x < 0f ? Spawn.LeftWealthDelta : Spawn.RightWealthDelta;
        }

        internal void Consume()
        {
            IsConsumed = true;
        }

        internal void Reset()
        {
            IsConsumed = false;
        }
    }

    internal sealed class FinishRecord
    {
        internal FinishRecord(FinishSpawn spawn)
        {
            Spawn = spawn;
        }

        internal FinishSpawn Spawn { get; }
        internal bool IsConsumed { get; private set; }

        internal bool Reached(float z)
        {
            return !IsConsumed && z >= Spawn.Z;
        }

        internal int MultiplierFor(float x)
        {
            if (x < -Spawn.LaneHalfWidth)
            {
                return Spawn.LeftMultiplier;
            }

            if (x > Spawn.LaneHalfWidth)
            {
                return Spawn.RightMultiplier;
            }

            return Spawn.CenterMultiplier;
        }

        internal void Consume()
        {
            IsConsumed = true;
        }

        internal void Reset()
        {
            IsConsumed = false;
        }
    }

    internal sealed class LevelModel
    {
        internal LevelModel(LevelLayout layout)
        {
            Layout = layout;
            Pickups = CreatePickups(layout.Pickups);
            Obstacles = CreateObstacles(layout.Obstacles);
            Gate = new GateRecord(layout.Gate);
            Finish = new FinishRecord(layout.Finish);
        }

        internal LevelLayout Layout { get; }
        internal PickupRecord[] Pickups { get; }
        internal ObstacleRecord[] Obstacles { get; }
        internal GateRecord Gate { get; }
        internal FinishRecord Finish { get; }

        internal void ResetRun()
        {
            for (int i = 0; i < Pickups.Length; i++)
            {
                Pickups[i].Reset();
            }

            for (int i = 0; i < Obstacles.Length; i++)
            {
                Obstacles[i].Reset();
            }

            Gate.Reset();
            Finish.Reset();
        }

        private static PickupRecord[] CreatePickups(PickupSpawn[] spawns)
        {
            var records = new PickupRecord[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                records[i] = new PickupRecord(spawns[i]);
            }

            return records;
        }

        private static ObstacleRecord[] CreateObstacles(ObstacleSpawn[] spawns)
        {
            var records = new ObstacleRecord[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                records[i] = new ObstacleRecord(spawns[i]);
            }

            return records;
        }
    }
}
