using System;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.System.Display
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerScreenRatioFitter : MonoBehaviour
    {
        private CanvasScaler _canvasScaler;

        private void Awake()
        {
            _canvasScaler = GetComponent<CanvasScaler>();
            
            DisplayData.SceenRatioChanged += FitToDisplay;
        }

        private void OnDestroy()
        {
            DisplayData.SceenRatioChanged -= FitToDisplay;
        }

        private void Start()
        {
            FitToDisplay();
        }

        private void FitToDisplay()
        {
            _canvasScaler.matchWidthOrHeight =  DisplayData.ScreenRatio <= DisplayData.UI_CANVAS_RATIO
                    ? 1f
                    : 0f;
        }
    }
}