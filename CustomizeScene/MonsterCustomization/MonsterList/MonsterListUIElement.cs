using System;
using DG.Tweening;
using SOSG.System.Display;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterListUIElement : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private MonsterListElementSO monsterListElementSO; // 스프라이트 데이터 저장되어 있음.

        [SerializeField] private Image monsterImage;
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image equippedTierImage;

        public MonsterInfo Data { get; private set; }

        private bool IsUnlocked => Data.IsUnlocked;
        private bool IsEquipped => Data.IsEquipped;

        private event Action<MonsterListUIElement> OnClicked;

        public void Initialize(Action<MonsterListUIElement> onClicked)
        {
            OnClicked = onClicked;
        }

        private void OnDestroy()
        {
            OnClicked = null;
        }

        public void Setup(MonsterInfo elementData)
        {
            Data = elementData;
            monsterImage.sprite = elementData.MonsterData.previewSprite;
            monsterImage.SetNativeSize();
            RefreshUI();
        }

        public void SetActive(bool value) => gameObject.SetActive(value);

        public void RefreshUI()
        {
            monsterImage.color = IsUnlocked ? Color.white : Color.black;

            if (IsEquipped)
            {
                equippedTierImage.sprite = monsterListElementSO.TierSpriteArr[Data.EquippedTier];
                equippedTierImage.enabled = true;
            }
            else
            {
                equippedTierImage.enabled = false;
            }
        }

        public void SetSelected(bool value) => outlineImage.sprite =
            value ? monsterListElementSO.OutlineSpriteArr[1] : monsterListElementSO.OutlineSpriteArr[0];

        public void JumpMonster()
        {
            if (!IsUnlocked)
            {
                return;
            }

            const float jumpDuration = 0.15f;
            const float jumpHeight = DisplayData.UI_PIXEL_PER_DOT * 10f;
            monsterImage.rectTransform.DOAnchorPosY(jumpHeight, jumpDuration)
                .From(new Vector2(0f, 0f))
                .SetEase(Ease.OutQuad)
                .SetLoops(2, LoopType.Yoyo)
                .Play();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            JumpMonster();
            OnClicked?.Invoke(this);
        }
    }
}