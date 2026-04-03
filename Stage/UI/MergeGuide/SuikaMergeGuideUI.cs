using System;
using DG.Tweening;
using SOSG.System.Setting;
using SOSG.UI;
using UnityEngine;

namespace SOSG.Stage.UI
{
    public class SuikaMergeGuideUI : MonoBehaviour
    {
        [SerializeField] private GashaponDataSO gashaponDataSO;

        [SerializeField] private RectTransform rt;
        [SerializeField] private MonsterIconUIElement[] elementArr;

        private const float RtWidth = 75f;
        private const float AniSpeed = RtWidth / 0.25f;

        private void Awake()
        {
            for (var i = 0; i < elementArr.Length; i++)
            {
                elementArr[i].Set(gashaponDataSO.monsterCapsuleDataArr[i]);
            }

            var isEnabled = SettingDataHelper.InterfaceSetting.suikaMergeGuide;
            rt.anchoredPosition = isEnabled ? new Vector2(0f, 0f) : new Vector2(RtWidth, 0f);
            gameObject.SetActive(isEnabled);
        }

        private void OnEnable()
        {
            SettingDataHelper.InterfaceSettingChanged += OnInterfaceSettingChanged;
        }

        private void OnDisable()
        {
            SettingDataHelper.InterfaceSettingChanged -= OnInterfaceSettingChanged;
        }
        
        private void OnInterfaceSettingChanged(InterfaceSetting interfaceSetting)
        {
            DOTween.Kill(GetInstanceID());
            if (interfaceSetting.suikaMergeGuide)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }
        
        private void Show()
        {
            gameObject.SetActive(true);
            rt.DOAnchorPosX(0f, AniSpeed).SetEase(Ease.OutSine).SetTarget(GetInstanceID()).SetSpeedBased(true)
                .SetUpdate(true).Play();
        }

        private void Hide()
        {
            rt.DOAnchorPosX(rt.sizeDelta.x, AniSpeed).SetEase(Ease.InSine).SetTarget(GetInstanceID())
                .SetSpeedBased(true).SetUpdate(true).OnComplete(() => gameObject.SetActive(false)).Play();
        }
    }
}