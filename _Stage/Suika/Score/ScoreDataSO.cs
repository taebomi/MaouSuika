using CodeStage.AntiCheat.ObscuredTypes;
using SOSG.Stage.Suika.Combo;
using TaeBoMi;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    [CreateAssetMenu(fileName = "ScoreDataSO", menuName = "SOSG/Suika/Score/Data")]
    public class ScoreDataSO : ScriptableObject
    {
        [SerializeField] private int[] mergeScoreArr;
        [SerializeField] private float[] comboMultiplierArr;

        public int GetScore(int tier, ComboGrade comboGrade)
        {
            var mergeScore = mergeScoreArr[tier];
            var comboGradeIdx = TBMUtility.GetEnumIndex(comboGrade);
            var comboMultiplier = comboMultiplierArr[comboGradeIdx];
            return (int)(mergeScore * comboMultiplier);
        }
    }
}