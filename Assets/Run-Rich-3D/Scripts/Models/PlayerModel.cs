using System;
using UniRx;

namespace RunRich3D.Models
{
    internal sealed class PlayerModel
    {
        internal IReadOnlyReactiveProperty<float> LateralOffset => _lateralOffset;
        internal IReadOnlyReactiveProperty<float> ForwardPosition => _forwardPosition;
        internal IReadOnlyReactiveProperty<int> Wealth => _wealth;
        internal IReadOnlyReactiveProperty<WealthTier> Tier => _tier;
        internal IReadOnlyReactiveProperty<int> OutfitIndex => _outfitIndex;
        internal IReadOnlyReactiveProperty<GamePhase> Phase => _phase;
        internal IObservable<int> GateReskin => _gateReskin;
        internal WealthRules Rules => _rules;

        private readonly WealthRules _rules;
        private readonly FloatReactiveProperty _lateralOffset = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _forwardPosition = new FloatReactiveProperty(0f);
        private readonly IntReactiveProperty _wealth = new IntReactiveProperty(0);
        private readonly ReactiveProperty<WealthTier> _tier = new ReactiveProperty<WealthTier>(WealthTier.Poor);
        private readonly IntReactiveProperty _outfitIndex = new IntReactiveProperty(CowboyOutfits.Casual);
        private readonly ReactiveProperty<GamePhase> _phase = new ReactiveProperty<GamePhase>(GamePhase.WaitingToStart);
        private readonly Subject<int> _gateReskin = new Subject<int>();

        internal PlayerModel(WealthRules rules)
        {
            _rules = rules;
        }

        internal void SetLateralOffset(float value)
        {
            _lateralOffset.Value = value;
        }

        internal void SetForwardPosition(float value)
        {
            _forwardPosition.Value = value;
        }

        internal void SetPhase(GamePhase phase)
        {
            _phase.Value = phase;
        }

        internal void AddWealth(int delta)
        {
            int next = _wealth.Value + delta;
            _wealth.Value = next < 0 ? 0 : next;
            _tier.Value = _rules.TierFrom(_wealth.Value);
        }

        internal void ApplyMultiplier(int multiplier)
        {
            if (multiplier <= 1)
            {
                return;
            }

            _wealth.Value *= multiplier;
            _tier.Value = _rules.TierFrom(_wealth.Value);
        }

        internal void ApplyGateReskin(int wealthDelta)
        {
            int next = CowboyOutfits.UpgradeFrom(_outfitIndex.Value);
            _outfitIndex.Value = next;
            _gateReskin.OnNext(next);
        }

        internal void Reset(int startWealth)
        {
            _lateralOffset.Value = 0f;
            _forwardPosition.Value = 0f;
            _wealth.Value = startWealth < 0 ? 0 : startWealth;
            _tier.Value = _rules.TierFrom(_wealth.Value);
            _outfitIndex.Value = CowboyOutfits.Casual;
            _phase.Value = GamePhase.WaitingToStart;
        }
    }
}
