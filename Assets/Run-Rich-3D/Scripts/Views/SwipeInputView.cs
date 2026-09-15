using System;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RunRich3D.Views
{
    public sealed class SwipeInputView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private readonly Subject<float> _pressed = new Subject<float>();
        private readonly Subject<float> _moved = new Subject<float>();
        private readonly Subject<Unit> _released = new Subject<Unit>();

        internal IObservable<float> Pressed => _pressed;
        internal IObservable<float> Moved => _moved;
        internal IObservable<Unit> Released => _released;

        public void OnPointerDown(PointerEventData eventData)
        {
            _pressed.OnNext(eventData.position.x);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _moved.OnNext(eventData.position.x);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _released.OnNext(Unit.Default);
        }

        private void OnDestroy()
        {
            _pressed.OnCompleted();
            _moved.OnCompleted();
            _released.OnCompleted();
        }
    }
}
