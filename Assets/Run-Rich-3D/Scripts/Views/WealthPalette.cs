using UnityEngine;
using RunRich3D.Models;

namespace RunRich3D.Views
{
    public static class WealthPalette
    {
        public static Color Of(WealthTier tier)
        {
            switch (tier)
            {
                case WealthTier.Comfortable:
                    return new Color(0.35f, 0.48f, 0.72f);
                case WealthTier.Rich:
                    return new Color(0.92f, 0.74f, 0.18f);
                default:
                    return new Color(0.45f, 0.32f, 0.22f);
            }
        }

        public static Color BarOf(WealthTier tier)
        {
            switch (tier)
            {
                case WealthTier.Comfortable:
                    return new Color(0.98f, 0.78f, 0.18f);
                case WealthTier.Rich:
                    return new Color(0.28f, 0.82f, 0.38f);
                default:
                    return new Color(0.92f, 0.28f, 0.28f);
            }
        }
    }
}
