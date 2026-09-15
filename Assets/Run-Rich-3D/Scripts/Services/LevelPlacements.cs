using System;
using UnityEngine;

namespace RunRich3D.Services
{
    [Serializable]
    public sealed class PickupPlacement
    {
        public float X;
        public float Z;
        public int WealthDelta;
    }

    [Serializable]
    public sealed class ObstaclePlacement
    {
        public float X;
        public float Z;
        public float HalfWidth = 0.55f;
        public float HalfDepth = 0.55f;
        public int WealthPenalty = -10;
    }

    [Serializable]
    public sealed class FlagPlacement
    {
        public float X;
        public float Z;
    }

    public enum PathPieceKind
    {
        Straight = 0,
        TurnRight = 1,
        TurnLeft = 2
    }

    [Serializable]
    public sealed class PathPieceTuning
    {
        public PathPieceKind Kind = PathPieceKind.Straight;
        public float Length = 10f;
        public float Radius = 12f;
        public float Angle = 90f;
    }
}
