using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage.Suika.GameOver
{
    public class WarningArea : MonoBehaviour
    {
        private const double CheckingInterval = 0.5;

        [SerializeField] private SuikaCollection collection;

        [SerializeField] private BoxCollider2D areaColl;

        private Collider2D[] _collidedSuikaArr;
        private ContactFilter2D _suikaFilter;

        private bool _isChecking;
        public bool IsWarning { get; private set; }

        private CancellationTokenSource _checkingCts;

        public event Action WarningSet;
        public event Action WarningUnset;


        private void Awake()
        {
            _collidedSuikaArr = new Collider2D[3];
            _suikaFilter = new ContactFilter2D
            {
                layerMask = PhysicsCache.GetLayerMask(PhysicsCache.LayerName.Suika),
                useLayerMask = true,
            };
            IsWarning = false;
            _isChecking = false;
        }

        private void OnDisable()
        {
            StopCheck();
        }

        public void StartCheck()
        {
            if (_isChecking)
            {
                return;
            }

            _isChecking = true;
            _checkingCts?.Dispose();
            _checkingCts = new CancellationTokenSource();
            Check(_checkingCts.Token).Forget();
        }

        public void StopCheck()
        {
            if (_isChecking is false)
            {
                return;
            }

            _checkingCts.Cancel();
            _isChecking = false;
        }


        private async UniTask Check(CancellationToken ct)
        {
            while (ct.IsCancellationRequested is false)
            {
                var isWarning = IsGroundedSuikaExists();
                SetWarning(isWarning);
                await UniTask.Delay(TimeSpan.FromSeconds(CheckingInterval), cancellationToken: ct);
            }
        }

        private bool IsGroundedSuikaExists()
        {
            var collidedCount = areaColl.Overlap(_suikaFilter, _collidedSuikaArr);
            for (var i = 0; i < collidedCount; i++)
            {
                var suika = collection.GetSuika(_collidedSuikaArr[i].GetInstanceID());
                if (suika.PhysicsComponent.IsGrounded)
                {
                    return true;
                }
            }

            return false;
        }

        private void SetWarning(bool value)
        {
            if (IsWarning == value)
            {
                return;
            }

            IsWarning = value;
            if (value)
            {
                WarningSet?.Invoke();
            }
            else
            {
                WarningUnset?.Invoke();
            }
        }
    }
}