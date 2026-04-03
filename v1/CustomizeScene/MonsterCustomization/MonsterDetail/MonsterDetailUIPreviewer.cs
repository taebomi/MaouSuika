using System;
using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using SOSG.Area;
using SOSG.Monster;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.Customization.Monster
{
    public class MonsterDetailUIPreviewer : MonoBehaviour
    {
        [SerializeField] private MonsterController[] monsterArr;

        [SerializeField] private Transform biomeContainer;
        [SerializeField] private SerializedDictionary<Biome, GameObject> biomeDict;

        private int _frontMonsterIdx;
        private GameObject _curBiome;

        private CancellationTokenSource _aniCts;

        private const float PreviewWidth = 6;

        private void Awake()
        {
            _frontMonsterIdx = 0;
            _curBiome = null;
            foreach (Transform childTr in biomeContainer)
            {
                childTr.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (monsterArr[_frontMonsterIdx].transform.localPosition.x > PreviewWidth)
            {
                var backIdx = (_frontMonsterIdx + monsterArr.Length - 1) % monsterArr.Length;
                monsterArr[_frontMonsterIdx].SetLocalPosition(
                    new Vector3(monsterArr[backIdx].transform.localPosition.x - PreviewWidth, 0f));
                _frontMonsterIdx = (_frontMonsterIdx + 1) % monsterArr.Length;
            }
        }

        public void Set(MonsterDataSO monsterData, bool isUnlocked)
        {
            SetBiome(monsterData.habitat);
            SetMonster(monsterData);
            SetUnlockState(isUnlocked);
            AnimateMonster().Forget();
        }

        private void SetUnlockState(bool value)
        {
            var color = value ? Color.white : Color.black;
            foreach (var monster in monsterArr)
            {
                monster.SetColor(color);
            }
        }

        private void SetMonster(MonsterDataSO monsterData)
        {
            _frontMonsterIdx = 0;
            var centerIdx = monsterArr.Length / 2;
            for (var idx = 0; idx < monsterArr.Length; idx++)
            {
                monsterArr[idx].Set(monsterData);
                monsterArr[idx].SetLocalPosition(new Vector3(PreviewWidth * (centerIdx - idx), 0f));
            }
        }

        private void SetBiome(Biome biome)
        {
            if (_curBiome)
            {
                _curBiome.SetActive(false);
            }

            if (biomeDict.TryGetValue(biome, out var biomeObj))
            {
                biomeObj.SetActive(true);
                _curBiome = biomeObj;
            }
        }

        private async UniTaskVoid AnimateMonster()
        {
            _aniCts?.CancelAndDispose();
            _aniCts = new CancellationTokenSource();

            var isMoving = Random.value < 0.5f;
            while (_aniCts.IsCancellationRequested is false)
            {
                if (isMoving)
                {
                    await AnimateMonsterMove(_aniCts.Token);
                    isMoving = false;
                }
                else
                {
                    await AnimateMonsterIdle(_aniCts.Token);
                    isMoving = true;
                }
            }
        }

        private async UniTask AnimateMonsterIdle(CancellationToken ct)
        {
            const float minDuration = 3f;
            const float maxDuration = 6f;
            var duration = Random.Range(minDuration, maxDuration);
            foreach (var monster in monsterArr)
            {
                monster.SetIdle();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
        }

        private async UniTask AnimateMonsterMove(CancellationToken ct)
        {
            const float minDuration = 3f;
            const float maxDuration = 6f;
            var duration = Random.Range(minDuration, maxDuration);
            foreach (var monster in monsterArr)
            {
                monster.SetMove();
            }

            await UniTask.Delay(TimeSpan.FromSeconds(duration), cancellationToken: ct);
        }
    }
}