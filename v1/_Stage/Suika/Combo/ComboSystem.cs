using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace SOSG.Stage.Suika.Combo
{
    public class ComboSystem : MonoBehaviour
    {
        [SerializeField] private ComboEffectPoolSO pool;

        public ObscuredInt CurCombo { get; private set; }
        public ComboGrade CurComboGrade { get; private set; }

        private bool _hasMerged;

        public event Action ComboFailed; // todo 마왕 대사, 스코어

        private void Awake()
        {
            _hasMerged = false;
            CurCombo = 0;
        }

        public void OnShot()
        {
            if (_hasMerged is false)
            {
                ComboFailed?.Invoke();
                CurCombo = 0;
            }

            _hasMerged = false;
        }

        public void OnMerged(SuikaMerger.MergedInfo info)
        {
            _hasMerged = true;
            CurCombo++;

            var curCombo = (int)CurCombo;
            CurComboGrade = ComboUtility.GetGrade(curCombo);
            if (CurComboGrade is not ComboGrade.None)
            {
                var effect = pool.Get(info.Pos);
                effect.Set(curCombo);
            }
        }
    }
}