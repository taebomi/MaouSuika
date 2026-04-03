using System;
using Cysharp.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace TBM.MaouSuika.Core.UGS
{
    public class UGSManager : CoreManager<UGSManager>
    {
        public async UniTask InitializeAsync()
        {
            var initSuccess = await InitializeUnityServicesAsync();
        }

        public void Deinitialize()
        {
            throw new System.NotImplementedException();
        }

        private async UniTask<bool> InitializeUnityServicesAsync()
        {
            try
            {
                await UnityServices.InitializeAsync();
                return true;
            }
            catch (ServicesInitializationException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}