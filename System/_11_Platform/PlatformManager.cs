using Cysharp.Threading.Tasks;
using SOSG.Ads;
using SOSG.GPGS;
using SOSG.System;
using SOSG.UGS;
using UnityEngine;

public class PlatformManager : MonoBehaviour, IGameSetUp, IGameSetUpAsync, IGameTearDown
{
    [SerializeField] private UGSManager ugsManager; // unity gaming services
    [SerializeField] private GPGSManager gpgsManager; // google play store only
    [SerializeField] private AdsManager adsManager; // admob


    public void SetUp()
    {
        ugsManager.SetUp();
        gpgsManager.SetUp();
        adsManager.SetUp();
    }

    public async UniTask SetUpAsync()
    {
        await ugsManager.SetUpAsync();
    }

    public void TearDown()
    {
        gpgsManager.TearDown();
        adsManager.TearDown();
        ugsManager.TearDown();
    }
}
