using System;
using SOSG.System.Display;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SOSG.System.UI
{
    public class PressButtonChildrenAdjuster : MonoBehaviour
    {
        private const float PressedOffset = DisplayData.UI_PIXEL_PER_DOT * 2;

        private Button _btn;
        private bool _isDowned;

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        public void OnPointerDown(BaseEventData _)
        {
            if (_btn.interactable is false)
            {
                return;
            }
            
            for (var idx = 0; idx < transform.childCount; idx++)
            {
                var childRect = transform.GetChild(idx) as RectTransform;
                childRect.anchoredPosition -= new Vector2(0f, PressedOffset);
            }

            _isDowned = true;
        }

        public void OnPointerUp(BaseEventData _)
        {
            if (_btn.interactable is false)
            {
                return;
            }
            
            for (var idx = 0; idx < transform.childCount; idx++)
            {
                var childRect = transform.GetChild(idx) as RectTransform;
                childRect.anchoredPosition += new Vector2(0f, PressedOffset);
            }

            _isDowned = false;
        }
    }
}