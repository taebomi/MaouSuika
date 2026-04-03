using System;
using UnityEngine;

namespace SOSG.System.Display
{
    public class CameraDisplayFitter : MonoBehaviour
    {
        [SerializeField] private CameraType cameraType;
        
        private Camera _camera;
        private CameraDisplayFitterBase _fitter;
        
        private float _orthoSize;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _orthoSize = _camera.orthographicSize;

            DisplayData.SceenRatioChanged += FitCameraSize;
        }

        private void OnDestroy()
        {
            DisplayData.SceenRatioChanged -= FitCameraSize;
        }

        private void Start()
        {
            _fitter = cameraType switch
            {
                CameraType.Top => new TopCameraDisplayFitter(),
                CameraType.Bottom => new BottomCameraDisplayFitter(),
                _ => _fitter
            };
            FitCameraSize();
        }

        private void FitCameraSize()
        {
            _camera.orthographicSize = _fitter.GetAccumulatedCameraSize(_orthoSize, DisplayData.ScreenRatio);
        }

        private enum CameraType
        {
            Top,
            Bottom,
        }
    }
}