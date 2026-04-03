using System;
using System.Collections.Generic;
using TaeBoMi;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SOSG.Stage.Suika.Shooter
{
    public class SuikaShooterTouchManager : MonoBehaviour
    {
        [SerializeField] private SuikaShooterTouchController[] touchControllerArr;

        [SerializeField] private SuikaShooterTouchAreaSO[] touchAreaDataArr;
        private SuikaShooterTouchAreaSO _curTouchArea;

        private Dictionary<int, int> _touchedPlayerDict;

        private void Awake()
        {
            _touchedPlayerDict = new Dictionary<int, int>();
        }

        public void SetUp(int playerNum)
        {
            _curTouchArea = touchAreaDataArr[playerNum - 1];
            for (var playerIdx = 0; playerIdx < playerNum; playerIdx++)
            {
                touchControllerArr[playerIdx].SetScreenYInverted(_curTouchArea.playerTouchAreaArr[playerIdx].isYInverted);
            }
            
            if (EnhancedTouchSupport.enabled is false)
            {
                EnhancedTouchSupport.Enable();
            }

            Touch.onFingerDown += OnFingerDown;
            Touch.onFingerUp += OnFingerUp;
        }

        public void TearDown()
        {
            Touch.onFingerDown -= OnFingerDown;
            Touch.onFingerUp -= OnFingerUp;
        }
        private void OnFingerDown(Finger finger)
        {
            var playerIdx = _curTouchArea.GetPlayerNum(finger.screenPosition);
            _touchedPlayerDict.Add(finger.index, playerIdx);
            touchControllerArr[playerIdx].AddFinger(finger);
        }

        private void OnFingerUp(Finger finger)
        {
            _touchedPlayerDict.Remove(finger.index, out var playerIdx);
            touchControllerArr[playerIdx].RemoveFinger(finger);
        }
    }
}