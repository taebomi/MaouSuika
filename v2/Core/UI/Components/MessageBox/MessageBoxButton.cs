using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace TBM.MaouSuika.Core.UI
{
    [RequireComponent(typeof(LocalizeStringEvent), typeof(Button))]
    public class MessageBoxButton : MonoBehaviour
    {
        public MessageBoxButtonAction Action { get; private set; }
        public event Action<MessageBoxButton> Clicked;

        // Components
        private LocalizeStringEvent _localizeStringEvent;
        private Button _button;

        private void Awake()
        {
            _localizeStringEvent = GetComponent<LocalizeStringEvent>();
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClicked);
        }

        public void Initialize(MessageBoxButtonOption option)
        {
            _localizeStringEvent.StringReference = option.Content;
            Action = option.Action;
            // todo color preset
            gameObject.SetActive(true);
        }

        private void OnClicked()
        {
            Clicked?.Invoke(this);
        }
    }
}