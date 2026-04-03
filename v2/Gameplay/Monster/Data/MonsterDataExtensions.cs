using System;
using System.Linq;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Gameplay.Monster;

namespace TBM.MaouSuika.Data
{
    public static class MonsterDataExtensions
    {
        public static MonsterLoadout ToRuntimeLoadout(this MonsterLoadoutData data, MonsterDatabaseSO db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            if (data == null || data.ids == null || data.ids.Count != GameRule.Puzzle.Suika.TIER_COUNT)
            {
                Logger.Warning($"Invalid Data. Return Default Loadout.");
                return db.CreateDefaultLoadout();
            }

            var monsters = new MonsterDataSO[GameRule.Puzzle.Suika.TIER_COUNT];
            for (var tier = 0; tier < GameRule.Puzzle.Suika.TIER_COUNT; tier++)
            {
                var id = data.ids[tier];
                var monster = db.GetMonsterData(id);

                if (monster == null)
                {
                    Logger.Warning($"Monster with id {id} is not found. Return Default Loadout.");
                    return db.CreateDefaultLoadout();
                }

                monsters[tier] = monster;
            }

            return new MonsterLoadout(monsters);
        }

        public static MonsterLoadoutData ToData(this MonsterLoadoutSO so)
        {
            if (so == null) throw new ArgumentNullException(nameof(so));

            if (so.Count != GameRule.Puzzle.Suika.TIER_COUNT) throw new InvalidOperationException($"Invalid Data");

            return new MonsterLoadoutData
            {
                ids = so.monsterData.Select(data => data.id).ToList()
            };
        }
    }
}