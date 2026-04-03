using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(menuName = "TaeBoMi/Play Data/Statistics/Game Play", fileName = "GamePlayStatisticsSO", order = 1100)]
    public class GamePlayStatisticsSO : ScriptableObject
    {
        public GamePlayStatistics data;

        [SerializeField] private PlayDataManagerSO playDataManagerSO;

        public void Initialize(GamePlayStatistics statistics)
        {
            data = statistics;
        }

        public void Add(StagePlayStatistics stagePlayStatistics)
        {
            data.Add(stagePlayStatistics);
        }

        public void Save() => playDataManagerSO.RequestSave();
    }
}