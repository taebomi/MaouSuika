using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Scene;
using TBM.MaouSuika.Data;
using UnityEngine.AddressableAssets;

namespace TBM.MaouSuika.Scenes.Gameplay
{
    public class GameplayPayload : ScenePayload
    {
        public MonsterLoadoutData MonsterLoadoutData;
        
        public GameplayPayload(MonsterLoadoutData monsterLoadoutData)
        {
            MonsterLoadoutData = monsterLoadoutData;
        }
    }
}