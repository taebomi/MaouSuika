using TMPro;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public interface IMainScoreCounter
    {
        Transform TargetTr { get; }
        TMP_Text TargetTmp { get; }
        int DisplayScore { get; }
        Color BaseColor { get; }

        void SetScore(int value);
    }
}