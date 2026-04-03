using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.CashShop;
using SOSG.System;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.UI;
using SOSG.UGS.IAP;
using UnityEngine;

namespace SOSG.MainScene
{
    public class CashShopView : BaseUI, IModalUI
    {
        [SerializeField] private IAPManagerSO iapManagerSO;
        
        [SerializeField] private CashShopDescription description;
        [SerializeField] private GameObject purchaseBtn;
        
        private TBMStringTable _stringTable;

        private bool _hasPurchasedBeforeVisit;
        private bool _hasPurchasedCurrentVisit;

        private bool _isShowing;

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

        public async UniTaskVoid ShowAsync()
        {
            if (iapManagerSO.IsPurchased(IAPProductId.RemoveInterstitialAds))
            {
                _stringTable.PrintConversation("cashshop_open_nothing_can_purchase");
                purchaseBtn.SetActive(false);
                _hasPurchasedBeforeVisit = true;
            }
            else
            {
                _stringTable.PrintConversation("cashshop_open");
                purchaseBtn.SetActive(true);
                _hasPurchasedBeforeVisit = false;
            }
            
            UIHelper.ShowUI(this);
            Canvas.enabled = true;
            await transform.SOSGUIPopUp().Play().WithCancellation(destroyCancellationToken);

            _isShowing = true;
        }

        public void OnOverlayClicked() => Hide();

        private void Hide()
        {
            if (_isShowing is false)
            {
                return;
            }

            if (_hasPurchasedCurrentVisit)
            {
                _stringTable.PrintConversation("cashshop_close_purchase");
            }
            else
            {
                if (_hasPurchasedBeforeVisit)
                {
                    _stringTable.PrintConversation("cashshop_close_nothing_purchase_but_has_purchased_before");
                }
                else
                {
                    _stringTable.PrintConversation("cashshop_close_nothing_purchase");
                }
            }

            UIHelper.HideUI(this);
            Canvas.enabled = false;
            _isShowing = false;
        }

        public void OnPurchaseBtnClicked()
        {
            var isPurchased = iapManagerSO.IsPurchased(IAPProductId.RemoveInterstitialAds);
            if (isPurchased)
            {
                _stringTable.PrintConversation("cashshop_purchase_but_already_purchased");
            }
            else
            {
                WaitIndicatorHelper.Show(this);
                iapManagerSO.PurchaseProduct(IAPProductId.RemoveInterstitialAds, OnPurchaseFinished);
            }
        }

        private void OnPurchaseFinished(PurchaseResult result)
        {
            WaitIndicatorHelper.Hide(this);
            if (result.ResultCode is PurchaseResult.Result.Succeed)
            {
                OnPurchaseSucceed(result.ProductId);
            }
            else
            {
                OnPurchaseFailed(result.ResultCode);
            }
        }

        private void OnPurchaseSucceed(string id)
        {
            _hasPurchasedBeforeVisit = true;
            _hasPurchasedCurrentVisit = true;
            if (id is IAPProductId.RemoveInterstitialAds)
            {
                _stringTable.PrintConversation("cashshop_interstitial_ads_purchase");
            }

            purchaseBtn.SetActive(false);
        }

        private void OnPurchaseFailed(PurchaseResult.Result result)
        {
            switch (result)
            {
                case PurchaseResult.Result.ExistingPurchasePending:
                    _stringTable.PrintConversation("cashshop_purchase_failed_existing_purchase_pending");
                    break;
                case PurchaseResult.Result.UserCancelled:
                    _stringTable.PrintConversation("cashshop_purchase_failed_user_cancelled");
                    break;
                case PurchaseResult.Result.PaymentDeclined:
                    _stringTable.PrintConversation("cashshop_purchase_failed_payment_declined");
                    break;
                case PurchaseResult.Result.DuplicateTransaction:
                case PurchaseResult.Result.Unknown:
                case PurchaseResult.Result.PurchasingUnavailable:
                case PurchaseResult.Result.SignatureInvalid:
                case PurchaseResult.Result.ProductUnavailable:
                    _stringTable.PrintConversation("cashshop_purchase_failed_unknown");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}