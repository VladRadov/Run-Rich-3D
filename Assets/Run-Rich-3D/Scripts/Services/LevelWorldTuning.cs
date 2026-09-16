using System;
using UnityEngine;

namespace RunRich3D.Services
{
    [Serializable]
    public sealed class LevelWorldTuning
    {
        [Header("Path")]
        public float PathSurfaceY = 0.5f;
        public int PathTileCount = 18;
        public float PathTileLength = 7.5f;

        [Header("Path Course")]
        public PathPieceTuning[] Pieces =
        {
            new PathPieceTuning { Kind = PathPieceKind.Straight, Length = 24f },
            new PathPieceTuning { Kind = PathPieceKind.TurnRight, Radius = 14f, Angle = 90f },
            new PathPieceTuning { Kind = PathPieceKind.Straight, Length = 16f },
            new PathPieceTuning { Kind = PathPieceKind.TurnLeft, Radius = 12f, Angle = 90f },
            new PathPieceTuning { Kind = PathPieceKind.Straight, Length = 10f },
            new PathPieceTuning { Kind = PathPieceKind.TurnRight, Radius = 12f, Angle = 90f },
            new PathPieceTuning { Kind = PathPieceKind.Straight, Length = 14f }
        };
        public int BendSegments = 18;
        public float BendHalfWidth = 3f;
        public float BendSurfaceY = 0.36f;

        [Header("Pickups")]
        public float MoneyScale = 1.4f;
        public float MoneySpinDegreesPerSecond = 72f;
        public float BottleScale = 1.15f;
        public float PickupHalfWidth = 0.7f;
        public float PickupHalfDepth = 0.65f;
        public Color MoneyTint = new Color(0.93f, 0.74f, 0.12f);
        public Color BottleTint = new Color(1f, 0f, 0f);
        public float PickupGlossiness = 0.42f;
        public float PickupMetallic = 0.15f;
        public float PickupSignScale = 0.7f;
        public Vector3 PickupSignOffset = new Vector3(0.4f, 0.28f, 0.12f);

        [Header("Flags")]
        public float FlagScale = 0.28f;
        public float FlagRaiseStart = 5.5f;
        public float FlagRaiseEnd = 0.35f;
        public Vector3 FlagStripPosition = new Vector3(0f, 0.02f, 21.2f);
        public Vector3 FlagStripScale = new Vector3(1f, 0.18f, 0.42f);
        public Vector3 LeftFlagUpEuler = new Vector3(0f, 180f, 0f);
        public Vector3 LeftFlagDownEuler = new Vector3(0f, 180f, 90f);
        public Vector3 RightFlagUpEuler = Vector3.zero;
        public Vector3 RightFlagDownEuler = new Vector3(0f, 0f, 90f);

        [Header("Gates")]
        public float GateHalfX = 1.45f;
        public float GateDoorY = 0f;
        public float GateDoorScale = 1.45f;
        public float GateIconY = 1.01f;
        public float PartyIconScale = 4.2f;
        public float SchoolIconScale = 6.5f;
        public Vector3 PartyIconSize = new Vector3(1.15f, 1.15f, 0.45f);
        public Vector3 SchoolIconSize = new Vector3(1.15f, 1.15f, 0.45f);
        public float GateLabelY = 2.45f;
        public float GateDepth = 0.8f;
        public Color PartyLabelColor = new Color(1f, 0.45f, 0.3f);
        public Color SchoolLabelColor = new Color(0.35f, 0.9f, 0.4f);
        public Vector3 PartyLabelBgSize = new Vector3(2.9f, 0.78f, 0.1f);
        public Vector3 SchoolLabelBgSize = new Vector3(2.2f, 0.78f, 0.1f);
        public Vector3 GateLabelOutlinePad = new Vector3(0.22f, 0.12f, 0.02f);

        [Header("Finish")]
        public float FinishPlaneY = 0.03f;
        public float FinishPlaneZOffset = 0.6f;
        public Vector3 FinishPlaneScale = new Vector3(1f, 0.16f, 0.55f);
        public Vector3 FinishPlaneSize = new Vector3(5.4f, 0.05f, 3.4f);
        public Vector3 FinishStarSize = new Vector3(2.8f, 0.04f, 1.1f);
        public float FinishLaneX = 1.7f;
        public float FinishPanelY = 0.9f;
        public float FinishPanelScale = 3.6f;
        public float FinishDoorZOffset = 1.15f;
        public float FinishLabelY = 2.15f;

        [Header("Labels")]
        public float LabelCharacterSize = 0.06f;
        public int LabelFontSize = 64;

        [Header("Shaders")]
        public string StandardShaderName = "Standard";
        public string PickupSignShaderName = "RunRich3D/PickupSign";
        public string UnlitTextureShaderName = "Unlit/Texture";
        public float PickupSignCutoff = 0.12f;
    }
}
