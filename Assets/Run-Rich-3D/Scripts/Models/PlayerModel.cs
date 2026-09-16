using UniRx;

namespace RunRich3D.Models
{
    public sealed class PlayerModel
    {
        public IReadOnlyReactiveProperty<float> LateralOffset => _lateralOffset;
        public IReadOnlyReactiveProperty<float> ForwardPosition => _forwardPosition;
        public IReadOnlyReactiveProperty<int> Wealth => _wealth;
        public IReadOnlyReactiveProperty<WealthTier> Tier => _tier;
        public IReadOnlyReactiveProperty<int> OutfitIndex => _outfitIndex;
        public IReadOnlyReactiveProperty<GamePhase> Phase => _phase;
        public WealthRules Rules => _rules;

        private readonly WealthRules _rules;
        private readonly FloatReactiveProperty _lateralOffset = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _forwardPosition = new FloatReactiveProperty(0f);
        private readonly IntReactiveProperty _wealth = new IntReactiveProperty(0);
        private readonly ReactiveProperty<WealthTier> _tier = new ReactiveProperty<WealthTier>(WealthTier.Poor);
        private readonly IntReactiveProperty _outfitIndex = new IntReactiveProperty(PlayerOutfits.Poor);
        private readonly ReactiveProperty<GamePhase> _phase = new ReactiveProperty<GamePhase>(GamePhase.WaitingToStart);

        public PlayerModel(WealthRules rules)
        {
            _rules = rules;
        }

        public void SetLateralOffset(float value)
        {
            _lateralOffset.Value = value;
        }

        public void SetForwardPosition(float value)
        {
            _forwardPosition.Value = value;
        }

        public void SetPhase(GamePhase phase)
        {
            _phase.Value = phase;
        }

        public void AddWealth(int delta)
        {
            int next = _wealth.Value + delta;
            if (next < 0)
            {
                _wealth.Value = 0;
                RefreshFromWealth();
                if (_phase.Value == GamePhase.Playing)
                {
                    _phase.Value = GamePhase.Lose;
                }

                return;
            }

            _wealth.Value = next;
            RefreshFromWealth();
        }

        public void ApplyMultiplier(int multiplier)
        {
            if (multiplier <= 1)
            {
                return;
            }

            _wealth.Value *= multiplier;
            RefreshFromWealth();
        }

        public void Reset(int startWealth)
        {
            _lateralOffset.Value = 0f;
            _forwardPosition.Value = 0f;
            _wealth.Value = startWealth < 0 ? 0 : startWealth;
            RefreshFromWealth();
            _phase.Value = GamePhase.WaitingToStart;
        }

        private void RefreshFromWealth()
        {
            _tier.Value = _rules.TierFrom(_wealth.Value);
            _outfitIndex.Value = _rules.OutfitFrom(_wealth.Value);
        }
    }
}
