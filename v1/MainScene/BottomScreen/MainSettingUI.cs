using Cysharp.Threading.Tasks;
using DG.Tweening;
using SOSG.System.UI;
using SOSG.UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.MainScene
{
    public class MainSetting : MonoBehaviour, IModalUI
    {
        [field:SerializeField] public Canvas Canvas { get; private set; }

        [SerializeField] private SettingUI settingUI;

        public bool CanInteract { get; private set; }

        private void Awake()
        {
            settingUI.DeactivateEvent = Deactivate;
        }


        public void Show() => ShowAsync().Forget();

        public async UniTaskVoid ShowAsync()
        {
            CanInteract = false;
            settingUI.Show();
            gameObject.SetActive(true);
            // modalUI.Show();
            await transform.SOSGUIPopUp().Play().WithCancellation(settingUI.destroyCancellationToken);
            // settingUI.OnShowFinished();
            CanInteract = true;
        }

        public void Deactivate()
        {
            CanInteract = false;
            // modalUI.Hide();
            gameObject.SetActive(false);
        }

        public void OnOverlayClicked()
        {
            settingUI.OnCancelRequested();
        }
    }
}