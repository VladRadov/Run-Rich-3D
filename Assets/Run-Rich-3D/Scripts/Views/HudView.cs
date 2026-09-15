using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich3D.Views
{
    public sealed class HudView : MonoBehaviour
    {
        private readonly Subject<Unit> _actionClicked = new Subject<Unit>();

        private Font _font;
        private Texture2D _buttonTexture;
        private Texture2D _retryTexture;
        private Texture2D _dollarTexture;

        private RectTransform _runRoot;
        private RectTransform _resultRoot;
        private Text _levelLabel;
        private Text _wealthLabel;
        private Text _resultTitle;
        private Text _resultSubtitle;
        private Text _actionLabel;
        private RawImage _actionImage;
        private bool _built;

        internal IObservable<Unit> ActionClicked => _actionClicked;

        internal void Bind(Font font, Texture2D buttonTexture, Texture2D retryTexture, Texture2D dollarTexture)
        {
            _font = font;
            _buttonTexture = buttonTexture;
            _retryTexture = retryTexture;
            _dollarTexture = dollarTexture;
            EnsureBuilt();
        }

        internal void ShowRun(string levelText, int wealth)
        {
            EnsureBuilt();
            _runRoot.gameObject.SetActive(true);
            _resultRoot.gameObject.SetActive(false);
            _levelLabel.text = levelText;
            SetWealth(wealth);
        }

        internal void ShowResult(string title, string subtitle, string actionLabel, bool isWin)
        {
            EnsureBuilt();
            _runRoot.gameObject.SetActive(false);
            _resultRoot.gameObject.SetActive(true);
            _resultTitle.text = title;
            _resultTitle.color = isWin
                ? new Color(1f, 0.86f, 0.22f)
                : new Color(1f, 0.42f, 0.32f);
            _resultSubtitle.text = subtitle;
            _actionLabel.text = actionLabel;
            _actionImage.texture = isWin ? _buttonTexture : _retryTexture;
            _actionImage.color = isWin
                ? new Color(0.32f, 0.86f, 0.38f)
                : new Color(1f, 0.48f, 0.28f);
        }

        internal void SetWealth(int wealth)
        {
            if (_wealthLabel == null)
            {
                return;
            }

            _wealthLabel.text = wealth.ToString();
        }

        internal void SetLevel(string levelText)
        {
            if (_levelLabel == null)
            {
                return;
            }

            _levelLabel.text = levelText;
        }

        private void OnDestroy()
        {
            _actionClicked.OnCompleted();
            _actionClicked.Dispose();
        }

        private void EnsureBuilt()
        {
            if (_built)
            {
                return;
            }

            var root = (RectTransform)transform;
            Stretch(root);

            _runRoot = CreatePanel("RunHud", root);
            Stretch(_runRoot);

            _levelLabel = CreateText(
                "LevelLabel",
                _runRoot,
                42,
                FontStyle.Bold,
                TextAnchor.UpperCenter,
                new Vector2(0.5f, 1f),
                new Vector2(0.5f, 1f),
                new Vector2(0f, -72f),
                new Vector2(720f, 80f));

            var wealthRoot = CreatePanel("Wealth", _runRoot);
            wealthRoot.anchorMin = new Vector2(0.5f, 1f);
            wealthRoot.anchorMax = new Vector2(0.5f, 1f);
            wealthRoot.pivot = new Vector2(0.5f, 1f);
            wealthRoot.anchoredPosition = new Vector2(0f, -150f);
            wealthRoot.sizeDelta = new Vector2(720f, 140f);

            if (_dollarTexture != null)
            {
                var dollar = CreateChild("Dollar", wealthRoot);
                var dollarImage = dollar.gameObject.AddComponent<RawImage>();
                dollarImage.texture = _dollarTexture;
                dollarImage.raycastTarget = false;
                dollar.anchorMin = new Vector2(0.5f, 0.5f);
                dollar.anchorMax = new Vector2(0.5f, 0.5f);
                dollar.pivot = new Vector2(1f, 0.5f);
                dollar.anchoredPosition = new Vector2(-90f, 0f);
                dollar.sizeDelta = new Vector2(84f, 84f);
            }

            _wealthLabel = CreateText(
                "WealthLabel",
                wealthRoot,
                96,
                FontStyle.Bold,
                TextAnchor.MiddleLeft,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(20f, 0f),
                new Vector2(280f, 140f));

            _resultRoot = CreatePanel("ResultPanel", root);
            Stretch(_resultRoot);
            var dimmer = _resultRoot.gameObject.AddComponent<Image>();
            dimmer.color = new Color(0f, 0f, 0f, 0.58f);
            dimmer.raycastTarget = true;

            _resultTitle = CreateText(
                "ResultTitle",
                _resultRoot,
                64,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 220f),
                new Vector2(900f, 140f));

            _resultSubtitle = CreateText(
                "ResultSubtitle",
                _resultRoot,
                36,
                FontStyle.Normal,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0f, 80f),
                new Vector2(860f, 90f));
            _resultSubtitle.color = new Color(1f, 1f, 1f, 0.9f);

            var action = CreateChild("ActionButton", _resultRoot);
            action.anchorMin = new Vector2(0.5f, 0.5f);
            action.anchorMax = new Vector2(0.5f, 0.5f);
            action.pivot = new Vector2(0.5f, 0.5f);
            action.anchoredPosition = new Vector2(0f, -280f);
            action.sizeDelta = new Vector2(520f, 150f);
            _actionImage = action.gameObject.AddComponent<RawImage>();
            _actionImage.raycastTarget = true;
            var button = action.gameObject.AddComponent<Button>();
            button.targetGraphic = _actionImage;
            var navigation = button.navigation;
            navigation.mode = Navigation.Mode.None;
            button.navigation = navigation;
            button.onClick.AddListener(() => _actionClicked.OnNext(Unit.Default));

            _actionLabel = CreateText(
                "ActionLabel",
                action,
                44,
                FontStyle.Bold,
                TextAnchor.MiddleCenter,
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                Vector2.zero,
                new Vector2(480f, 120f));

            _resultRoot.gameObject.SetActive(false);
            _built = true;
        }

        private Text CreateText(
            string name,
            RectTransform parent,
            int size,
            FontStyle style,
            TextAnchor alignment,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 position,
            Vector2 sizeDelta)
        {
            var rect = CreateChild(name, parent);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = sizeDelta;

            var text = rect.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = size;
            text.fontStyle = style;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            var outline = rect.gameObject.AddComponent<Outline>();
            outline.effectColor = new Color(0f, 0f, 0f, 0.55f);
            outline.effectDistance = new Vector2(2f, -2f);
            return text;
        }

        private static RectTransform CreatePanel(string name, RectTransform parent)
        {
            return CreateChild(name, parent);
        }

        private static RectTransform CreateChild(string name, RectTransform parent)
        {
            var child = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)child.transform;
            rect.SetParent(parent, false);
            return rect;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
