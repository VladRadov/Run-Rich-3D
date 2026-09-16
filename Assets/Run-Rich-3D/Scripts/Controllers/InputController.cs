using System;
using UniRx;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Controllers
{
    public sealed class InputController : IDisposable
    {
        private readonly InputModel _model;
        private readonly SwipeInputView _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        public InputController(InputModel model, SwipeInputView view)
        {
            _model = model;
            _view = view;
        }

        public void Initialize()
        {
            _view.Pressed.Subscribe(_model.Begin).AddTo(_disposables);
            _view.Moved.Subscribe(_model.Move).AddTo(_disposables);
            _view.Released.Subscribe(_ => _model.End()).AddTo(_disposables);
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
