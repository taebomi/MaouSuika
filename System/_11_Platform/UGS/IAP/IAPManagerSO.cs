using System;
using UnityEngine;

namespace SOSG.UGS.IAP
{
    [CreateAssetMenu(fileName = "IAPManagerSO", menuName = "TaeBoMi/IAP/IAPManagerSO", order = 1200)]
    public class IAPManagerSO : ScriptableObject
    {
        public bool IsInitialized { get; private set; }
        public Action<bool> OnInitializeStateChanged;

        public Action<string, Action<PurchaseResult>> OnPurchaseRequested;
        public Func<string, bool> FuncIsPurchased;
        public Func<Action<bool>, bool> OnRestoreTransactionRequested;


        public void Initialize()
        {
            IsInitialized = false;
        }

        public void SetInitializeState(bool value)
        {
            IsInitialized = value;
            OnInitializeStateChanged?.Invoke(value);
        }

        public void PurchaseProduct(string id, Action<PurchaseResult> successCallback)
        {
            if (OnPurchaseRequested is null)
            {
                successCallback.Invoke(new PurchaseResult(id, PurchaseResult.Result.Unknown));
                return;
            }

            OnPurchaseRequested.Invoke(id, successCallback);
        }

        public bool IsPurchased(string productId)
        {
            return FuncIsPurchased?.Invoke(productId) ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultCallback">지원하는 스토어일 경우 결과보고 받음.</param>
        /// <returns>false 반환 시 지원하지 않거나 iap 초기화되지 않음.</returns>
        public bool RestoreTransactions(Action<bool> resultCallback)
        {
            return OnRestoreTransactionRequested?.Invoke(resultCallback) ?? false;
        }
    }
}