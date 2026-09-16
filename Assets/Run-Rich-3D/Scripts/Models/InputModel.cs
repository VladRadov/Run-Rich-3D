using System;
using UniRx;

namespace RunRich3D.Models
{
    public sealed class InputModel
    {
        public IReadOnlyReactiveProperty<bool> IsHeld => _isHeld;
        public IReadOnlyReactiveProperty<float> PointerScreenX => _pointerScreenX;
        public IObservable<float> Pressed => _pressed;
        public IObservable<float> Dragged => _dragged;
        public IObservable<Unit> Released => _released;

        private readonly BoolReactiveProperty _isHeld = new BoolReactiveProperty(false);
        private readonly FloatReactiveProperty _pointerScreenX = new FloatReactiveProperty(0f);
        private readonly Subject<float> _pressed = new Subject<float>();
        private readonly Subject<float> _dragged = new Subject<float>();
        private readonly Subject<Unit> _released = new Subject<Unit>();

        public void Begin(float screenX)
        {
            _pointerScreenX.Value = screenX;
            _isHeld.Value = true;
            _pressed.OnNext(screenX);
        }

        public void Move(float screenX)
        {
            if (!_isHeld.Value)
            {
                return;
            }

            _pointerScreenX.Value = screenX;
            _dragged.OnNext(screenX);
        }

        public void End()
        {
            if (!_isHeld.Value)
            {
                return;
            }

            _isHeld.Value = false;
            _released.OnNext(Unit.Default);
        }
    }
}
