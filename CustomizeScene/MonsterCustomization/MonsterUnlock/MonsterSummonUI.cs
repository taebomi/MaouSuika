using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.PlayData;
using SOSG.System.UI;
using SOSG.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Customization.Monster
{
    public class MonsterSummon : MonoBehaviour, IModalUI
    {
        [SerializeField] private MonsterUnlock monsterUnlock;

        [SerializeField] private MagiSO magiSO;
        

           [field:SerializeField] public Canvas Canvas { get; private set; }
           public void OnCloseRequested()
           {
               throw new NotImplementedException();
           }

           public bool CanInteract { get; private set; }

        private event Action OnUiClosed;

        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            _destroyCts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
        }

        public void Initialize(Action onUIClosed)
        {
            OnUiClosed = onUIClosed;
            gameObject.SetActive(false);
        }
        
        public async UniTaskVoid Open()
        {
            gameObject.SetActive(true);
            CanInteract = false;
            // modalUI.Show();
            await transform.SOSGUIPopUp().Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, _destroyCts.Token);
            CanInteract = true;
        }

        public void OnOverlayClicked() => Close();

        public void Close()
        {
            if (CanInteract is false)
            {
                return;
            }

            CanInteract = false;
            // modalUI.Hide();
            gameObject.SetActive(false);
            
            OnUiClosed?.Invoke();
        }


        public void OnSummonBtnClicked()
        {
            if (CanInteract is false)
            {
                return;
            }
            
            monsterUnlock.OnSummonBtnClicked();
        }

        public void OnSummonStarted() => CanInteract = false;
        public void OnSummonFinished() => CanInteract = true;

    }
}