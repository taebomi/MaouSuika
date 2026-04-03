using UnityEngine;

namespace SOSG.System.UI
{
    public class UIManager : MonoBehaviour
    {
        [field: SerializeField] public UISortingOrderController SortingOrderController { get; private set; }
        [field: SerializeField] public ModalUIController ModalUIController { get; private set; }

        public void ShowUI(BaseUI subject)
        {
            if (subject is IModalUI modalUI)
            {
                ShowModalUI(modalUI);
            }
            AddSortingOrder(subject);
        }

        public void HideUI(BaseUI subject)
        {
            RemoveSortingOrder(subject);
            if (subject is IModalUI modalUI)
            {
                HideModalUI(modalUI);
            }
        }

        public void AddSortingOrder(BaseUI subject)
        {
            SortingOrderController.Add(subject);
        }

        public void RemoveSortingOrder(BaseUI subject)
        {
            SortingOrderController.Remove(subject);
        }

        public void ShowModalUI(IModalUI subject)
        {
            ModalUIController.Show(subject);
        }

        public void HideModalUI(IModalUI subject)
        {
            ModalUIController.Hide(subject);
        }
    }
}