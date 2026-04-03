using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

// todo 나중에 battleArea로 합칠 예정.
public class TitleScreenMap : MonoBehaviour
{
    [SerializeField] private Transform curNearAreaTr, nextNearAreaTr; 
    [SerializeField] private Transform curFarAreaTr, nextFarAreaTr;
    [SerializeField] private Transform cloudsTr;

    [SerializeField] private Transform nearAreaTr;
    [SerializeField] private Transform farAreaTr;

    private CancellationTokenSource _destroyCts;

    private const float ScrollSpeed = 1.5f;
    private const float FarScrollSpeed = ScrollSpeed * 0.5f;
    public const float AreaWidth = 48f;

    private void Awake()
    {
        _destroyCts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        _destroyCts.CancelAndDispose();
    }

    public void SetMainScenePosition(bool isFirst)
    {
        if (isFirst)
        {
            var firstPos = new Vector3(0f, -15f);
            nearAreaTr.localPosition = firstPos;
            farAreaTr.localPosition = firstPos;
            cloudsTr.localPosition = firstPos;
        }
    }

    public void MoveUpNearMap()
    {
        nearAreaTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
    }

    public void MoveUpFarMap()
    {
        farAreaTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
    }

    public void MoveUpClouds()
    {
        cloudsTr.DOLocalMoveY(0f, 0.3f).SetEase(Ease.OutBack).Play();
    }


    public async UniTaskVoid ScrollMap()
    {
        while (_destroyCts.IsCancellationRequested is false)
        {
            var moveDelta = -ScrollSpeed * Time.unscaledDeltaTime;
            var farMoveDelta = -FarScrollSpeed * Time.unscaledDeltaTime;

            curNearAreaTr.position += new Vector3(moveDelta, 0f);
            nextNearAreaTr.position += new Vector3(moveDelta, 0f);
            curFarAreaTr.position += new Vector3(farMoveDelta, 0f);
            nextFarAreaTr.position += new Vector3(farMoveDelta, 0f);

            if (curNearAreaTr.localPosition.x < -AreaWidth)
            {
                curNearAreaTr.position = nextNearAreaTr.position + new Vector3(AreaWidth, 0f);
                (curNearAreaTr, nextNearAreaTr) = (nextNearAreaTr, curNearAreaTr);
            }

            if (curFarAreaTr.localPosition.x < -AreaWidth)
            {
                curFarAreaTr.position = nextFarAreaTr.position + new Vector3(AreaWidth, 0f);
                (curFarAreaTr, nextFarAreaTr) = (nextFarAreaTr, curFarAreaTr);
            }

            await UniTask.Yield(_destroyCts.Token);
        }
    }
}


