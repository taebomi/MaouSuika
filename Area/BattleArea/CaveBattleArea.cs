using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.Area;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CaveBattleArea : BattleArea
{
    [SerializeField] private Transform nearArea1Tr;
    [SerializeField] private Transform nearArea2Tr;

    [SerializeField] private Transform nearArea1ContainerTr;
    [SerializeField] private Transform nearArea2ContainerTr;

    [SerializeField] private Tilemap bgTilemap;

    private Transform _curNearArea1Tr, _nextNearArea1Tr;
    private Transform _curNearArea2Tr, _nextNearArea2Tr;

    private const float AppearDelay = 0.35f;
    private const float AppearDuration = 0.35f;
    private const float AppearInterval = 0.25f;

    protected override void InitializeAfter(AreaData areaData)
    {
        _curNearArea1Tr = nearArea1Tr;
        _curNearArea2Tr = nearArea2Tr;

        _nextNearArea1Tr = Instantiate(nearArea1Tr, nearArea1ContainerTr);
        _nextNearArea2Tr = Instantiate(nearArea2Tr, nearArea2ContainerTr);
        _nextNearArea1Tr.localPosition = new Vector3(AreaWidth, 0f);
        _nextNearArea2Tr.localPosition = new Vector3(AreaWidth, 0f);
    }

    public override void InitializePosition(Vector3 localPos)
    {
        nearArea1ContainerTr.localPosition = localPos;
        nearArea2ContainerTr.localPosition = localPos;
    }

    public override void OnMainSceneEvent(int timing)
    {
        if (timing == 1)
        {
            nearArea1ContainerTr.DOLocalMoveY(0f, AppearDuration).SetEase(Ease.OutBack).Play();
        }
        else if (timing == 2)
        {
            nearArea2ContainerTr.DOLocalMoveY(0f, AppearDuration).SetEase(Ease.OutBack).Play();
        }
        else if (timing == 10)
        {
            ScrollArea().Forget();
        }
    }

    public override async UniTask Appear()
    {
        FadeBackground(true).Forget();
        await UniTask.Delay(TimeSpan.FromSeconds(AppearDelay), cancellationToken: DestroyCts.Token);
        _ = nearArea1ContainerTr.DOLocalMoveY(0f, AppearDuration).SetEase(Ease.OutBack).Play();
        await UniTask.Delay(TimeSpan.FromSeconds(AppearInterval), cancellationToken: DestroyCts.Token);
        await nearArea2ContainerTr.DOLocalMoveY(0f, AppearDuration).SetEase(Ease.OutBack).Play();
    }

    public override async UniTask Disappear()
    {
        FadeBackground(false).Forget();
        _ = nearArea1ContainerTr.DOLocalMoveY(BattleAreaManager.WaitingAreaYPos, AppearDuration)
            .SetEase(Ease.InBack).Play();
        await UniTask.Delay(TimeSpan.FromSeconds(AppearInterval), cancellationToken: DestroyCts.Token);
        await nearArea2ContainerTr.DOLocalMoveY(BattleAreaManager.WaitingAreaYPos, AppearDuration)
            .SetEase(Ease.InBack).Play();
    }

    public override async UniTaskVoid ScrollArea()
    {
        while (DestroyCts.IsCancellationRequested is false)
        {
            var moveDelta = OverlordMoveXSpeedVarSO.Value * Time.deltaTime;
            _curNearArea1Tr.Translate(moveDelta, 0f, 0f);
            _curNearArea2Tr.Translate(moveDelta, 0f, 0f);
            _nextNearArea1Tr.Translate(moveDelta, 0f, 0f);
            _nextNearArea2Tr.Translate(moveDelta, 0f, 0f);
            if (_curNearArea1Tr.localPosition.x < -AreaWidth)
            {
                _curNearArea1Tr.position = _nextNearArea1Tr.position + new Vector3(AreaWidth, 0f);
                (_curNearArea1Tr, _nextNearArea1Tr) = (_nextNearArea1Tr, _curNearArea1Tr);
            }

            if (_curNearArea2Tr.localPosition.x < -AreaWidth)
            {
                _curNearArea2Tr.position = _nextNearArea2Tr.position + new Vector3(AreaWidth, 0f);
                (_curNearArea2Tr, _nextNearArea2Tr) = (_nextNearArea2Tr, _curNearArea2Tr);
            }

            await UniTask.Yield(DestroyCts.Token);
        }
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
}