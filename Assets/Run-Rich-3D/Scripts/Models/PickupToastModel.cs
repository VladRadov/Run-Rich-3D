using UniRx;

namespace RunRich3D.Models
{
    public sealed class PickupToastModel
    {
        private readonly IntReactiveProperty _amount = new IntReactiveProperty(0);
        private readonly BoolReactiveProperty _visible = new BoolReactiveProperty(false);

        public IReadOnlyReactiveProperty<int> Amount => _amount;
        public IReadOnlyReactiveProperty<bool> IsVisible => _visible;

        public void Add(int delta)
        {
            if (delta <= 0)
            {
                return;
            }

            _amount.Value += delta;
            _visible.Value = true;
        }

        public void Hide()
        {
            _visible.Value = false;
            _amount.Value = 0;
        }
    }
}
