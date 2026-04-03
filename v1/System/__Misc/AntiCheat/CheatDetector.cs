using System;
using CodeStage.AntiCheat.Detectors;
using UnityEngine;

namespace SOSG.System.Cheat
{
    public class CheatDetector : MonoBehaviour
    {
        private void Start()
        {
            ObscuredCheatingDetector.StartDetection();
        }

        private void OnDestroy()
        {
            ObscuredCheatingDetector.Dispose();
        }

        /// <summary>
        /// inspector event
        /// </summary>
        public void OnCheatDetected()
        {
            Application.Quit();
        }
    }
}