using System;
using SOSG.System.Display;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.UI
{
    public class PressButtonWithImage : Button
    {
        [field:SerializeField] public Image ElementImage { get; private set; }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            ElementImage.rectTransform.anchoredPosition = state switch
            {
                SelectionState.Normal or SelectionState.Selected or SelectionState.Highlighted
                    or SelectionState.Disabled => Vector2.zero,
                SelectionState.Pressed => new Vector2(0, -DisplayData.UI_PIXEL_PER_DOT),
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
        }
    }
}