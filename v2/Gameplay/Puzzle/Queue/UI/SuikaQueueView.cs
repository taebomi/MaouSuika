using System.Collections.Generic;
using TBM.Extensions;
using TBM.MaouSuika.Data;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaQueueView : MonoBehaviour
    {
        [SerializeField] private SuikaQueueSlot[] slots;

        private SuikaTierDataTable _suikaTierDataTable;

        public void Initialize(SuikaTierDataTable tierDataTable)
        {
            _suikaTierDataTable = tierDataTable;
        }

        public void Refresh(IReadOnlyList<int> items, int visibleCount)
        {
            var count = Mathf.Min(slots.Length, items.Count, visibleCount);
            for (var i = 0; i < slots.Length; i++)
            {
                if (i < count)
                {
                    var tier = items[i];
                    var data = _suikaTierDataTable[tier].MonsterData;
                    slots[i].Set(data.icon, data.suikaColor);
                    slots[i].SetActive(true);
                }
                else
                {
                    slots[i].SetActive(false);
                }
            }
        }
    }
}