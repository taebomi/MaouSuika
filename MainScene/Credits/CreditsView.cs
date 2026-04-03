using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.Localization;
using SOSG.System.Scene;
using SOSG.System.UI;

namespace SOSG.MainScene
{
    public class CreditsView : BaseUI, IModalUI
    {
        private TBMStringTable _stringTable;

        private bool _isShowing;

        protected override void AwakeAfter()
        {
            _isShowing = false;
            Canvas.enabled = false;
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private async UniTask SetUpAsync()
        {
            _stringTable = GetComponent<TBMStringTable>();
            await _stringTable.SetUpAsync(LocalizationTableName.MainScene);
        }

        public void OnOverlayClicked()
        {
            Hide();
        }


        public async UniTaskVoid ShowAsync()
        {
            _stringTable.PrintConversation("credits_open");
            UIHelper.ShowUI(this);
            Canvas.enabled = true;
            await transform.SOSGUIPopUp().Play().WithCancellation(destroyCancellationToken);
            _isShowing = true;
        }

        private void Hide()
        {
            if (_isShowing is false)
            {
                return;
            }

            UIHelper.HideUI(this);
            Canvas.enabled = false;
            _stringTable.PrintConversation("credits_close");
            _isShowing = false;
        }
    }
}