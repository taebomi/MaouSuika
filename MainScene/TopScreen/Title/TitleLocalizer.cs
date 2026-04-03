using System;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.Setting;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class TitleLocalizer : MonoBehaviour
    {
        [SerializeField] private IntEventSO titleTimingEventSO;

        [SerializeField] private SerializedDictionary<string, Title> titleDict;
        
        private RectTransform _rt;
        
        private Title _curTitle;

        private void Awake()
        {
            _rt = transform as RectTransform;
            SceneSetUpHelper.AddTask(SetUp);
        }

        private void OnEnable()
        {
            titleTimingEventSO.OnEventRaised += OnTitleTimingEventRaised;
        }

        private void OnDisable()
        {
            titleTimingEventSO.OnEventRaised -= OnTitleTimingEventRaised;
        }

        private void SetUp()
        {
            if (titleDict.TryGetValue( SettingDataHelper.InterfaceSetting.locale, out var title) is false)
            {
                return;
            }
            _curTitle = title;
            if (MainSceneManager.SkipIntro)
            {
                _curTitle.SetActive(true);
                _rt.anchoredPosition = Vector2.zero;
                _curTitle.PlayShine().Forget();
                _curTitle.PlayImpact().Forget();
            }
            else
            {
                _curTitle.SetActive(true);
                _rt.anchoredPosition = new Vector2(0f, -480f);
            }
        }

        private void OnTitleTimingEventRaised(int timing)
        {
            switch (timing)
            {
                case 4:
                    MoveUp();
                    break;
                case 10:
                    _curTitle.PlayShine().Forget();
                    _curTitle.PlayImpact().Forget();
                    break;
            }
        }

        private void MoveUp()
        {
            _rt.DOAnchorPosY(0f, 0.3333f).SetEase(Ease.OutBack)
                .Play()
                .OnComplete(
                    () => _curTitle.PlayImpact().Forget());
        }

        public void ChangeLocale(string localeCode)
        {
            if (titleDict.TryGetValue(localeCode, out var title) is false)
            {
                return;
            }

            _curTitle.SetActive(false);
            _curTitle = title;
            _curTitle.SetActive(true);
            _curTitle.PlayShine().Forget();
            _curTitle.PlayImpact().Forget();
        }
    }
}