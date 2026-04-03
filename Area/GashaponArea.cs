using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SOSG.Area
{
    public class GashaponArea : MonoBehaviour
    {
        private Renderer[] rendererArr;
        private Light2D[] lightArr;
        private float[] _lightOriginalIntensityArr;

        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            
            _destroyCts = new CancellationTokenSource();
            CacheComponents();
            SaveOriginalLightIntensity(lightArr);
        }

        private void CacheComponents()
        {
            rendererArr = GetComponentsInChildren<Renderer>();
            lightArr = GetComponentsInChildren<Light2D>();
            _lightOriginalIntensityArr = new float[lightArr.Length];
        }
        
        private void SaveOriginalLightIntensity(IReadOnlyList<Light2D> lightArr)
        {
            for (var i = 0; i < lightArr.Count; i++)
            {
                _lightOriginalIntensityArr[i] = lightArr[i].intensity;
            }
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
        }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void SetMaterial(Material mat)
        {
            foreach (var rend in rendererArr)
            {
                rend.material = mat;
            }
        }

        public void SetLightIntensity(float intensity)
        {
            for (var i = 0; i < lightArr.Length; i++)
            {
                lightArr[i].intensity = _lightOriginalIntensityArr[i] * intensity;
            }
        }
    }
}