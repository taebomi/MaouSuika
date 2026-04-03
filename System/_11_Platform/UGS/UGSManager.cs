using System;
using Cysharp.Threading.Tasks;
using SOSG.UGS.IAP;
using SOSG.Utility;
using TaeBoMi;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;

namespace SOSG.UGS
{
    public class UGSManager : MonoBehaviour
    {
        [SerializeField] private IAPManager iapManager;

        public void SetUp()
        {
            iapManager.SetUp();
        }

        public async UniTask SetUpAsync()
        {
            TBMUtility.Log("# UGS Manager - Initialize Started");
            try
            {
                var options = new InitializationOptions().SetEnvironmentName("production");
                await UnityServices.InitializeAsync(options);
                OnSuccess();
            }
            catch (Exception ex)
            {
                OnFailed(ex);
            }
        }

        public void TearDown()
        {
            iapManager.TearDown();
        }
        private void OnSuccess()
        {
            TBMUtility.Log("# UGS Manager - Initialization Success");
            iapManager.Initialize();
        }

        private void OnFailed(Exception ex)
        {
            TBMUtility.Log("# UGS Manager - Initialization Failed");
            TBMUtility.Log($"## Exception - {ex.Message}");
            iapManager.Initialize();
        }

    }
}