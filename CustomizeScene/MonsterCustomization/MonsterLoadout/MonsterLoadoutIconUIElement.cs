using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterLoadoutIconUIElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image iconImage;

        public event Action<MonsterInfo> OnIconClicked;

        private MonsterInfo _info;

        public void Initialize(Action<MonsterInfo> onIconClicked)
        {
            OnIconClicked = onIconClicked;
        }

        private void OnDestroy()
        {
            OnIconClicked = null;
        }

        public void Set(MonsterInfo info)
        {
            _info = info;
            var data = info.MonsterData;
            outlineImage.color = data.capsuleColor;
            backgroundImage.color = data.capsuleColor;
            iconImage.sprite = data.icon;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnIconClicked?.Invoke(_info);
        }
    }
}