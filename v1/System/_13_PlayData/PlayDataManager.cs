using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.PlayData
{
    public class PlayDataManager : MonoBehaviour
    {
        [SerializeField] private PlayDataManagerSO managerSO;

        [SerializeField] private PlayStatisticsManager playStatisticsManager;
        [SerializeField] private PlayerLoadoutManager loadoutManager;
        [SerializeField] private MonsterUnlockManager monsterUnlockManager;
        [SerializeField] private MagiManager magiManager;

        private ES3Settings _es3Settings;

        private bool _isInitialized;

        private const string FilePath = "SOSG.playdata";
        private const string EncryptionKey = "o)VS2}s&[z}eIpdiSA[l'm/Q!;O!Rw";

        public void SetUp()
        {
        }

        public async UniTask SetUpAsync()
        {
            _es3Settings = new ES3Settings(ES3.Location.Cache, ES3.CompressionType.Gzip);
            _es3Settings = new ES3Settings(FilePath, ES3.EncryptionType.AES, EncryptionKey, _es3Settings);
            ES3.CacheFile(_es3Settings);

            playStatisticsManager.Initialize(_es3Settings);
            magiManager.Initialize(_es3Settings);

            await loadoutManager.Initialize(_es3Settings);
            monsterUnlockManager.Initialize(_es3Settings);
            _isInitialized = true;
        }

        private void OnEnable()
        {
            managerSO.OnSaveRequested += Save;

        }

        private void OnDisable()
        {
            managerSO.OnSaveRequested -= Save;

        }

        private void Save()
        {
            if (_isInitialized is false)
            {
                return;
            }

            playStatisticsManager.Save(_es3Settings);
            loadoutManager.Save(_es3Settings);
            monsterUnlockManager.Save(_es3Settings);
            magiManager.Save(_es3Settings);
            ES3.StoreCachedFile(_es3Settings);
        }

        private void OnApplicationQuit() => Save();
    }
}