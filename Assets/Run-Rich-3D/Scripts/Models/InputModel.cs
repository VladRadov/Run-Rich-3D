using System;
using UniRx;

namespace RunRich3D.Models
{
    internal sealed class InputModel
    {
        internal IReadOnlyReactiveProperty<bool> IsHeld => _isHeld;
        internal IReadOnlyReactiveProperty<float> PointerScreenX => _pointerScreenX;
        internal IObservable<float> Pressed => _pressed;
        internal IObservable<float> Dragged => _dragged;
        internal IObservable<Unit> Released => _released;

        private readonly BoolReactiveProperty _isHeld = new BoolReactiveProperty(false);
        private readonly FloatReactiveProperty _pointerScreenX = new FloatReactiveProperty(0f);
        private readonly Subject<float> _pressed = new Subject<float>();
        private readonly Subject<float> _dragged = new Subject<float>();
        private readonly Subject<Unit> _released = new Subject<Unit>();

        internal void Begin(float screenX)
        {
            _pointerScreenX.Value = screenX;
            _isHeld.Value = true;
            _pressed.OnNext(screenX);
        }

        internal void Move(float screenX)
        {
            if (!_isHeld.Value)
            {
                return;
            }

            _pointerScreenX.Value = screenX;
            _dragged.OnNext(screenX);
        }

        internal void End()
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
