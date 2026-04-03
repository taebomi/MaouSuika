using System.Collections.Generic;
using SOSG.Monster;
using SOSG.Monster.Overlord;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

namespace SOSG.Customization.Monster
{
    public class MonsterSummoner : MonoBehaviour
    {
        [SerializeField] private PlayableDirector director;
        [SerializeField] private PlayableAsset idleAsset;
        [SerializeField] private PlayableAsset allUnlockedAsset;
        [SerializeField] private PlayableAsset summonAsset;
        [SerializeField] private PlayableAsset summonSucceedAsset;

        [SerializeField] private ParticleSystem[] summonEffectParticleArr;
        [SerializeField] private GameObject screenFadingEffect;

        [SerializeField] private MonsterController monsterController;
        [SerializeField] private OverlordAnimationController overlordController;

        private List<MonsterGrade> _availableGradeList;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        public void Activate(bool allUnlocked)
        {
            gameObject.SetActive(true);
            if (allUnlocked)
            {
                SetAllUnlockedState();
            }
            else
            {
                SetIdleState();
            }
        }

        public void Deactivate()
        {
            director.Stop();
            gameObject.SetActive(false);
        }

        #region Playable State

        public void SetIdleState()
        {
            monsterController.gameObject.SetActive(false);
            overlordController.gameObject.SetActive(true);

            director.extrapolationMode = DirectorWrapMode.Loop;
            director.Play(idleAsset);
        }

        public void SetAllUnlockedState()
        {
            director.extrapolationMode = DirectorWrapMode.Loop;
            director.Play(allUnlockedAsset);
        }

        public void SetSummonState(MonsterDataSO summonedMonsterData)
        {
            screenFadingEffect.SetActive(true);
            overlordController.gameObject.SetActive(true);
            monsterController.gameObject.SetActive(false);

            _availableGradeList = MonsterGradeCache.CreateMonsterGradeListInstance();
            _availableGradeList.Remove(summonedMonsterData.grade);

            director.extrapolationMode = DirectorWrapMode.None;
            director.Play(summonAsset);
        }

        public void SetSummonSucceedState(MonsterDataSO summonedMonsterData)
        {
            StopAllSummonEffects();

            monsterController.gameObject.SetActive(true);
            monsterController.Set(summonedMonsterData);
            monsterController.RotateJump();
            monsterController.SetAnimationLoop(true);
            overlordController.gameObject.SetActive(false);

            director.extrapolationMode = DirectorWrapMode.None;
            director.Play(summonSucceedAsset);
        }

        #endregion


        #region Timeline Signal Callback

        public void ActivateAllSummonEffects()
        {
            foreach (var particle in summonEffectParticleArr)
            {
                particle.Play(true);
            }
        }

        public void StopSummonEffect()
        {
            var grade = _availableGradeList[Random.Range(0, _availableGradeList.Count)];
            _availableGradeList.Remove(grade);
            
            var idx = MonsterGradeCache.GetIndex(grade);
            summonEffectParticleArr[idx].Stop(true);
        }

        private void StopAllSummonEffects()
        {
            foreach (var particle in summonEffectParticleArr)
            {
                particle.Stop(true);
            }
        }

        public void DeactivateFadingCanvas() => screenFadingEffect.SetActive(false);

        #endregion
    }
}