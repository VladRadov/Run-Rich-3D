namespace RunRich3D.Models
{
    public sealed class WealthRules
    {
        public WealthRules(
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

        public int ComfortableThreshold { get; }
        public int RichThreshold { get; }
        public int MaxDisplay { get; }
        public int MiddleOutfitWealth { get; }
        public int CasualOutfitWealth { get; }
        public int CocktailOutfitWealth { get; }
        public int BusinessOutfitWealth { get; }
        public int BlingOutfitWealth { get; }

        public WealthTier TierFrom(int wealth)
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

        public int OutfitFrom(int wealth)
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

        public float Normalized(int wealth)
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
