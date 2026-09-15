using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using RunRich3D.Controllers;
using RunRich3D.Models;
using RunRich3D.Views;

namespace RunRich3D.Services
{
    public sealed class InputService : MonoBehaviour, IGameService
    {
        [Header("References")]
        [SerializeField] private Transform _uiRoot;

        [Header("Canvas")]
        [SerializeField] private Vector2 _referenceResolution = new Vector2(1080f, 1920f);
        [SerializeField] private float _matchWidthOrHeight = 1f;

        private InputModel _model;
        private SwipeInputView _view;
        private InputController _controller;

        internal InputModel Model => _model;
        internal Transform CanvasRoot { get; private set; }

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
            Transform existing = _uiRoot.Find("EventSystem");
            if (existing != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.transform.SetParent(_uiRoot, false);
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }

        private GameObject EnsureInputCatcher()
        {
            Transform canvasTransform = _uiRoot.Find("Canvas");
            GameObject canvasObject;
            if (canvasTransform == null)
            {
                canvasObject = new GameObject("Canvas");
                canvasObject.transform.SetParent(_uiRoot, false);
                var canvas = canvasObject.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 0;
                var scaler = canvasObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = _referenceResolution;
                scaler.matchWidthOrHeight = _matchWidthOrHeight;
                canvasObject.AddComponent<GraphicRaycaster>();
            }
            else
            {
                canvasObject = canvasTransform.gameObject;
            }

            CanvasRoot = canvasObject.transform;

            Transform catcherTransform = canvasObject.transform.Find("InputCatcher");
            if (catcherTransform != null)
            {
                return catcherTransform.gameObject;
            }

            var catcher = new GameObject("InputCatcher");
            var rect = catcher.AddComponent<RectTransform>();
            catcher.transform.SetParent(canvasObject.transform, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var image = catcher.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0f);
            image.raycastTarget = true;
            return catcher;
        }
    }
}
