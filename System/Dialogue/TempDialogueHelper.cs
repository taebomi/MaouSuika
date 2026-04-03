using System;
using Cysharp.Threading.Tasks;
using SOSG.System.Localization;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.System.Dialogue
{
    // [RequireComponent(typeof(LocalizationHelper))]
    public class TempDialogueHelper : MonoBehaviour
    {
        public Action<ConversationData> OnLineFinished;
        public Action<string[]> OnLineEventRaised;

        // private LocalizationHelper _localizationHelper;

        public string GetLocalizedValue(string key)
        {
            // return _localizationHelper.GetLocalizedValue(key);
            return String.Empty;
        }

        private void Awake()
        {
            // _localizationHelper = GetComponent<LocalizationHelper>();
        }

        public void RequestLine(string key)
        {
            // var lineString = _localizationHelper.GetLocalizedValue(key);
            // DialogueEventChannel.RequestLine(lineString);
        }

        public void RequestLine(string key, (string, string) args)
        {
            // var lineString = _localizationHelper.GetLocalizedValue(key, args);
            // DialogueEventChannel.RequestLine(lineString);
        }

        public async UniTask<int> RequestChoiceAsync(string choiceKey)
        {
            // var localizedValue = _localizationHelper.GetLocalizedValue(choiceKey);
            // return await DialogueEventChannel.RequestChoiceAsync(localizedValue);
            return 0;
        }

        public void SetPortraitActive(bool value) => DialogueHelper.SetPortraitActive(value);

        public void SetListener(bool value)
        {
            
        }
    }
}