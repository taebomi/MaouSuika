using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace SOSG.Area
{
    public class OutsideBattleArea : BattleArea
    {
        [SerializeField] private Transform nearAreaTr;
        [SerializeField] private Transform farAreaTr;

        [SerializeField] private Transform nearAreaContainerTr;
        [SerializeField] private Transform farAreaContainerTr;
        [SerializeField] private Transform skyAreaContainerTr;

        [SerializeField] private Tilemap bgTilemap;

        private Transform _curNearAreaTr, _nextNearAreaTr;
        private Transform _curFarAreaTr, _nextFarAreaTr;

        private const float FarScrollSpeedMultiplier = 0.5f;
        private const float ChangingDuration = 0.35f;
        private const float ChangingDelay = 0.15f;


        protected override void InitializeAfter(AreaData areaData)
        {
            _curNearAreaTr = nearAreaTr;
            _curFarAreaTr = farAreaTr;

            _nextNearAreaTr = Instantiate(nearAreaTr, nearAreaContainerTr);
            _nextFarAreaTr = Instantiate(farAreaTr, farAreaContainerTr);
            _nextNearAreaTr.localPosition = new Vector3(AreaWidth, 0f);
            _nextFarAreaTr.localPosition = new Vector3(AreaWidth, 0f);
        }

        public override void InitializePosition(Vector3 localPos)
        {
            nearAreaContainerTr.localPosition = localPos;
            farAreaContainerTr.localPosition = localPos;
            skyAreaContainerTr.localPosition = localPos;
        }


        public override void OnMainSceneEvent(int timing)
        {
            if (timing == 0)
            {
                nearAreaContainerTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
            }
            else if (timing == 1)
            {
                farAreaContainerTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
            }
            else if (timing == 2)
            {
                skyAreaContainerTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
            }
            else if (timing == 10)
            {
                ScrollArea().Forget();
            }
        }

        public override async UniTask Appear()
        {
            FadeBackground(true).Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(ChangingDuration * 0.5f), cancellationToken: DestroyCts.Token);
            _ = nearAreaContainerTr.DOLocalMoveY(0f, ChangingDuration).SetEase(Ease.OutBack).Play();
            await UniTask.Delay(TimeSpan.FromSeconds(ChangingDelay), cancellationToken: DestroyCts.Token);
            _ = farAreaContainerTr.DOLocalMoveY(0f, ChangingDuration).SetEase(Ease.OutBack).Play();
            await UniTask.Delay(TimeSpan.FromSeconds(ChangingDelay), cancellationToken: DestroyCts.Token);
            await skyAreaContainerTr.DOLocalMoveY(0f, ChangingDuration).SetEase(Ease.OutBack).Play()
                .WithCancellation(DestroyCts.Token);
        }

        public override async UniTask Disappear()
        {
            FadeBackground(false).Forget();
            _ = nearAreaContainerTr.DOLocalMoveY(BattleAreaManager.WaitingAreaYPos, ChangingDuration)
                .SetEase(Ease.InBack).Play();
            await UniTask.Delay(TimeSpan.FromSeconds(ChangingDelay), cancellationToken: DestroyCts.Token);
            _ = farAreaContainerTr.DOLocalMoveY(BattleAreaManager.WaitingAreaYPos, ChangingDuration)
                .SetEase(Ease.InBack).Play();
            await UniTask.Delay(TimeSpan.FromSeconds(ChangingDelay), cancellationToken: DestroyCts.Token);
            await skyAreaContainerTr.DOLocalMoveY(BattleAreaManager.WaitingAreaYPos, ChangingDuration)
                .SetEase(Ease.InBack).Play()
                .WithCancellation(DestroyCts.Token);
        }

        private async UniTaskVoid FadeBackground(bool willAppear)
        {
            var timer = 0f;
            const float duration = 1f;
            while (timer < duration)
            {
                var alpha = Easing.OutSine(timer, duration);
                alpha = willAppear ? alpha : 1f - alpha;
                bgTilemap.color = new Color(1f, 1f, 1f, alpha);
                timer += Time.deltaTime;
                await UniTask.Yield(DestroyCts.Token);
            }
        }

        public override async UniTaskVoid ScrollArea()
        {
            while (DestroyCts.IsCancellationRequested is false)
            {
                // 이동
                var nearMoveDelta = OverlordMoveXSpeedVarSO.Value * Time.deltaTime;
                _curNearAreaTr.Translate(nearMoveDelta, 0f, 0f);
                _nextNearAreaTr.Translate(nearMoveDelta, 0f, 0f);
                var farMoveDelta = nearMoveDelta * FarScrollSpeedMultiplier;
                _curFarAreaTr.Translate(farMoveDelta, 0f, 0f);
                _nextFarAreaTr.Translate(farMoveDelta, 0f, 0f);

                // 범위 체크
                if (_curNearAreaTr.localPosition.x < -AreaWidth)
                {
                    _curNearAreaTr.position = _nextNearAreaTr.position + new Vector3(AreaWidth, 0f);
                    (_curNearAreaTr, _nextNearAreaTr) = (_nextNearAreaTr, _curNearAreaTr);
                }

                if (_curFarAreaTr.localPosition.x < -AreaWidth)
                {
                    _curFarAreaTr.position = _nextFarAreaTr.position + new Vector3(AreaWidth, 0f);
                    (_curFarAreaTr, _nextFarAreaTr) = (_nextFarAreaTr, _curFarAreaTr);
                }

                await UniTask.Yield(DestroyCts.Token);
            }
        }
    }
}