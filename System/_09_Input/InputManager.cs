using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

namespace SOSG.System.Input
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField] private GameObject touchBlocker;

        private readonly HashSet<int> _blockedCallerIds = new();

        private void Awake()
        {
            TouchSimulation.Enable();
            EnhancedTouchSupport.Enable();
        }

        private void OnEnable()
        {
            InputHelper.InputBlockRequested += BlockInput;
            InputHelper.InputUnblockRequested += UnblockInput;
        }

        private void OnDisable()
        {
            InputHelper.InputBlockRequested -= BlockInput;
            InputHelper.InputUnblockRequested -= UnblockInput;
        }

        private void BlockInput(int callerId)
        {
            if (_blockedCallerIds.Contains(callerId))
            {
                return;
            }

            _blockedCallerIds.Add(callerId);
            touchBlocker.SetActive(true);
        }

        private void UnblockInput(int callerId)
        {
            if (!_blockedCallerIds.Contains(callerId))
            {
                return;
            }

            _blockedCallerIds.Remove(callerId);
            if (_blockedCallerIds.Count == 0)
            {
                touchBlocker.SetActive(false);
            }
        }
    }
}