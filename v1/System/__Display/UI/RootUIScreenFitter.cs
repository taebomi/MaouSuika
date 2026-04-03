using System;
using TaeBoMi;
using UnityEngine;

namespace SOSG.System.Display
{
    public class RootUIScreenFitter : MonoBehaviour
    {
        [SerializeField] private Type type;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();

            DisplayData.PillarBoxEnabled += FitUI;
        }

        private void Start()
        {
            FitUI(DisplayData.IsPillarBoxOn);
        }

        private void OnDestroy()
        {
            DisplayData.PillarBoxEnabled -= FitUI;
        }

        private void FitUI(bool pillarBox)
        {
            var (anchorMinY, anchorMaxY) = type switch
            {
                Type.All => (0f, 1f),
                Type.Bottom => (DisplayData.BOTTOM_SCREEN_MIN_Y, DisplayData.BOTTOM_SCREEN_MAX_Y),
                Type.Dialogue => (DisplayData.DIALOGUE_SCREEN_MIN_Y, DisplayData.DIALOGUE_SCREEN_MAX_Y),
                Type.Top => (DisplayData.TOP_SCREEN_MIN_Y, DisplayData.TOP_SCREEN_MAX_Y),
                _ => throw new ArgumentOutOfRangeException()
            };


            if (pillarBox is false)
            {
                _rt.anchorMin = new Vector2(0f, anchorMinY);
                _rt.anchorMax = new Vector2(1f, anchorMaxY);
            }
            else
            {
                _rt.anchorMin = new Vector2(DisplayData.MinX, anchorMinY);
                _rt.anchorMax = new Vector2(DisplayData.MaxX, anchorMaxY);
            }

            _rt.offsetMin = new Vector2(0f, 0f);
            _rt.offsetMax = new Vector2(0f, 0f);
        }

        private enum Type
        {
            All,
            Top,
            Dialogue,
            Bottom,
        }
    }
}