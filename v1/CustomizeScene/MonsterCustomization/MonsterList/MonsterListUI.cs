using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterListUI : MonoBehaviour
    {
        [SerializeField] private MonsterList monsterList;

        [SerializeField] private Transform containerTr;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private List<MonsterListUIElement> elementList;

        public MonsterInfo SelectedMonsterInfo => _selectedListElement?.Data;

        private List<MonsterInfo> _curMonsterInfoList;
        private MonsterListUIElement _selectedListElement;

        private Action<MonsterInfo> _onElementClicked;

        public void Initialize(Action<MonsterInfo> onElementClicked)
        {
            _selectedListElement = null;
            _onElementClicked = onElementClicked;
            foreach (var element in elementList)
            {
                element.Initialize(OnElementClicked);
            }
        }

        public void ShowList(List<MonsterInfo> monsterInfoList)
        {
            if (_curMonsterInfoList == monsterInfoList)
            {
                return;
            }

            _curMonsterInfoList = monsterInfoList;

            // 개수 부족 시 추가 생성
            if (monsterInfoList.Count > elementList.Count)
            {
                var diff = monsterInfoList.Count - elementList.Count;
                for (var i = 0; i < diff; i++)
                {
                    var newElement = Instantiate(elementList[0], containerTr);
                    newElement.Initialize(OnElementClicked);
                    elementList.Add(newElement);
                }
            }

            // 적용
            for (var i = 0; i < monsterInfoList.Count; i++)
            {
                var monsterInfo = monsterInfoList[i];
                elementList[i].Setup(monsterInfo);
                elementList[i].SetActive(true);
            }

            // 남는 것들 비활성화
            for (var i = monsterInfoList.Count; i < elementList.Count; i++)
            {
                elementList[i].SetActive(false);
            }

            // 선택된 요소 존재하면 해제
            _selectedListElement?.SetSelected(false);
            _selectedListElement = null;
        }

        public void RefreshUI()
        {
            for (var idx = 0; idx < _curMonsterInfoList.Count; idx++)
            {
                elementList[idx].RefreshUI();
            }
        }

        public void Select(MonsterInfo monsterInfo)
        {
            var listElement = elementList.Find(element => element.Data == monsterInfo);
            if (listElement == null)
            {
                Debug.LogError($"# MonsterListUI.Select: {monsterInfo.MonsterData.name} not found in the list");
                return;
            }

            _selectedListElement?.SetSelected(false);
            _selectedListElement = listElement;
            _selectedListElement.SetSelected(true);
            _selectedListElement.JumpMonster();
            ScrollTo(listElement.transform as RectTransform);
        }


        private void ScrollTo(RectTransform target)
        {
            scrollRect.content.ForceUpdateRectTransforms();
            scrollRect.viewport.ForceUpdateRectTransforms();

            var scrollHeight = scrollRect.content.rect.height;
            var targetHeight = target.rect.height;
            var targetY = Mathf.Abs(target.localPosition.y);
            if (targetY < targetHeight)
            {
                targetY = 0f;
            }
            else if (targetY > scrollHeight - targetHeight)
            {
                targetY = scrollHeight;
            }

            var targetNormalizedY = targetY / scrollHeight;

            scrollRect.verticalNormalizedPosition = 1f - targetNormalizedY;
            scrollRect.content.ForceUpdateRectTransforms();
        }

        private void OnElementClicked(MonsterListUIElement clickedListElement)
        {
            // 같은 것 클릭 시 nothing to do
            if (_selectedListElement == clickedListElement)
            {
                return;
            }

            _selectedListElement?.SetSelected(false);
            _selectedListElement = clickedListElement;
            _selectedListElement.SetSelected(true);
            _onElementClicked?.Invoke(clickedListElement.Data);
        }
    }
}