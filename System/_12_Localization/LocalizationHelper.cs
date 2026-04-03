using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.Localization
{
    public static class LocalizationHelper
    {
        private static LocalizationManager manager;
        private static LocalizationManager Manager
        {
            get
            {
                if (ReferenceEquals(manager, null))
                {
                    manager = GameManager.Instance.LocalizationManager;
                }

                return manager;
            }
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void InitializeOnSubsystemRegistration()
        {
            manager = null;
        }


        public static void ChangeLocale(string locale, Action onChanged)
        {
            Manager.ChangeLocaleAsync(locale, onChanged).Forget();
        }
    }
}