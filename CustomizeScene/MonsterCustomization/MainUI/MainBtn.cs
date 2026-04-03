using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace SOSG.Customization.Monster
{
    public class MainBtn : MonoBehaviour
    {
        [SerializeField] private TMP_Text tmp;

        private Mode _mode;

        public enum Mode
        {
            None,
            Equip,
            Unlock,
        }

        private event Action<Mode> OnClicked;

        private void Awake()
        {
            _mode = Mode.None;
        }

        public void Initialize(Action<Mode> onClicked)
        {
            OnClicked = onClicked;
        }

        public async UniTaskVoid Activate(Mode mode)
        {
            if (gameObject.activeSelf && _mode == mode)
            {
                return;
            }

            _mode = mode;
            gameObject.SetActive(true);
            await transform.SOSGUIPopUp().Play();
        }
        public void SetText(string text)
        {
            tmp.text = text;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }


        public void OnBtnClicked()
        {
            OnClicked?.Invoke(_mode);
        }
    }
}