using DG.Tweening;
using SOSG.Stage.Area;
using SOSG.System.Audio;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SOSG.Area
{
    public class StageAreaDataApplier : MonoBehaviour
    {
        [SerializeField] private StageAreaManagerSO stageAreaManagerSO;

        [SerializeField] private Light2D globalLight;

        private void Awake()
        {
            stageAreaManagerSO.ActionOnAreaChanged += OnAreaChanged;
        }

        private void OnDestroy()
        {
            stageAreaManagerSO.ActionOnAreaChanged -= OnAreaChanged;
        }

        public void Initialize(AreaData areaData)
        {
            AudioSystemHelper.PlayBgm(areaData.bgmRef);
            globalLight.intensity = areaData.globalLightIntensity;
        }

        private void OnAreaChanged(AreaData areaData)
        {
            AudioSystemHelper.PlayBgm(areaData.bgmRef);
            if (Mathf.Abs(globalLight.intensity - areaData.globalLightIntensity) > 0.01f)
            {
                DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x,
                    areaData.globalLightIntensity, StageAreaManager.AreaChangeDuration).SetEase(Ease.OutSine).Play();
            }
        }
    }
}