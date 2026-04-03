using System;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;

namespace SOSG.System.Scene
{
    public class LoadingMonster : MonoBehaviour
    {
        [SerializeField] private MonsterDB monsterDB;

        [SerializeField] private MonsterAnimator[] monsterAnimatorArr;
        [SerializeField] private SpriteRenderer[] monsterSrArr;

        private MonsterDataSO _monsterData;
        private int _monsterNum;

        private const float MonsterMinColor = 0f;

        public async UniTask SetUpAsync()
        {
            await SelectRandomMonster();
            SetUpMonster();
        }

        public void TearDown()
        {
            Addressables.Release(_monsterData);
            _monsterData = null;
        }

        public void UpdateProgress(float progressRatio)
        {
            var monsterNumInverse = 1f / _monsterNum;
            var fullNum = (int)(progressRatio * _monsterNum);
            for (var idx = 0; idx < fullNum; idx++)
            {
                monsterSrArr[idx].color = Color.white;
            }

            if (fullNum == _monsterNum)
            {
                return;
            }

            var remainedProgressRatio = progressRatio - fullNum * monsterNumInverse;
            var lastFilledRatio = remainedProgressRatio * _monsterNum;

            const float colorInterval = 0.25f;
            const float colorMultiplier = 1 - MonsterMinColor;
            var colorSize = colorMultiplier * lastFilledRatio;
            var colorSizeByInterval = colorSize - colorSize % colorInterval;
            var colorValue = MonsterMinColor + colorSizeByInterval;
            monsterSrArr[fullNum].color = new Color(colorValue, colorValue, colorValue);
        }

        private async UniTask SelectRandomMonster()
        {
            var randomGrade = GetRandomMonsterGrade();
            var monsterId = monsterDB.GetRandomMonsterId(randomGrade);
            _monsterData = await MonsterDataSOLoader.LoadMonsterDataSOAsync(monsterId);
        }

        private void SetUpMonster()
        {
            _monsterNum = GetMonsterNum();
            ApplyMonsterData();
            ArrangeMonsters();
        }

        private int GetMonsterNum()
        {
            return _monsterData.grade switch
            {
                MonsterGrade.Common => 5,
                MonsterGrade.Uncommon => 3,
                MonsterGrade.Rare => 2,
                MonsterGrade.Epic => 1,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void ApplyMonsterData()
        {
            for (var i = 0; i < _monsterNum; i++)
            {
                var monsterAnimator = monsterAnimatorArr[i];
                monsterAnimator.SetUp(_monsterData);
                monsterAnimator.Move();
                monsterAnimator.gameObject.SetActive(true);
                monsterSrArr[i].color = new Color(MonsterMinColor, MonsterMinColor, MonsterMinColor);
            }

            for (var i = _monsterNum; i < monsterAnimatorArr.Length; i++)
            {
                monsterAnimatorArr[i].gameObject.SetActive(false);
            }
        }

        private void ArrangeMonsters()
        {
            const float dist = 1f;
            var halfMonsterNum = _monsterNum / 2;
            var startIdxAccumulation = _monsterNum % 2 == 1 ? halfMonsterNum : halfMonsterNum - 0.5f;
            for (var i = 0; i < _monsterNum; i++)
            {
                var monsterAnimator = monsterAnimatorArr[i];
                monsterAnimator.transform.position =
                    new Vector3((dist + _monsterData.xMaxLength * 2) * (i - startIdxAccumulation), 0, 0);
            }
        }

        private MonsterGrade GetRandomMonsterGrade()
        {
            var random = Random.value;
            return random switch
            {
                <= 0.5f => MonsterGrade.Common,
                <= 0.825f => MonsterGrade.Uncommon,
                <= 0.95f => MonsterGrade.Rare,
                _ => MonsterGrade.Epic
            };
        }
    }
}