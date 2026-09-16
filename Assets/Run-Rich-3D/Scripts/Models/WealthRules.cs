namespace RunRich3D.Models
{
    internal sealed class WealthRules
    {
        internal WealthRules(
            int comfortableThreshold,
            int richThreshold,
            int maxDisplay,
            int middleOutfitWealth,
            int casualOutfitWealth,
            int cocktailOutfitWealth,
            int businessOutfitWealth,
            int blingOutfitWealth)
        {
            ComfortableThreshold = comfortableThreshold;
            RichThreshold = richThreshold;
            MaxDisplay = maxDisplay < 1 ? 1 : maxDisplay;
            MiddleOutfitWealth = ClampMin(middleOutfitWealth, 0);
            CasualOutfitWealth = ClampMin(casualOutfitWealth, MiddleOutfitWealth);
            CocktailOutfitWealth = ClampMin(cocktailOutfitWealth, CasualOutfitWealth);
            BusinessOutfitWealth = ClampMin(businessOutfitWealth, CocktailOutfitWealth);
            BlingOutfitWealth = ClampMin(blingOutfitWealth, BusinessOutfitWealth);
        }

        internal int ComfortableThreshold { get; }
        internal int RichThreshold { get; }
        internal int MaxDisplay { get; }
        internal int MiddleOutfitWealth { get; }
        internal int CasualOutfitWealth { get; }
        internal int CocktailOutfitWealth { get; }
        internal int BusinessOutfitWealth { get; }
        internal int BlingOutfitWealth { get; }

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
            if (wealth >= BlingOutfitWealth)
            {
                return PlayerOutfits.Bling;
            }

            if (wealth >= BusinessOutfitWealth)
            {
                return PlayerOutfits.Business;
            }

            if (wealth >= CocktailOutfitWealth)
            {
                return PlayerOutfits.Cocktail;
            }

            if (wealth >= CasualOutfitWealth)
            {
                return PlayerOutfits.Casual;
            }

            if (wealth >= MiddleOutfitWealth)
            {
                return PlayerOutfits.Middle;
            }

            return PlayerOutfits.Poor;
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

        private static int ClampMin(int value, int min)
        {
            return value < min ? min : value;
        }
    }
}
