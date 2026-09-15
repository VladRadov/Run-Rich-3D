namespace RunRich3D.Models
{
    internal static class WealthScale
    {
        internal const int ComfortableThreshold = 40;
        internal const int RichThreshold = 80;
        internal const int MaxDisplay = 120;

        internal static WealthTier FromWealth(int wealth)
        {
            if (wealth >= RichThreshold)
            {
                return WealthTier.Rich;
            }

            if (wealth >= ComfortableThreshold)
            {
                return WealthTier.Comfortable;
            }

            return WealthTier.Poor;
        }

        internal static float Normalized(int wealth)
        {
            if (wealth <= 0)
            {
                return 0f;
            }

            if (wealth >= MaxDisplay)
            {
                return 1f;
            }

            return wealth / (float)MaxDisplay;
        }

        internal static string Label(WealthTier tier)
        {
            switch (tier)
            {
                case WealthTier.Comfortable:
                    return "Состоятельный";
                case WealthTier.Rich:
                    return "Богатый";
                default:
                    return "Бедный";
            }
        }
    }
}
