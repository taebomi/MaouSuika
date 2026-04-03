using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System;
using SOSG.Utility;
using TaeBoMi;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace SOSG.UGS.IAP
{
    public class IAPManager : MonoBehaviour, IDetailedStoreListener
    {
        [SerializeField] private IAPManagerSO iapManagerSO;

        [SerializeField] private IAPWaiter waiter;
        private bool IsValidatorSupported => _validator is not null;

        private IStoreController _controller;
        private CrossPlatformValidator _validator;
        private IExtensionProvider _extensions;

        private Action<PurchaseResult> _onPurchaseFinished;
        private Action<bool> _onRestoreTransactionsFinished;

        public void SetUp()
        {
            iapManagerSO.Initialize();
            InitializeCallbacks(true);
        }


        public void TearDown()
        {
            InitializeCallbacks(false);
        }

        private void InitializeCallbacks(bool value)
        {
            if (value)
            {
                iapManagerSO.FuncIsPurchased = IsProductPurchased;
                iapManagerSO.OnPurchaseRequested = Purchase;
                iapManagerSO.OnRestoreTransactionRequested = RestoreTransaction;
            }
            else
            {
                iapManagerSO.FuncIsPurchased = null;
                iapManagerSO.OnPurchaseRequested = null;
            }
        }
        public void Initialize()
        {
            TBMUtility.Log($"# IAP Manager - Initialize Started");
#if UNITY_EDITOR
            StandardPurchasingModule.Instance().useFakeStoreAlways = true;
            StandardPurchasingModule.Instance().useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#else
            StandardPurchasingModule.Instance().useFakeStoreAlways = false;
#endif
            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
            var catalog = ProductCatalog.LoadDefaultCatalog();
            IAPConfigurationHelper.PopulateConfigurationBuilder(ref builder, catalog);

            UnityPurchasing.Initialize(this, builder);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            TBMUtility.Log($"# IAP Manager - Initialize Success");
            _controller = controller;
            _extensions = extensions;

#if !UNITY_EDITOR
            InitializeValidator();
#endif
            iapManagerSO.SetInitializeState(true);
        }

        private void InitializeValidator()
        {
            if (StandardPurchasingModule.Instance().appStore is AppStore.GooglePlay or AppStore.AppleAppStore
                or AppStore.MacAppStore)
            {
                _validator = new CrossPlatformValidator(
                    GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            }
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            TBMUtility.Log($"# IAP Manager - Initialize Failed");
            TBMUtility.Log($"## error - {error}");
            iapManagerSO.SetInitializeState(false);
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            TBMUtility.Log($"# IAP Manager - Initialize Failed");
            TBMUtility.Log($"## error   - {error}");
            TBMUtility.Log($"## message - {message}");
            iapManagerSO.SetInitializeState(false);
        }

        private void Purchase(string productId, Action<PurchaseResult> onPurchaseFinished)
        {
            TBMUtility.Log($"# IAP Manager - Purchase Started");
            if (iapManagerSO.IsInitialized is false)
            {
                TBMUtility.Log($"# IAP Manager - Purchase Canceled");
                TBMUtility.Log($"## IAP Manager is not initialized");
                return;
            }

            TBMTimeScale.Pause(this); // 구매 중 게임 일시 정지
            _onPurchaseFinished = onPurchaseFinished;
            _controller.InitiatePurchase(IAPProductId.GetFullId(productId));
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            TBMUtility.Log("# IAP Manager - Process Purchase");
            ProcessPurchaseAsync(purchaseEvent).Forget();
            return PurchaseProcessingResult.Complete;
        }

        private async UniTaskVoid ProcessPurchaseAsync(PurchaseEventArgs purchaseEvent)
        {
            var product = purchaseEvent.purchasedProduct;
            var productId = purchaseEvent.purchasedProduct.definition.id;
            var isValid = IsPurchaseValid(product);

            await waiter.Wait(destroyCancellationToken);
            TBMTimeScale.UnPause(this);

            if (isValid)
            {
                TBMUtility.Log($"# IAP Manager - Purchase Succeed");
                var result = new PurchaseResult(productId, PurchaseResult.Result.Succeed);
                _onPurchaseFinished?.Invoke(result);
            }
            else
            {
                TBMUtility.Log($"# IAP Manager - Purchase Failed");
                TBMUtility.Log($"## reason - Receipt Invalid");
                var result = new PurchaseResult(productId, PurchaseResult.Result.ReceiptInvalid);
                _onPurchaseFinished?.Invoke(result);
            }
        }

        /// <summary>
        /// 미사용 함수
        /// </summary>
        /// <param name="product"></param>
        /// <param name="failureReason"></param>
        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            TBMUtility.LogWarning($"# IAP Manager - 호출 되면 안되는 함수, 호출 될 시 내용 수정할 것");
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            TBMUtility.Log($"# IAP Manager - Purchase Failed");
            TBMUtility.Log($"## product - {product}");
            TBMUtility.Log($"## failure reason - {failureDescription.reason}");
            TBMUtility.Log($"## message - {failureDescription.message}");

            OnPurchaseFailed(product.definition.id, failureDescription).Forget();
        }

        private async UniTaskVoid OnPurchaseFailed(string id, PurchaseFailureDescription description)
        {
            await waiter.Wait(destroyCancellationToken);
            TBMTimeScale.UnPause(this);

            var purchaseResult = new PurchaseResult
            {
                ProductId = id,
                ResultCode = description.reason switch
                {
                    PurchaseFailureReason.PurchasingUnavailable => PurchaseResult.Result.PurchasingUnavailable,
                    PurchaseFailureReason.ExistingPurchasePending => PurchaseResult.Result.ExistingPurchasePending,
                    PurchaseFailureReason.ProductUnavailable => PurchaseResult.Result.ProductUnavailable,
                    PurchaseFailureReason.SignatureInvalid => PurchaseResult.Result.SignatureInvalid,
                    PurchaseFailureReason.UserCancelled => PurchaseResult.Result.UserCancelled,
                    PurchaseFailureReason.PaymentDeclined => PurchaseResult.Result.PaymentDeclined,
                    PurchaseFailureReason.DuplicateTransaction => PurchaseResult.Result.DuplicateTransaction,
                    PurchaseFailureReason.Unknown => PurchaseResult.Result.Unknown,
                    _ => throw new ArgumentOutOfRangeException()
                }
            };
            _onPurchaseFinished?.Invoke(purchaseResult);
        }


        // 영수증 검증

        private bool IsPurchaseValid(Product product)
        {
            TBMUtility.Log($"# IAP Manager - 영수증 검증 시작");
            if (IsValidatorSupported is false)
            {
                TBMUtility.Log($"## 영수증 검증 미지원");
                return true;
            }

            try
            {
                TBMUtility.Log($"## receipt - {product.receipt}");
                var result = _validator.Validate(product.receipt);
                foreach (var purchaseReceipt in result)
                {
                    if (purchaseReceipt is GooglePlayReceipt googleReceipt)
                    {
                        TBMUtility.Log($"## google receipt - {googleReceipt}");
                        TBMUtility.Log($"## purchase state - {googleReceipt.purchaseState}");
                        if (googleReceipt.purchaseState != GooglePurchaseState.Purchased)
                        {
                            continue;
                        }
                    }

                    if (purchaseReceipt.productID.Contains(IAPProductId.Prefix))
                    {
                        TBMUtility.Log($"## 유효한 영수증");
                        return true;
                    }
                }
            }
            catch (IAPSecurityException ex)
            {
                TBMUtility.Log($"## IAPSecurityException - {ex.Message}");
                return false;
            }

            TBMUtility.Log($"## 유효하지 않은 영수증");
            return false;
        }

        private bool IsProductPurchased(string productId)
        {
            TBMUtility.Log($"# IAP Manager - Check Product Purchased Started.");
            if (iapManagerSO.IsInitialized is false)
            {
                TBMUtility.Log($"## IAP Manager is not initialized.");
                return false;
            }

            var product = GetProductById(productId);
            if (product is null)
            {
                TBMUtility.Log($"## cannot find product.");
                return false;
            }

            if (product.hasReceipt is false)
            {
                TBMUtility.Log($"## {product.definition.id} has no receipt.");
                return false;
            }

            if (IsPurchaseValid(product) is false)
            {
                TBMUtility.Log($"## {product.definition.id} receipt is invalid.");
                return false;
            }

            TBMUtility.Log($"## {product.definition.id} is purchased.");
            return true;
        }

        private bool RestoreTransaction(Action<bool> resultCallback)
        {
            TBMUtility.Log($"# IAP Manager - Restore Purchase Started");
            TBMUtility.Log($"## app store - {StandardPurchasingModule.Instance().appStore}");
            if (StandardPurchasingModule.Instance().appStore is AppStore.GooglePlay)
            {
                TBMUtility.Log($"##{_extensions}");
                var googleExtension = _extensions.GetExtension<IGooglePlayStoreExtensions>();
                TBMUtility.Log($"## google extension - {googleExtension}");
                _onRestoreTransactionsFinished = resultCallback;
                googleExtension.RestoreTransactions(OnRestoreTransactionsFinished);
                return true;
            }

            return false;
        }

        private void OnRestoreTransactionsFinished(bool success, string msg)
        {
            if (success)
            {
                TBMUtility.Log($"# IAP Manager - Restore Purchase Succeed");
            }
            else
            {
                TBMUtility.Log($"# IAP Manager - Restore Purchase Failed");
                TBMUtility.Log($"## message - {msg}");
            }

            _onRestoreTransactionsFinished?.Invoke(success);
            _onRestoreTransactionsFinished = null;
        }

        private Product GetProductById(string id)
        {
            if (_controller is null)
            {
                return null;
            }

            var fullId = IAPProductId.GetFullId(id);
            var product = _controller.products.WithID(fullId);
            return product;
        }

    }
}