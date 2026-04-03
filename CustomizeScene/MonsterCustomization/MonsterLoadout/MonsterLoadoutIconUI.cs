using System.Collections.Generic;
using SOSG.System.UI;
using SOSG.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using NotImplementedException = System.NotImplementedException;

namespace SOSG.Customization.Monster
{
    public class MonsterLoadoutIcon : MonoBehaviour, IModalUI
    {
        [field:SerializeField] public Canvas Canvas { get; private set; }
        public void OnCloseRequested()
        {
            throw new NotImplementedException();
        }

        [SerializeField] private MonsterLoadout loadout;

        [SerializeField] private MonsterLoadoutIconUIElement[] iconArr;
        public bool CanInteract => true;

        private void Awake()
        {
            foreach (var iconUIElement in iconArr)
            {
                iconUIElement.Initialize(loadout.OnIconClicked);
            }
        }

        public void OnOverlayClicked() => CancelMonsterEquip();

        public void ActivateBlackBackground()
        {
            // modalUI.Show();
        }

        public void DeactivateBlackBackground()
        {
            // modalUI.Hide();
        }

        public void UpdateIcons(List<MonsterInfo> equippedMonsterInfoArr)
        {
            for (var tier = 0; tier < iconArr.Length; tier++)
            {
                iconArr[tier].Set(equippedMonsterInfoArr[tier]);
            }
        }

        public void UpdateIcon(int tier, MonsterInfo monsterInfo) => iconArr[tier].Set(monsterInfo);

        private void CancelMonsterEquip() => loadout.OnEquipCanceled();
    }
}