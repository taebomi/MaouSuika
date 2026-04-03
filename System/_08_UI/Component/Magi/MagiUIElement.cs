using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.PlayData;
using TMPro;
using UnityEngine;

namespace SOSG.UI
{
    public class MagiUIElement : MonoBehaviour
    {
        [SerializeField] private MagiSO magiSO;
        
        [SerializeField] private TMP_Text magiTmp;

        private void OnEnable()
        {
            OnMagiInitialized(magiSO.Magi);
            magiSO.OnMagiInitialized += OnMagiInitialized;
            magiSO.OnMagiChanged += OnMagiChanged;   
        }
        
        private void OnDisable()
        {
            magiSO.OnMagiInitialized -= OnMagiInitialized;
            magiSO.OnMagiChanged -= OnMagiChanged;
        }
        
        private void OnMagiInitialized(Magi magi)
        {
            magiTmp.text = $"{magi}";
        }
        
        private void OnMagiChanged(Magi magi)
        {
            magiTmp.text = $"{magi}";
        }
    }
}