using SOSG.System.PlayData;
using UnityEngine;

namespace SOSG.System.MonsterUnlock
{
    [CreateAssetMenu(fileName = "MonsterUnlockDataVarSO", menuName = "TaeBoMi/Play Data/Monster Unlock Data")]
    public class MonsterUnlockProgressVarSO : ScriptableObject
    {
        [SerializeField] private PlayDataManagerSO managerSO;
        
        public MonsterUnlockProgress Progress { get; private set; }

        public void Initialize(MonsterUnlockProgress progress)
        {
            Progress = progress;
        }

        public void Unlock(string id) => Progress.Unlock(id);

        public bool IsUnlocked(string id) => Progress.IsUnlocked(id);

        public void RequestSave() => managerSO.RequestSave();
    }
}