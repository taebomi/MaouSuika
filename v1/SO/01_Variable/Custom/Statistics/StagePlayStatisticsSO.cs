using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(fileName = "StagePlayStatisticsSO", menuName = "TaeBoMi/Play Data/Statistics/Stage Play")]
    public class StagePlayStatisticsSO : ScriptableObject
    {
        [field:SerializeField] public StagePlayStatistics Data { get; private set; }

        [SerializeField] private GamePlayStatisticsSO gamePlayStatisticsSO;

        public void Initialize()
        {
            Data = new StagePlayStatistics();
        }

        public void IncreaseCreatedMonsterCount(int level)
        {
            Data.createdMonsterCount[level]++;
        }

        public void IncreaseShotCount()
        {
            Data.shotCount++;
        }

        public void SetScore(int score)
        {
            Data.score = score;
        }

        public void IncreaseStageIdx()
        {
            Data.lastAreaIdx++;
        }

        public void ApplyToGamePlayStatistics()
        {
            gamePlayStatisticsSO.Add(Data);
        }
    
    }
}