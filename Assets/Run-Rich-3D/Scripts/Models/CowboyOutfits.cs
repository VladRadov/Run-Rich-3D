namespace RunRich3D.Models
{
    internal static class CowboyOutfits
    {
        internal const int Casual = 0;
        internal const int Poor = 1;
        internal const int Middle = 2;
        internal const int Rich = 3;
        internal const int Millionaire = 4;
        internal const int Count = 5;

        internal const int PoorThreshold = 20;
        internal const int MiddleThreshold = 40;
        internal const int RichThreshold = 80;
        internal const int MillionaireThreshold = 120;

        internal static int UpgradeFrom(int current)
        {
            if (current <= Casual)
            {
                return Middle;
            }

            if (current >= Count - 1)
            {
                return Count - 1;
            }

            return current + 1;
        }

        internal static int DowngradeFrom(int current)
        {
            if (current <= Casual)
            {
                return Casual;
            }

            return current - 1;
        }

        internal static int FromWealth(int wealth)
        {
            if (wealth >= MillionaireThreshold)
            {
                return Millionaire;
            }

            if (wealth >= RichThreshold)
            {
                return Rich;
            }

            if (wealth >= MiddleThreshold)
            {
                return Middle;
            }

            if (wealth >= PoorThreshold)
            {
                return Poor;
            }

            return Casual;
        }
    }
}
