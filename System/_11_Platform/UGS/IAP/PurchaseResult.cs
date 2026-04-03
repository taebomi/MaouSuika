namespace SOSG.UGS.IAP
{
    public struct PurchaseResult
    {
        public string ProductId;
        public Result ResultCode;

        public enum Result
        {
            None,
            NotInitialized,
            Succeed,
            ReceiptInvalid,

            /// <summary>
            /// Purchasing may be disabled in security settings.
            /// </summary>
            PurchasingUnavailable,

            /// <summary>
            /// Another purchase is already in progress.
            /// </summary>
            ExistingPurchasePending,

            /// <summary>
            /// The product was reported unavailable by the purchasing system.
            /// </summary>
            ProductUnavailable,

            /// <summary>
            /// Signature validation of the purchase's receipt failed.
            /// </summary>
            SignatureInvalid,

            /// <summary>
            /// The user opted to cancel rather than proceed with the purchase.
            /// This is not specified on platforms that do not distinguish
            /// cancellation from other failure (Amazon, Microsoft).
            /// </summary>
            UserCancelled,

            /// <summary>
            /// There was a problem with the payment.
            /// This is unique to Apple platforms.
            /// </summary>
            PaymentDeclined,

            /// <summary>
            /// The transaction has already been completed successfully. This error can occur
            /// on Apple platforms if the transaction is finished successfully while the user
            /// is logged out of the app store, using a receipt generated while the user was
            /// logged in.
            /// </summary>
            DuplicateTransaction,

            /// <summary>
            /// A catch all for remaining purchase problems.
            /// Note: Use Enum.Parse to use this named constant if targeting Unity 5.3
            /// or 5.4. Its value differs for 5.5+ which introduced DuplicateTransaction.
            /// </summary>
            Unknown
        }


        public PurchaseResult(string productId)
        {
            ProductId = productId;
            ResultCode = Result.None;
        }

        public PurchaseResult(string productId, Result result)
        {
            ProductId = productId;
            ResultCode = result;
        }
    }
}