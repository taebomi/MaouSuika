using SOSG.System;
using UnityEngine;

namespace SOSG.Stage
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField] private CameraVarSO bottomScreenCamSO;
        [SerializeField] private CameraVarSO topScreenCamSO;

        [SerializeField] private Camera gashaponCam;
        [SerializeField] private Camera battleCam;

        private void Awake()
        {
            topScreenCamSO.value = battleCam;
            bottomScreenCamSO.value = gashaponCam;
        }
    }
}