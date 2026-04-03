using System;
using SOSG.System.MonsterUnlock;
using TaeBoMi;
using UnityEngine;

namespace SOSG.System.PlayData
{
    public class MonsterUnlockManager : MonoBehaviour
    {
        [SerializeField] private MonsterDB monsterDB;
        
        [SerializeField] private MonsterUnlockProgressVarSO monsterUnlockProgressVarSO;
        
        private const string SaveKey = "MonsterUnlock";

        public void Initialize(ES3Settings es3Settings)
        {
            TBMUtility.Log($"# Monster Unlock Manager - Initialize");
            var data = LoadUnlockData(es3Settings);
            monsterUnlockProgressVarSO.Initialize(data);
        }

        private MonsterUnlockProgress LoadUnlockData(ES3Settings es3Settings)
        {
            TBMUtility.Log($"# Monster Unlock Manager - Load Unlock Data");
            
            MonsterUnlockProgress progress;
            try
            {
                progress = ES3.Load(SaveKey, new MonsterUnlockProgress(), es3Settings);
            }
            catch (Exception ex)
            {
                TBMUtility.LogError($"## Exception Occurred: {ex.Message}");
                progress =new MonsterUnlockProgress();
            }

            return progress;
        }

        public void Save(ES3Settings es3Settings)
        {
            TBMUtility.Log($"# Monster Unlock Manager - Save Unlock Data");
            
            ES3.Save(SaveKey, monsterUnlockProgressVarSO.Progress, es3Settings);
        }
    }
}