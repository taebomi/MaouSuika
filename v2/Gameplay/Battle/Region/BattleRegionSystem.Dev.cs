#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;


namespace TBM.MaouSuika.Gameplay.Battle
{
    public partial class BattleRegionSystem
    {
        [BoxGroup("Dev"), DisableInEditorMode]
        [SerializeField] private int dev_curRegionIndex;

        [BoxGroup("Dev")]
        [Button(Expanded = true), DisableInEditorMode]
        private void Dev_TransitionByScore(int curRegionIndex)
        {
            if (_transitionRoutine != null) return;
            
            var entries = progression.Dev_Entries;
            if (entries == null || entries.Length == 0)
            {
                Logger.Error("Progression is not set.");
                return;
            }
            
            var maxIndex = entries.Length + 1;
            curRegionIndex = curRegionIndex < 0 ? maxIndex : curRegionIndex >= maxIndex ? 0 : curRegionIndex;

            dev_curRegionIndex = curRegionIndex;
            var region = dev_curRegionIndex == 0 ? progression.InitialBattle : entries[dev_curRegionIndex - 1].battleRegion;
            _transitionRoutine = StartCoroutine(TransitionTo(region));
        }

        [ButtonGroup("Dev/Transition")]
        [Button(Expanded = true), DisableInEditorMode]

        private void Dev_TransitionToPrev()
        {
            Dev_TransitionByScore(dev_curRegionIndex - 1);
        }

        [ButtonGroup("Dev/Transition")]
        [Button(Expanded = true), DisableInEditorMode]

        private void Dev_TransitionToNext()
        {
            Dev_TransitionByScore(dev_curRegionIndex + 1);
        }
    }
}


#endif