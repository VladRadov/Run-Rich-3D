using System;

namespace RunRich3D.Models
{
    internal readonly struct PathPose
    {
        internal PathPose(float x, float y, float z, float yawDegrees)
        {
            X = x;
            Y = y;
            Z = z;
            YawDegrees = yawDegrees;
        }

        internal float X { get; }
        internal float Y { get; }
        internal float Z { get; }
        internal float YawDegrees { get; }
    }

    internal readonly struct PathSegment
    {
        internal PathSegment(bool isTurn, float lengthOrRadius, float angleDegrees, bool turnRight)
        {
            IsTurn = isTurn;
            if (isTurn)
            {
                Radius = lengthOrRadius < 1f ? 1f : lengthOrRadius;
                float degrees = angleDegrees < 1f ? 90f : angleDegrees;
                float sign = turnRight ? 1f : -1f;
                SignedAngle = sign * degrees * (float)(Math.PI / 180.0);
                float absAngle = SignedAngle < 0f ? -SignedAngle : SignedAngle;
                Length = Radius * absAngle;
            }
            else
            {
                Radius = 0f;
                SignedAngle = 0f;
                Length = lengthOrRadius < 0.01f ? 0.01f : lengthOrRadius;
            }
        }

        internal bool IsTurn { get; }
        internal float Length { get; }
        internal float Radius { get; }
        internal float SignedAngle { get; }
        internal float Sign => SignedAngle < 0f ? -1f : 1f;

        internal static PathSegment Straight(float length)
        {
            return new PathSegment(false, length, 0f, true);
        }

        internal static PathSegment Turn(float radius, float angleDegrees, bool turnRight)
        {
            return new PathSegment(true, radius, angleDegrees, turnRight);
        }

        internal static PathSegment[] DefaultCourse()
        {
            return new[]
            {
                Straight(24f),
                Turn(14f, 90f, true),
                Straight(16f),
                Turn(12f, 90f, false),
                Straight(10f),
                Turn(12f, 90f, true),
                Straight(14f)
            };
        }
    }

    internal sealed class PathBend
    {
        private readonly CompiledPiece[] _pieces;
        private readonly float _pathY;

        internal PathBend(PathSegment[] segments, float pathY)
        {
            _pathY = pathY;
            _pieces = Compile(segments);
            TotalLength = _pieces.Length == 0 ? 0f : _pieces[_pieces.Length - 1].StartDistance + _pieces[_pieces.Length - 1].Length;
        }

        internal float TotalLength { get; }
        internal CompiledPiece[] Pieces => _pieces;

        internal PathPose Sample(float distance, float lateral)
        {
            if (_pieces.Length == 0)
            {
                return new PathPose(lateral, _pathY, distance, 0f);
            }

            if (distance < 0f)
            {
                distance = 0f;
            }

            CompiledPiece piece = _pieces[_pieces.Length - 1];
            for (int i = 0; i < _pieces.Length; i++)
            {
                float end = _pieces[i].StartDistance + _pieces[i].Length;
                if (distance <= end || i == _pieces.Length - 1)
                {
                    piece = _pieces[i];
                    break;
                }
            }

            float local = distance - piece.StartDistance;
            if (local < 0f)
            {
                local = 0f;
            }

            if (!piece.IsTurn)
            {
                return StraightPose(piece, local, lateral);
            }

            return ArcPose(piece, local, lateral);
        }

        internal bool IsOnTurn(float distance)
        {
            for (int i = 0; i < _pieces.Length; i++)
            {
                CompiledPiece piece = _pieces[i];
                if (distance < piece.StartDistance || distance > piece.StartDistance + piece.Length)
                {
                    continue;
                }

                return piece.IsTurn;
            }

            return false;
        }

        private PathPose StraightPose(CompiledPiece piece, float local, float lateral)
        {
            float hx = (float)Math.Sin(piece.StartHeading);
            float hz = (float)Math.Cos(piece.StartHeading);
            float rx = (float)Math.Cos(piece.StartHeading);
            float rz = -(float)Math.Sin(piece.StartHeading);
            return new PathPose(
                piece.StartX + hx * local + rx * lateral,
                _pathY,
                piece.StartZ + hz * local + rz * lateral,
                piece.StartHeading * (180f / (float)Math.PI));
        }

        private PathPose ArcPose(CompiledPiece piece, float local, float lateral)
        {
            float radius = piece.Radius;
            float theta = local / radius;
            float absAngle = piece.SignedAngle < 0f ? -piece.SignedAngle : piece.SignedAngle;
            if (theta > absAngle)
            {
                theta = absAngle;
            }

            float sign = piece.Sign;
            float hx = (float)Math.Sin(piece.StartHeading);
            float hz = (float)Math.Cos(piece.StartHeading);
            float rx = (float)Math.Cos(piece.StartHeading);
            float rz = -(float)Math.Sin(piece.StartHeading);
            float x = piece.StartX + hx * (radius * (float)Math.Sin(theta)) + rx * (sign * radius * (1f - (float)Math.Cos(theta)));
            float z = piece.StartZ + hz * (radius * (float)Math.Sin(theta)) + rz * (sign * radius * (1f - (float)Math.Cos(theta)));
            float heading = piece.StartHeading + sign * theta;
            float crx = (float)Math.Cos(heading);
            float crz = -(float)Math.Sin(heading);
            return new PathPose(
                x + crx * lateral,
                _pathY,
                z + crz * lateral,
                heading * (180f / (float)Math.PI));
        }

        private static CompiledPiece[] Compile(PathSegment[] segments)
        {
            if (segments == null || segments.Length == 0)
            {
                segments = PathSegment.DefaultCourse();
            }

            var pieces = new CompiledPiece[segments.Length];
            float distance = 0f;
            float x = 0f;
            float z = 0f;
            float heading = 0f;
            for (int i = 0; i < segments.Length; i++)
            {
                PathSegment segment = segments[i];
                pieces[i] = new CompiledPiece(
                    segment.IsTurn,
                    distance,
                    segment.Length,
                    segment.Radius,
                    segment.SignedAngle,
                    x,
                    z,
                    heading);

                float endLocal = segment.Length;
                float hx = (float)Math.Sin(heading);
                float hz = (float)Math.Cos(heading);
                float rx = (float)Math.Cos(heading);
                float rz = -(float)Math.Sin(heading);
                if (!segment.IsTurn)
                {
                    x += hx * endLocal;
                    z += hz * endLocal;
                }
                else
                {
                    float theta = endLocal / segment.Radius;
                    float sign = segment.Sign;
                    x += hx * (segment.Radius * (float)Math.Sin(theta)) + rx * (sign * segment.Radius * (1f - (float)Math.Cos(theta)));
                    z += hz * (segment.Radius * (float)Math.Sin(theta)) + rz * (sign * segment.Radius * (1f - (float)Math.Cos(theta)));
                    heading += sign * theta;
                }

                distance += segment.Length;
            }

            return pieces;
        }

        internal readonly struct CompiledPiece
        {
            internal CompiledPiece(
                bool isTurn,
                float startDistance,
                float length,
                float radius,
                float signedAngle,
                float startX,
                float startZ,
                float startHeading)
            {
                IsTurn = isTurn;
                StartDistance = startDistance;
                Length = length;
                Radius = radius;
                SignedAngle = signedAngle;
                StartX = startX;
                StartZ = startZ;
                StartHeading = startHeading;
            }

            internal bool IsTurn { get; }
            internal float StartDistance { get; }
            internal float Length { get; }
            internal float Radius { get; }
            internal float SignedAngle { get; }
            internal float Sign => SignedAngle < 0f ? -1f : 1f;
            internal float StartX { get; }
            internal float StartZ { get; }
            internal float StartHeading { get; }
        }
    }
}
