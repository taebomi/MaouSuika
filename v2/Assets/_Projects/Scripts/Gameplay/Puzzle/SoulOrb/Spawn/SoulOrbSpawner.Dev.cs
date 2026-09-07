using Sirenix.OdinInspector;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class SoulOrbSpawner
    {
        [Button]
        public void DEV_Spawn(int tier, Vector3 position = default)
        {
            Spawn(tier, position, false, SoulOrbSpawnMode.Field);
        }
    }
}