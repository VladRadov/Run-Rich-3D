using UniRx;

namespace RunRich3D.Models
{
    internal sealed class GameLoopModel
    {
        internal IReadOnlyReactiveProperty<int> LevelNumber => _levelNumber;
        internal IReadOnlyReactiveProperty<int> BankedCoins => _bankedCoins;

        private readonly IntReactiveProperty _levelNumber = new IntReactiveProperty(1);
        private readonly IntReactiveProperty _bankedCoins = new IntReactiveProperty(0);

        internal void AdvanceLevel()
        {
            _levelNumber.Value += 1;
        }

        internal void AddBankedCoins(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _bankedCoins.Value += amount;
        }
    }
}
