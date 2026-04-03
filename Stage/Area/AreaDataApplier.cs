using System;
using DG.Tweening;
using FMODUnity;
using SOSG.Stage.Area;
using SOSG.System.Audio;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SOSG.Stage.Map
{
    public class AreaDataApplier : MonoBehaviour
    {
        [SerializeField] private StageAreaSO stageAreaSO;

        [SerializeField] private Light2D globalLight;

        private void Awake()
        {
            stageAreaSO.ActionOnAreaChanged += OnCurAreaChanged;
        }

        private void OnDestroy()
        {
            stageAreaSO.ActionOnAreaChanged -= OnCurAreaChanged;
        }

        public void Initialize(AreaData areaData)
        {
            ApplyAreaBgm(areaData.bgmRef);
            globalLight.intensity = areaData.globalLightIntensity;
        }

        private void OnCurAreaChanged(AreaData areaData)
        {
            ApplyAreaBgm(areaData.bgmRef);
            ApplyGlobalLight(areaData.globalLightIntensity);
        }
        

        private void ApplyAreaBgm(EventReference bgmRef)
        {
            AudioSystemHelper.PlayBgm(bgmRef);
        }

        private void ApplyGlobalLight(float intensity)
        {
            if (Math.Abs(intensity - globalLight.intensity) > 0.01f)
            {
                DOTween.To(() => globalLight.intensity, x => globalLight.intensity = x,
                        intensity, 1f)
                    .SetEase(Ease.InOutSine)
                    .Play();
            }
            else
            {
                globalLight.intensity = intensity;
            }
        }
    }
}