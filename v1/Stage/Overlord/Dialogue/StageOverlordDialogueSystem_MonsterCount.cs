using System;
using System.Collections.Generic;
using SOSG.Monster;
using SOSG.System.Localization;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SOSG.System.Dialogue
{
    public partial class StageOverlordDialogueSystem
    {
        [FormerlySerializedAs("monsterIdDB")]
        [Header("Monster Count")]
        [SerializeField] private MonsterDB monsterDB;
        [SerializeField] private IntEventSO monsterSpawnEvent;

        private Dictionary<MonsterGrade, int> _remainedMonsterCountDict;

        private const int MinCommonGradeRemainedCount = 40;
        private const int MaxCommonGradeRemainedCount = 60;
        private const int MinUncommonGradeRemainedCount = 20;
        private const int MaxUncommonGradeRemainedCount = 40;
        private const int MinRareGradeRemainedCount = 10;
        private const int MaxRareGradeRemainedCount = 20;
        private const int MinEpicGradeRemainedCount = 1;
        private const int MaxEpicGradeRemainedCount = 1;

        private void AwakeMonsterCount()
        {
            _remainedMonsterCountDict = new Dictionary<MonsterGrade, int>(4);
            foreach (MonsterGrade grade in Enum.GetValues(typeof(MonsterGrade)))
            {
                _remainedMonsterCountDict.Add(grade, GetRandomRemainedMonsterCount(grade));
            }

            monsterSpawnEvent.OnEventRaised += OnMonsterSpawned;
        }

        private void OnDestroyMonsterCount()
        {
            monsterSpawnEvent.OnEventRaised -= OnMonsterSpawned;
        }


        private void OnMonsterSpawned(int level)
        {
            var grade = MonsterController.GetGrade(level);
            _remainedMonsterCountDict[grade]--;
            if (_remainedMonsterCountDict[grade] > 0)
            {
                return;
            }
            
            _remainedMonsterCountDict[grade] = GetRandomRemainedMonsterCount(grade);
            RequestLineMonsterCount(grade);
        }
        
        private void RequestLineMonsterCount(MonsterGrade grade)
        {
            var randomMonsterID = monsterDB.GetRandomMonsterId(grade);
            var key = $"{randomMonsterID}-spawn";
            const int randomIdxCount = 5;
            RequestRandomLine(LocalizationTableName.Stage_System, key, randomIdxCount, StageOverlordLineType.Normal);
        }

        private static int GetRandomRemainedMonsterCount(MonsterGrade grade)
        {
            return grade switch
            {
                MonsterGrade.Common => Random.Range(MinCommonGradeRemainedCount, MaxCommonGradeRemainedCount),
                MonsterGrade.Uncommon => Random.Range(MinUncommonGradeRemainedCount, MaxUncommonGradeRemainedCount),
                MonsterGrade.Rare => Random.Range(MinRareGradeRemainedCount, MaxRareGradeRemainedCount),
                MonsterGrade.Epic => Random.Range(MinEpicGradeRemainedCount, MaxEpicGradeRemainedCount),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}