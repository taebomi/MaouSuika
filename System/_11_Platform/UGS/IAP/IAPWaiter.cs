using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System.Input;
using TMPro;
using UnityEngine;

namespace SOSG.UGS.IAP
{
    public class IAPWaiter : MonoBehaviour
    {
        [SerializeField] private GameInputSO inputSO;
    
        [SerializeField] private TMP_Text text;
    
        public async UniTask Wait(CancellationToken ct)
        {
            inputSO.DisableAllInputs(true);
            gameObject.SetActive(true);
            
            await UniTask.Yield(); // 구매 창에서 돌아오면 unscaled delta time이 해당 시간만큼 있어서 초기화하기 위함.
            var timer = 0f;
            const float duration = 1f;
            while (timer < duration && ct.IsCancellationRequested is false)
            {
                text.text = $"{timer / duration * 100f:00}%";
                timer += Time.unscaledDeltaTime;
                await UniTask.Yield(ct);
            }

            inputSO.DisableAllInputs(false);
            gameObject.SetActive(false);
        }
    }
}