using UniRx;

namespace RunRich3D.Models
{
    public sealed class GameLoopModel
    {
        public IReadOnlyReactiveProperty<int> LevelNumber => _levelNumber;
        public IReadOnlyReactiveProperty<int> BankedCoins => _bankedCoins;

        private readonly IntReactiveProperty _levelNumber = new IntReactiveProperty(1);
        private readonly IntReactiveProperty _bankedCoins = new IntReactiveProperty(0);

        public void AdvanceLevel()
        {
            _levelNumber.Value += 1;
        }

        public void AddBankedCoins(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _bankedCoins.Value += amount;
        }
    }
}
