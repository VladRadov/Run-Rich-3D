namespace RunRich3D.Models
{
    public static class WealthScale
    {
        public const int ComfortableThreshold = 40;
        public const int RichThreshold = 80;
        public const int MaxDisplay = 120;

        public static WealthTier FromWealth(int wealth)
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

        public static float Normalized(int wealth)
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

        public static string Label(WealthTier tier)
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
