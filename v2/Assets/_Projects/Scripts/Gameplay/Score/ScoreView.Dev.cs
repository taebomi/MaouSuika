#if UNITY_EDITOR

using Sirenix.OdinInspector;

namespace MaouSuika.Gameplay
{
    public partial class ScoreView
    {
        [Button]
        private void DEV_UpdateScore(int score)
        {
            UpdateScore(score);
        }
    }
}
#endif