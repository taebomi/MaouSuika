using TBM.MaouSuika.Core.Scene;
using TBM.MaouSuika.Data;
using UnityEngine;

namespace TBM.MaouSuika.Scenes.Gameplay
{
    [CreateAssetMenu(fileName = "StandardModeTestPayloadSO",
        menuName = "TBM/Scenes/StandardMode/Test Payload")]
    public class StandardModeTestPayloadSO : SceneTestPayloadSO
    {
        [SerializeField] private MonsterLoadoutSO monsterLoadout;

        public override object CreatePayload()
        {
            return new GameplayPayload(monsterLoadout.ToData());
        }
    }
}