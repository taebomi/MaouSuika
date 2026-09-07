namespace MaouSuika.Gameplay
{
    public readonly struct SoulOrbSetupData
    {
        public readonly int Tier;
        public readonly float Scale;
        public readonly int CreationOrder;
        public readonly bool HasLanded;
        public readonly SoulOrbSpawnMode SpawnMode;
        public readonly MonsterDataSO MonsterData;

        public SoulOrbSetupData(int tier, float scale,
            int creationOrder, bool hasLanded, SoulOrbSpawnMode spawnMode,
            MonsterDataSO monsterData)
        {
            Tier = tier;
            Scale = scale;
            CreationOrder = creationOrder;
            HasLanded = hasLanded;
            SpawnMode = spawnMode;
            MonsterData = monsterData;
        }
    }
}