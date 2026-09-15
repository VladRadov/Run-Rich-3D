using UniRx;

namespace RunRich3D.Models
{
    internal sealed class PickupToastModel
    {
        private readonly IntReactiveProperty _amount = new IntReactiveProperty(0);
        private readonly BoolReactiveProperty _visible = new BoolReactiveProperty(false);

        internal IReadOnlyReactiveProperty<int> Amount => _amount;
        internal IReadOnlyReactiveProperty<bool> IsVisible => _visible;

        internal void Add(int delta)
        {
            if (delta <= 0)
            {
                return;
            }

            _amount.Value += delta;
            _visible.Value = true;
        }

        internal void Hide()
        {
            _visible.Value = false;
            _amount.Value = 0;
        }
    }
}
