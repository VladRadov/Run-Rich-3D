using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Settings;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class InputService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Transform _uiRoot;
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _inputCatcher;

        private InputSettings _settings;
        private InputModel _model;
        private SwipeInputView _view;
        private InputController _controller;

        public InputModel Model => _model;
        public Transform CanvasRoot { get; private set; }

        [Inject]
        public void Construct(InputSettings settings)
        {
            _settings = settings;
        }

        public void Initialize()
        {
            EnsureEventSystem();
            GameObject catcher = EnsureInputCatcher();

            _model = new InputModel();
            _view = EntityViewFactory.CreateOn<SwipeInputView>(catcher);
            _controller = new InputController(_model, _view);
            _controller.Initialize();
        }

        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }

        private void EnsureEventSystem()
        {
            if (_eventSystem != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(_uiRoot, false);
            _eventSystem = eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private GameObject EnsureInputCatcher()
        {
            if (_canvas == null)
            {
                var canvasObject = new GameObject("Canvas");
                canvasObject.transform.SetParent(_uiRoot, false);
                _canvas = canvasObject.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 0;
                var scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = _settings.ReferenceResolution;
                scaler.matchWidthOrHeight = _settings.MatchWidthOrHeight;
                canvasObject.AddComponent<GraphicRaycaster>();
            }

            CanvasRoot = _canvas.transform;
            if (_inputCatcher != null)
            {
                return _inputCatcher;
            }

            _inputCatcher = new GameObject("InputCatcher");
            var rect = _inputCatcher.AddComponent<RectTransform>();
            _inputCatcher.transform.SetParent(_canvas.transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = _inputCatcher.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;
            return _inputCatcher;
        }
    }
}
