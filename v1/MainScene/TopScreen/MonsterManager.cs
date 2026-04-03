using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using SOSG.Monster;
using SOSG.Monster.Overlord;
using SOSG.System;
using SOSG.System.Audio;
using SOSG.System.PlayData;
using SOSG.System.Scene;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private IntEventSO titleTimingEventSO;
        [SerializeField] private PlayerLoadoutVarSO loadoutVarSO;

        [SerializeField] private OverlordAnimationController overlordAni;
        [SerializeField] private Animator[] aniArr;
        [SerializeField] private MonsterAnimator[] monsterDataComponentArr;

        [SerializeField] private EventReference jumpSfx;

        private const double JumpDuration = 0.5;
        private const double JumpRotateDuration = 1; 

        private void Awake()
        {
            SceneSetUpHelper.AddTask(SetUp);
        }

        private void OnEnable()
        {
            titleTimingEventSO.OnEventRaised += OnTitleTimingEventRaised;
        }

        private void OnDisable()
        {
            titleTimingEventSO.OnEventRaised -= OnTitleTimingEventRaised;
        }

        private void OnDestroy()
        {
            titleTimingEventSO.OnEventRaised -= OnTitleTimingEventRaised;
        }

        private void SetUp()
        {
            overlordAni.SetDirection(TaeBoMiCache.TwoDirection.Right);

            var monsterLoadout = loadoutVarSO.monsterLoadout.Data;
            var monsterXPos = 13.5f;
            for (var tier = 0; tier < monsterLoadout.Length; tier++)
            {
                var monsterXLength = monsterLoadout[tier].xMaxLength;
                var monsterPos = new Vector3(monsterXPos - monsterXLength, 0f);
                monsterDataComponentArr[tier].SetUp(monsterLoadout[tier]);
                monsterDataComponentArr[tier].transform.localPosition = monsterPos;
                monsterXPos -= monsterXLength * 2f;
            }

            if (MainSceneManager.SkipIntro)
            {
                transform.localPosition = Vector3.zero;
                JumpRepeat().Forget();
                SetMove(true);
            }
            else
            {
                transform.localPosition = new Vector3(-50f, 0f);
                SetMove(false);
            }
        }

        private void OnTitleTimingEventRaised(int timing)
        {
            switch (timing)
            {
                case 3:
                    transform.DOMoveX(-50f, 0.3f).SetEase(Ease.OutBack).Play();
                    break;
                case 10:
                    SetMove(true);
                    JumpRepeat().Forget();
                    break;
            }
        }

        private void SetMove(bool value)
        {
            overlordAni.SetMove(value);
            foreach (var ani in aniArr)
            {
                ani.SetBool(AnimatorCache.IsMove, value);
            }
        }

        private async UniTaskVoid JumpRepeat()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: destroyCancellationToken);
            while (destroyCancellationToken.IsCancellationRequested is false)
            {
                overlordAni.Play(OverlordAnimationController.AniType.Summon);
                await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: destroyCancellationToken);
                var random = Random.value;
                var repeatCount = Random.Range(1, 4);

                Func<UniTask> jumpAction = random switch
                {
                    < 0.2f => Jump1,
                    < 0.4f => JumpRotate1,
                    < 0.55f => Jump2,
                    < 0.7f => JumpRotate2,
                    < 0.85f => Jump3,
                    _ => JumpRotate3
                };
                for (var i = 0; i < repeatCount; i++)
                {
                    await UniTask.Create(jumpAction);
                }

                await UniTask.Delay(TimeSpan.FromSeconds(Random.Range(3f, 7f)),
                    cancellationToken: destroyCancellationToken);
            }
        }


        private async UniTask Jump1()
        {
            AudioSystemHelper.PlaySfx(jumpSfx);
            foreach (var ani in aniArr)
            {
                ani.SetTrigger(AnimatorCache.JumpTrigger);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(JumpDuration + 0.25), cancellationToken: destroyCancellationToken);
        }

        private async UniTask JumpRotate1()
        {
            AudioSystemHelper.PlaySfx(jumpSfx);
            foreach (var ani in aniArr)
            {
                ani.SetTrigger(AnimatorCache.RotateJumpTrigger);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(JumpRotateDuration + 0.25),
                cancellationToken: destroyCancellationToken);
        }

        private async UniTask Jump2()
        {
            foreach (var ani in aniArr)
            {
                AudioSystemHelper.PlaySfx(jumpSfx);
                ani.SetTrigger(AnimatorCache.JumpTrigger);
                await UniTask.Delay(TimeSpan.FromSeconds(0.1), cancellationToken: destroyCancellationToken);
            }
        }

        private async UniTask JumpRotate2()
        {
            foreach (var ani in aniArr)
            {
                AudioSystemHelper.PlaySfx(jumpSfx);
                ani.SetTrigger(AnimatorCache.RotateJumpTrigger);
                await UniTask.Delay(TimeSpan.FromSeconds(0.2), cancellationToken: destroyCancellationToken);
            }
        }

        private async UniTask Jump3()
        {
            var idx = 0;
            while (idx < aniArr.Length)
            {
                aniArr[idx].SetTrigger(AnimatorCache.JumpTrigger);
                idx += 2;
            }
            
            AudioSystemHelper.PlaySfx(jumpSfx);
            await UniTask.Delay(TimeSpan.FromSeconds(0.25), cancellationToken: destroyCancellationToken);

            idx = 1;
            while (idx < aniArr.Length)
            {
                aniArr[idx].SetTrigger(AnimatorCache.JumpTrigger);
                idx += 2;
            }
            
            AudioSystemHelper.PlaySfx(jumpSfx);
            await UniTask.Delay(TimeSpan.FromSeconds(JumpDuration), cancellationToken: destroyCancellationToken);
        }

        private async UniTask JumpRotate3()
        {
            var idx = 0;
            while (idx < aniArr.Length)
            {
                aniArr[idx].SetTrigger(AnimatorCache.RotateJumpTrigger);
                idx += 2;
            }
            AudioSystemHelper.PlaySfx(jumpSfx);

            await UniTask.Delay(TimeSpan.FromSeconds(0.5), cancellationToken: destroyCancellationToken);

            idx = 1;
            while (idx < aniArr.Length)
            {
                aniArr[idx].SetTrigger(AnimatorCache.RotateJumpTrigger);
                idx += 2;
            }

            AudioSystemHelper.PlaySfx(jumpSfx);

            await UniTask.Delay(TimeSpan.FromSeconds(JumpRotateDuration), cancellationToken: destroyCancellationToken);
        }
    }
}