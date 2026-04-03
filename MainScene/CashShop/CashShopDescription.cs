using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace SOSG.CashShop
{
    public class CashShopDescription : MonoBehaviour
    {
        [SerializeField] private RectTransform rt;
        [SerializeField] private RectTransform textRt;
        [SerializeField] private TMP_Text descriptionText;

        private CancellationTokenSource _enableCts;

        private void OnEnable()
        {
            _enableCts?.Dispose();
            _enableCts = new CancellationTokenSource();
            ShowAsync(_enableCts.Token).Forget();
        }

        private void OnDisable()
        {
            _enableCts.Cancel();
        }

        private async UniTaskVoid ShowAsync(CancellationToken ct)
        {
            textRt.anchoredPosition = new Vector2(rt.rect.width, 0f);
            const float speed = 150f;
            while (ct.IsCancellationRequested is false)
            {
                textRt.anchoredPosition -= new Vector2(speed * Time.deltaTime, 0f);
                if (textRt.anchoredPosition.x < -descriptionText.preferredWidth)
                {
                    textRt.anchoredPosition = new Vector2(rt.rect.width, 0f);
                }

                await UniTask.Yield(ct);
            }
        }
    }
}