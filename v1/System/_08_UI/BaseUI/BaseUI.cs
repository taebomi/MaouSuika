using UnityEngine;

namespace SOSG.System.UI
{
    public class BaseUI : MonoBehaviour
    {
        public Canvas Canvas { get; private set; }

        private void Awake()
        {
            Canvas = GetComponent<Canvas>();
            AwakeAfter();
        }

        protected virtual void AwakeAfter()
        {
            
        }
        
        public void SetSortingOrder(int sortingOrder)
        {
            Canvas.sortingOrder = sortingOrder;
        }
    }
}