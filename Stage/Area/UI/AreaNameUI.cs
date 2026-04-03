using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.Stage.Area;
using SOSG.System;
using SOSG.System.Scene;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;

namespace SOSG.Stage.Map
{
    public class AreaNameUI : MonoBehaviour
    {
        [SerializeField] private StageAreaSO stageAreaSO;

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TextMeshProUGUI text;

        private StringTable _areaNameTable;

        private string _curAreaName;
        private CancellationTokenSource _fadingCts;

        private void Awake()
        {
            canvasGroup.alpha = 0f;

            stageAreaSO.ActionOnAreaChanged += OnAreaChanged;

            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            _fadingCts?.CancelAndDispose();

            stageAreaSO.ActionOnAreaChanged -= OnAreaChanged;
        }

        private async UniTask SetUpAsync()
        {
            _areaNameTable = await LocalizationSettings.StringDatabase.GetTableAsync("Area");
        }

        private void OnAreaChanged(AreaData areaData)
        {
            _fadingCts?.CancelAndDispose();
            _fadingCts = new CancellationTokenSource();

            var newAreaName = areaData.areaName;
            if (IsAreaNameChanged(newAreaName) is false)
            {
                return;
            }

            SetText(newAreaName);
            ShowText(_fadingCts.Token).Forget();
        }

        private bool IsAreaNameChanged(string newAreaName) => _curAreaName != newAreaName;

        private void SetText(string areaName)
        {
            var entry = _areaNameTable.GetEntry(areaName);
            var localizedValue = entry.Value;
            text.text = $"─ {localizedValue} ─";
        }

        private async UniTaskVoid ShowText(CancellationToken ct)
        {
            const float fadingDuration = 0.5f;
            await canvasGroup.DOFade(1f, fadingDuration).From(0f).SetEase(Ease.InOutSine).Play().WithCancellation(ct);
            const double waitDuration = 3f;
            await UniTask.Delay(TimeSpan.FromSeconds(waitDuration), cancellationToken: ct);
            await canvasGroup.DOFade(0f, fadingDuration).SetEase(Ease.InOutSine).Play().WithCancellation(ct);
        }
    }
}