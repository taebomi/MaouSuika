using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.GPGS;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.UI;
using SOSG.UGS.IAP;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace SOSG.MainScene
{
    public class AccountView : BaseUI, IModalUI
    {
        [SerializeField] private GPGSManagerSO gpgsManagerSO;
        [SerializeField] private IAPManagerSO iapManagerSO;

        [SerializeField] private LocalizeStringEvent signBtnLocalizeStringEvent;
        [SerializeField] private Button restoreTransactionBtn;

        private bool _canClickSignBtn;
        private bool _canClickRestoreTransactionsBtn;

        private bool _isShowing;

        private TBMStringTable _stringTable;

        protected override void AwakeAfter()
        {
            Canvas.enabled = false;
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private async UniTask SetUpAsync()
        {
            _stringTable = GetComponent<TBMStringTable>();
            await _stringTable.SetUpAsync(LocalizationTableName.MainScene);
        }

        public void OnAccountIconClicked() => ShowAsync().Forget();

        private async UniTaskVoid ShowAsync()
        {
            _stringTable.PrintConversation("account_open");

            SetSignBtn(gpgsManagerSO.IsAuthenticated);

            UIHelper.ShowUI(this);
            Canvas.enabled = true;

            await transform.SOSGUIPopUp().Play().WithCancellation(destroyCancellationToken);
            _isShowing = true;
        }

        private void SetSignBtn(bool value)
        {
            if (value)
            {
                signBtnLocalizeStringEvent.StringReference.TableEntryReference = "account_btn_signed_in";
                restoreTransactionBtn.interactable = true;
            }
            else
            {
                signBtnLocalizeStringEvent.StringReference.TableEntryReference = "account_btn_sign_in";
                restoreTransactionBtn.interactable = false;
            }
        }

        private void Hide()
        {
            if (_isShowing is false)
            {
                return;
            }

            _isShowing = false;

            UIHelper.HideUI(this);
            Canvas.enabled = false;
        }

        public void OnSignBtnClicked()
        {
            if (gpgsManagerSO.IsAuthenticated is false)
            {
                WaitIndicatorHelper.Show(this);
                gpgsManagerSO.Authenticate(OnAuthenticateFinished);
            }
            else
            {
                _stringTable.PrintConversation("account_already_authenticated");
            }
        }

        private void OnAuthenticateFinished(bool value)
        {
            WaitIndicatorHelper.Hide(this);

            if (value)
            {
                _stringTable.PrintConversation("account_sign_in_succeed");
                SetSignBtn(true);
            }
            else
            {
                _stringTable.PrintConversation("account_sign_in_failed");
                SetSignBtn(false);
            }
        }


        public void OnRestoreTransactionsBtnClicked()
        {
            WaitIndicatorHelper.Show(this);
            var canRestore = iapManagerSO.RestoreTransactions(OnRestoreTransactionsFinished);
            if (canRestore is false)
            {
                WaitIndicatorHelper.Hide(this);
                _stringTable.PrintConversation("account_restore_transaction_unavailable");
            }
        }

        private void OnRestoreTransactionsFinished(bool value)
        {
            if (value)
            {
                _stringTable.PrintConversation("account_restore_transaction_succeed");
            }
            else
            {
                _stringTable.PrintConversation("account_restore_transaction_failed");
            }

            WaitIndicatorHelper.Hide(this);
        }

        public void OnOverlayClicked() => Hide();
    }
}