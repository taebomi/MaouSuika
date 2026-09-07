using System.Collections.Generic;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public interface ISoulOrbSpawner
    {
        SoulOrb Spawn(int tier, Vector3 position, bool hasLanded, SoulOrbSpawnMode spawnMode);
        SoulOrb Spawn(int tier, Vector3 position, int creationOrder, bool hasLanded, SoulOrbSpawnMode spawnMode);
        void Despawn(SoulOrb orb);
        IReadOnlyList<SoulOrb> ActiveOrbs { get; }
    }
}