using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SOSG.System.UI
{
    public class ModalUIController : MonoBehaviour
    {
        [SerializeField] private UISortingOrderController sortingOrderController;

        [SerializeField] private ModalOverlay overlay;

        private readonly LinkedList<IModalUI> _modalUIList = new();


        public void Show(IModalUI added)
        {
            overlay.SetActive(true);
            sortingOrderController.Add(overlay);
            _modalUIList.AddLast(added);
        }

        public void Hide(IModalUI removed)
        {
            var curNode = _modalUIList.Last;
            while (curNode is not null)
            {
                if (curNode.Value == removed)
                {
                    _modalUIList.Remove(curNode);
                    sortingOrderController.Remove(overlay);
                    if(_modalUIList.Count == 0)
                    {
                        overlay.SetActive(false);
                    }
                    return;
                }

                curNode = curNode.Previous;
            }
        }

        public void OnOverlayClicked()
        {
            var topModalUI = _modalUIList.Last;
            if (topModalUI is not null)
            {
                topModalUI.Value.OnOverlayClicked();
            }
        }
    }
}