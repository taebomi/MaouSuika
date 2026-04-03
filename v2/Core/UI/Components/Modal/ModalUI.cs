using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TBM.MaouSuika.Core.UI
{
    public class ModalUI : MonoBehaviour
    {
        [field: SerializeField] protected ModalBackdrop Backdrop { get; private set; }
        [field: SerializeField] protected RectTransform Panel { get; private set; }

        #region Show/Hide

        protected async UniTask DefaultShowAsync()
        {
            gameObject.SetActive(true);
            // backdrop fade 작업
            // 동시에 panel 보여주기

            await UniTask.CompletedTask;
        }

        protected async UniTask DefaultHideAsync()
        {
            gameObject.SetActive(false);
            await UniTask.CompletedTask;
        }

        #endregion
    }
}