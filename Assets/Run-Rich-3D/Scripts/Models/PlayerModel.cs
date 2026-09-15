using UniRx;

namespace RunRich3D.Models
{
    internal sealed class PlayerModel
    {
        internal IReadOnlyReactiveProperty<float> LateralOffset => _lateralOffset;
        internal IReadOnlyReactiveProperty<float> ForwardPosition => _forwardPosition;
        internal IReadOnlyReactiveProperty<int> Wealth => _wealth;
        internal IReadOnlyReactiveProperty<GamePhase> Phase => _phase;

        private readonly FloatReactiveProperty _lateralOffset = new FloatReactiveProperty(0f);
        private readonly FloatReactiveProperty _forwardPosition = new FloatReactiveProperty(0f);
        private readonly IntReactiveProperty _wealth = new IntReactiveProperty(0);
        private readonly ReactiveProperty<GamePhase> _phase = new ReactiveProperty<GamePhase>(GamePhase.WaitingToStart);

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
        }

        internal void ApplyMultiplier(int multiplier)
        {
            if (multiplier <= 1)
            {
                return;
            }

            _wealth.Value *= multiplier;
        }

        internal void Reset(int startWealth)
        {
            _lateralOffset.Value = 0f;
            _forwardPosition.Value = 0f;
            _wealth.Value = startWealth < 0 ? 0 : startWealth;
            _phase.Value = GamePhase.WaitingToStart;
        }
    }
}
