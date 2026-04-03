using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using FMODUnity;
using SOSG.Stage;
using SOSG.System;
using SOSG.System.Audio;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace SOSG.Skill
{
    public class SuikaSlash : MonoBehaviour
    {
        [SerializeField] private Transform cursorTr;
        [SerializeField] private SpriteRenderer cursorSr;

        [SerializeField] private SpriteRenderer topBlackScreenSr;
        [SerializeField] private SpriteRenderer bottomBlackScreenSr;
        [SerializeField] private GameObject slashEffect;

        [SerializeField] private EventReference sfxRef;

        private SkillManager _manager;
        private Sequence _colorSequence;
        private Gashapon _targetGashapon;

        private ContactFilter2D _contactFilter;
        private Collider2D[] _colliderArr;
        private Vector3 _lastTouchedPos;

        public string skillNameDialoguePrefix = "suika-slash";


        private CancellationTokenSource _skillCts;

        private void Awake()
        {
            _colorSequence = DOTween.Sequence().Append(
                    cursorSr.DOColor(Color.yellow, 0.25f).From(Color.white).SetEase(Ease.InOutSine))
                .SetUpdate(true).SetLoops(-1, LoopType.Yoyo).SetAutoKill(false);
            _colliderArr = new Collider2D[5];
            _contactFilter = new ContactFilter2D()
            {
                layerMask = PhysicsCache.GetLayerMask(PhysicsCache.LayerName.Gashapon),
                useLayerMask = true,
            };
            cursorTr.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _skillCts?.CancelAndDispose();
        }

        public void Initialize(SkillManager manager)
        {
            _manager = manager;

            var bottomCam = manager.BottomScreenCameraVarSO.value;
            var bottomCamHeight = bottomCam.orthographicSize;
            var bottomCamWidth = bottomCamHeight * bottomCam.aspect;
            var bottomCamSize = new Vector2(bottomCamWidth * 2f, bottomCamHeight * 2f);
            var bottomBlackScreenPos = bottomCam.transform.position;
            bottomBlackScreenPos.z = 0f;
            bottomBlackScreenSr.transform.position = bottomBlackScreenPos;
            bottomBlackScreenSr.transform.localScale = new Vector3(bottomCamSize.x, bottomCamSize.y, 1f);
            bottomBlackScreenSr.gameObject.SetActive(false);

            var topCam = manager.TopScreenCameraVarSO.value;
            var topCamHeight = topCam.orthographicSize;
            var topCamWidth = topCamHeight * topCam.aspect;
            var topCamSize = new Vector2(topCamWidth * 2f, topCamHeight * 2f);
            var topBlackScreenPos = topCam.transform.position;
            topBlackScreenPos.z = 0f;
            topBlackScreenSr.transform.position = topBlackScreenPos;
            topBlackScreenSr.transform.localScale = new Vector3(topCamSize.x, topCamSize.y, 1f);
            topBlackScreenSr.gameObject.SetActive(false);

            slashEffect.gameObject.SetActive(false);
        }

        public void Activate()
        {
            _colorSequence.Play();
            _skillCts?.CancelAndDispose();
            _skillCts = new CancellationTokenSource();
            _targetGashapon = null;
            Aim().Forget();
        }


        public void Cancel()
        {
            _skillCts?.Cancel();
            _colorSequence.Pause();
            _manager.OnSkillCanceled();
            cursorTr.gameObject.SetActive(false);
        }

        private async UniTask WaitForFirstTouch()
        {
            await UniTask.WaitUntil(() => Touch.activeTouches.Count > 0, cancellationToken: _skillCts.Token);
        }

        private async UniTask Aim()
        {
            await WaitForFirstTouch();
            // 터치를 했다면, 터치를 뗄 때까지 커서 따라다니기
            var cam = _manager.BottomScreenCameraVarSO.value;
            var collDict = _manager.GashaponCollDictVarSO.Dict;
            while (_skillCts.IsCancellationRequested is false)
            {
                if (Touch.activeTouches.Count == 0)
                {
                    OnTouchFinished();
                    return;
                }

                if (StageGashaponArea.GetTouchWorldPos(cam, out var touchPos) is false)
                {
                    if (_targetGashapon)
                    {
                        _targetGashapon.SetShooterState(GashaponShooter.State.None);
                        _targetGashapon = null;
                    }

                    cursorTr.gameObject.SetActive(false);
                    await UniTask.Yield(_skillCts.Token);
                    continue;
                }

                cursorTr.gameObject.SetActive(true);
                cursorTr.position = touchPos;

                // 가샤폰 체크
                var collidedCount = Physics2D.OverlapPoint(touchPos, _contactFilter, _colliderArr);
                for (var i = 0; i < collidedCount; i++)
                {
                    if (collDict.TryGetValue(_colliderArr[i].GetInstanceID(), out var gashapon))
                    {
                        if (gashapon.IsGrounded is false || gashapon.IsMerging)
                        {
                            continue;
                        }

                        if (_targetGashapon != gashapon && _targetGashapon)
                        {
                            _targetGashapon.SetShooterState(GashaponShooter.State.None);
                        }

                        _targetGashapon = gashapon;
                        gashapon.SetShooterState(GashaponShooter.State.Collided);
                        break;
                    }
                }

                if (_targetGashapon)
                {
                    if (collidedCount == 0 || _targetGashapon.IsMerging)
                    {
                        _targetGashapon.SetShooterState(GashaponShooter.State.None);
                        _targetGashapon = null;
                    }
                }

                await UniTask.Yield(_skillCts.Token);
            }
        }

        private void OnTouchFinished()
        {
            _colorSequence.Pause();
            cursorSr.gameObject.SetActive(false);
            // 현재 선택된 가샤폰이 존재한다면 해당 가샤폰에 스킬 사용.
            if (_targetGashapon)
            {
                SlashSuika().Forget();
            }
            else
            {
                _manager.OnSkillCanceled();
            }
        }

        private async UniTaskVoid SlashSuika()
        {
            // _manager..SetListener(true);
            // _manager.DialogueHelper.RequestLine($"{skillNameDialoguePrefix}-use-started");

            topBlackScreenSr.gameObject.SetActive(true);
            bottomBlackScreenSr.gameObject.SetActive(true);
            _ = topBlackScreenSr.DOFade(1f, 2.5f).From(0f).SetEase(Ease.Linear).SetUpdate(true).Play();
            _ = bottomBlackScreenSr.DOFade(1f, 2.5f).From(0f).SetEase(Ease.Linear).SetUpdate(true).Play();
            TBMTimeScale.Set(this, 0f);

            // 대사 중간에 이벤트 받았을 때 스킬 발생시킴.
            var waitCts = new CancellationTokenSource();
            var waitCt = CancellationTokenSource.CreateLinkedTokenSource(waitCts.Token, _skillCts.Token).Token;
            Action<string[]> waitForSlashEvent = delegate(string[] eventParams)
            {
                if ((eventParams.Length == 1 && eventParams[0] == "slash") is false)
                {
                    return;
                }

                waitCts.CancelAndDispose();
                // _manager.DialogueHelper.SetListener(false);
            };

            // _manager.DialogueHelper.OnLineEventRaised += waitForSlashEvent;
            await waitCt.WaitUntilCanceled();
            // _manager.DialogueHelper.OnLineEventRaised -= waitForSlashEvent;

            // 대사 중간의 이벤트 발생 후, 화면 밝아지며 
            TBMTimeScale.Unset(this);


            _ = topBlackScreenSr.DOFade(0f, 1f).SetEase(Ease.InSine).SetUpdate(true).Play();
            _ = bottomBlackScreenSr.DOFade(0f, 1f).SetEase(Ease.InSine).SetUpdate(true).Play();

            slashEffect.transform.position = _targetGashapon.transform.position;
            slashEffect.gameObject.SetActive(true);
            _targetGashapon.Deactivate();

            AudioSystemHelper.PlaySfx(sfxRef);
            _manager.OnSkillUseFinished();
            await UniTask.WaitUntil(() => !slashEffect.activeSelf, cancellationToken: _skillCts.Token);
            topBlackScreenSr.gameObject.SetActive(false);
            bottomBlackScreenSr.gameObject.SetActive(false);
        }
    }
}