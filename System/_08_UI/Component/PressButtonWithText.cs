using System;
using SOSG.System.Display;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.UI
{
    public class PressButtonWithText : Button
    {
        [SerializeField] private TMP_Text tmp;
        [SerializeField] private Sprite normalSprite, disabledSprite;

        public void SetPosition(Vector2 pos)
        {
            ((RectTransform)transform).anchoredPosition = pos;
        }

        public void SetText(string text)
        {
            tmp.text = text;
        }

        public void SetDisable(bool value)
        {
            if (value)
            {
                transition = Transition.None;
                image.sprite = disabledSprite;
                tmp.rectTransform.anchoredPosition =
                    new Vector2(tmp.rectTransform.anchoredPosition.x, -DisplayData.UI_PIXEL_PER_DOT);
            }
            else
            {
                transition = Transition.SpriteSwap;
                image.sprite = normalSprite;
                tmp.rectTransform.anchoredPosition = new Vector2(tmp.rectTransform.anchoredPosition.x, 0f);
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            if (transition is Transition.None)
            {
                return;
            }

            tmp.rectTransform.anchoredPosition = state switch
            {
                SelectionState.Normal or SelectionState.Selected or SelectionState.Highlighted
                    or SelectionState.Disabled => new Vector2(tmp.rectTransform.anchoredPosition.x, 0f),
                SelectionState.Pressed => new Vector2(tmp.rectTransform.anchoredPosition.x, -DisplayData.UI_PIXEL_PER_DOT),
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}