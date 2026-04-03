using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(fileName = "SplitScreenModePlayDataVarSO", menuName = "SOSG/Play Data/Split Screen Mode")]
    public class SplitScreenModePlayDataVarSO : ScriptableObject
    {
        public int playerNum = 2;
        public PlayerLoadoutString[] playerLoadoutStringArr =
        {
            PlayerLoadoutString.CreateDefaultInstance(),
            PlayerLoadoutString.CreateDefaultInstance()
        };
    }
}