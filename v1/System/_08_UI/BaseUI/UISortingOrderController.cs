using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.System.UI
{
    public class UISortingOrderController : MonoBehaviour
    {
        public int SortingOrder { get; private set; } = 0;

        private readonly LinkedList<BaseUI> _uiList = new();

        public void Add(BaseUI added)
        {
            SortingOrder++;
            _uiList.AddLast(added);
            added.SetSortingOrder(SortingOrder);
        }

        public void Remove(BaseUI removed)
        {
            var curNode = _uiList.Last;
            while (curNode is not null)
            {
                if (curNode.Value == removed)
                {
                    _uiList.Remove(curNode);
                    SortingOrder--;
                    return;
                }

                curNode = curNode.Previous;
            }
        }
    }
}