namespace RunRich3D.Models
{
    internal sealed class WealthRules
    {
        internal WealthRules(
            int comfortableThreshold,
            int richThreshold,
            int maxDisplay,
            int poorOutfitWealth,
            int middleOutfitWealth,
            int richOutfitWealth,
            int millionaireOutfitWealth)
        {
            ComfortableThreshold = comfortableThreshold;
            RichThreshold = richThreshold;
            MaxDisplay = maxDisplay < 1 ? 1 : maxDisplay;
            PoorOutfitWealth = poorOutfitWealth;
            MiddleOutfitWealth = middleOutfitWealth;
            RichOutfitWealth = richOutfitWealth;
            MillionaireOutfitWealth = millionaireOutfitWealth;
        }

        internal int ComfortableThreshold { get; }
        internal int RichThreshold { get; }
        internal int MaxDisplay { get; }
        internal int PoorOutfitWealth { get; }
        internal int MiddleOutfitWealth { get; }
        internal int RichOutfitWealth { get; }
        internal int MillionaireOutfitWealth { get; }

        internal WealthTier TierFrom(int wealth)
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

        internal int OutfitFrom(int wealth)
        {
            if (wealth >= MillionaireOutfitWealth)
            {
                return CowboyOutfits.Millionaire;
            }

            if (wealth >= RichOutfitWealth)
            {
                return CowboyOutfits.Rich;
            }

            if (wealth >= MiddleOutfitWealth)
            {
                return CowboyOutfits.Middle;
            }

            if (wealth >= PoorOutfitWealth)
            {
                return CowboyOutfits.Poor;
            }

            return CowboyOutfits.Casual;
        }

        internal float Normalized(int wealth)
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
    }
}
