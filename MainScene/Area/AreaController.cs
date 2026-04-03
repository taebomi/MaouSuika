using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Area;
using SOSG.System;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class AreaController : MonoBehaviour
    {
        [SerializeField] private StageAreaDataSO stageAreaDataSO;
        [SerializeField] private GamePlayStatisticsSO gamePlayStatisticsSO;

        [SerializeField] private TopScreenAreaController battleAreaController;
        [SerializeField] private BottomScreenAreaController gashaponAreaController;
        [SerializeField] private Light2D globalLight;

        private void Awake()
        {
             SceneSetUpHelper.AddTask(CreateRandomAreaAsync);
        }

        private async UniTask CreateRandomAreaAsync()
        {
            // Get Random Available Area
            var maxAreaIdx = gamePlayStatisticsSO.data.maxAreaIdx;
            var randomAreaIdx = Random.Range(0, maxAreaIdx + 1);
            var areaData = stageAreaDataSO.data.areaData[randomAreaIdx];

            globalLight.intensity = areaData.globalLightIntensity;
            await UniTask.WhenAll(battleAreaController.LoadArea(areaData), gashaponAreaController.LoadArea(areaData));
        }
    }
}