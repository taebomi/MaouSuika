using UnityEngine;

namespace SOSG.System.UI
{
    public static class UIHelper
    {
        private static UIManager manager;
        private static UIManager Manager
        {
            get
            {
                if (ReferenceEquals(manager, null))
                {
                    manager = GameManager.Instance.UIManager;
                }

                return manager;
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            manager = null;
        }

        public static void ShowUI(BaseUI subject)
        {
            Manager.ShowUI(subject);
        }

        public static void HideUI(BaseUI subject)
        {
            Manager.HideUI(subject);
        }

        public static void AddSortingOrder(BaseUI subject)
        {
            Manager.AddSortingOrder(subject);
        }

        public static void RemoveSortingOrder(BaseUI subject)
        {
            Manager.RemoveSortingOrder(subject);
        }

        public static void ShowModalOverlay(IModalUI subject)
        {
            Manager.ShowModalUI(subject);
        }

        public static void HideModalOverlay(IModalUI subject)
        {
            Manager.HideModalUI(subject);
        }
    }
}