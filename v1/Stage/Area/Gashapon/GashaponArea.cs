using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace SOSG.Stage.Area
{
    public class GashaponArea : MonoBehaviour
    {
        private Renderer[] _rendererArr;
        private Light2D[] _lightArr;
        private float[] _lightOriIntensityArr;

        private void Awake()
        {
            _rendererArr = GetComponentsInChildren<Renderer>();
            _lightArr = GetComponentsInChildren<Light2D>();
            _lightOriIntensityArr = new float[_lightArr.Length];
            SaveOriginalLightIntensity();
        }

        private void SaveOriginalLightIntensity()
        {
            for (var i = 0; i < _lightArr.Length; i++)
            {
                _lightOriIntensityArr[i] = _lightArr[i].intensity;
            }
        }

        public void SetMaterial(Material mat)
        {
            foreach (var ren in _rendererArr)
            {
                ren.material = mat;
            }
        }

        public void SetLightIntensity(float value)
        {
            for (var idx = 0; idx < _lightArr.Length; idx++)
            {
                _lightArr[idx].intensity = _lightOriIntensityArr[idx] * value;
            }
        }
    }
}