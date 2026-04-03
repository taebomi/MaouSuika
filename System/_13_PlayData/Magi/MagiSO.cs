using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(fileName = "MagiSO", menuName = "TaeBoMi/Play Data/Magi")]
    public class MagiSO : ScriptableObject
    {
        public Magi Magi { get; private set; }

        public Action<Magi> OnMagiInitialized;
        public Action<Magi> OnMagiChanged;

        public void Initialize(Magi magi)
        {
            Magi = magi;
            OnMagiInitialized?.Invoke(magi);
        }

        public void Add(int value)
        {
            Magi += value;
            OnMagiChanged?.Invoke(Magi);
        }
        
        public void Subtract(int value)
        {
            Magi -= value;
            OnMagiChanged?.Invoke(Magi);
        }
    }
}