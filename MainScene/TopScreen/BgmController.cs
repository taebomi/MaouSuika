using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using SOSG.System;
using SOSG.System.Audio;
using SOSG.System.Scene;
using UnityEngine;

namespace SOSG.MainScene
{
    public class BgmController : MonoBehaviour
    {
        [SerializeField] private IntEventSO bgmTimingEventSO;
        [SerializeField] private TitleLocalizer titleLocalizer;

        [SerializeField] private EventReference bgmRef;

        public void PlayBgm(bool skipIntro)
        {
            if (skipIntro is false)
            {
                AudioSystemHelper.PlayBgm(bgmRef, OnMarkerChanged);
            }
            else
            {                
                AudioSystemHelper.PlayBgm(bgmRef);
                AudioSystemHelper.ChangeBgmTimelinePosition(64000);
            }
        }

        private void OnMarkerChanged(string markerName)
        {
            switch (markerName)
            {
                case "0":
                    bgmTimingEventSO.RaiseEvent(0);
                    break;
                case "1":
                    bgmTimingEventSO.RaiseEvent(1);
                    break;
                case "2":
                    bgmTimingEventSO.RaiseEvent(2);
                    break;
                case "3":
                    bgmTimingEventSO.RaiseEvent(3);
                    break;
                case "4":
                    bgmTimingEventSO.RaiseEvent(4);
                    break;
                case "5":
                    bgmTimingEventSO.RaiseEvent(5);
                    break;
            }
        }
    }
}