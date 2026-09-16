namespace RunRich3D.Models
{
    public abstract class ConsumableRecord
    {
        protected ConsumableRecord(float x, float z, float halfWidth, float halfDepth)
        {
            X = x;
            Z = z;
            HalfWidth = halfWidth;
            HalfDepth = halfDepth;
        }

        public float X { get; }
        public float Z { get; }
        public float HalfWidth { get; }
        public float HalfDepth { get; }
        public bool IsConsumed { get; private set; }

        public bool Overlaps(float x, float z)
        {
            return !IsConsumed
                   && x <= X + HalfWidth
                   && x >= X - HalfWidth
                   && z <= Z + HalfDepth
                   && z >= Z - HalfDepth;
        }

        public void Consume()
        {
            IsConsumed = true;
        }

        public void Reset()
        {
            IsConsumed = false;
        }
    }

    public sealed class PickupRecord : ConsumableRecord
    {
        public PickupRecord(PickupSpawn spawn, float halfWidth, float halfDepth)
            : base(spawn.X, spawn.Z, halfWidth, halfDepth)
        {
            WealthDelta = spawn.WealthDelta;
        }

        public int WealthDelta { get; }
    }

    public sealed class ObstacleRecord : ConsumableRecord
    {
        public ObstacleRecord(ObstacleSpawn spawn)
            : base(spawn.X, spawn.Z, spawn.HalfWidth, spawn.HalfDepth)
        {
            WealthPenalty = spawn.WealthPenalty;
        }

        public int WealthPenalty { get; }
    }

    public sealed class GateRecord
    {
        public GateRecord(GateSpawn spawn, float depth)
        {
            Spawn = spawn;
            Depth = depth;
        }

        public GateSpawn Spawn { get; }
        public float Depth { get; }
        public bool IsConsumed { get; private set; }

        public bool Overlaps(float z)
        {
            float lead = Depth > 0.1f ? Depth : 0.5f;
            return !IsConsumed && z >= Spawn.Z - lead;
        }

        public int WealthDeltaFor(float x)
        {
            return x <= 0f ? Spawn.LeftWealthDelta : Spawn.RightWealthDelta;
        }

        public void Consume()
        {
            IsConsumed = true;
        }

        public void Reset()
        {
            IsConsumed = false;
        }
    }

    public sealed class FinishRecord
    {
        public FinishRecord(FinishSpawn spawn)
        {
            Spawn = spawn;
        }

        public FinishSpawn Spawn { get; }
        public bool IsConsumed { get; private set; }

        public bool Reached(float z)
        {
            return !IsConsumed && z >= Spawn.Z;
        }

        public int MultiplierFor(float x)
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

        public void Consume()
        {
            IsConsumed = true;
        }

        public void Reset()
        {
            IsConsumed = false;
        }
    }

    public sealed class LevelModel
    {
        public LevelModel(
            LevelLayout layout,
            float pickupHalfWidth,
            float pickupHalfDepth,
            float gateDepth)
        {
            Layout = layout;
            Pickups = CreatePickups(layout.Pickups, pickupHalfWidth, pickupHalfDepth);
            Obstacles = CreateObstacles(layout.Obstacles);
            Gate = new GateRecord(layout.Gate, gateDepth);
            Finish = new FinishRecord(layout.Finish);
        }

        public LevelLayout Layout { get; }
        public PickupRecord[] Pickups { get; }
        public ObstacleRecord[] Obstacles { get; }
        public GateRecord Gate { get; }
        public FinishRecord Finish { get; }

        public void ResetRun()
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

        private static PickupRecord[] CreatePickups(PickupSpawn[] spawns, float halfWidth, float halfDepth)
        {
            var records = new PickupRecord[spawns.Length];
            for (int i = 0; i < spawns.Length; i++)
            {
                records[i] = new PickupRecord(spawns[i], halfWidth, halfDepth);
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
