using System;
using UnityEngine;

namespace SOSG.System
{
    public class WaitIndicator : MonoBehaviour
    {
        [SerializeField] private GameObject ui;

        private void OnEnable()
        {
            WaitIndicatorHelper.WaitIndicatorActiveRequested += SetActive;
        }
        
        private void OnDisable()
        {
            WaitIndicatorHelper.WaitIndicatorActiveRequested -= SetActive;
        }

        public void SetActive(bool value)
        {
            ui.SetActive(value);
        }
    }
}