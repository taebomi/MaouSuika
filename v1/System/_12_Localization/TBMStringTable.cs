using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace SOSG.System.Localization
{
    public class TBMStringTable : MonoBehaviour
    {
        private LocalizedStringTable _localizedStringTable;
        private StringTable _stringTable;

        private static readonly Dictionary<string, string> Args = new();

        public event Action TableReloaded;

        public async UniTask SetUpAsync(LocalizationTableName tableName)
        {
            var tcs = new UniTaskCompletionSource();

            _localizedStringTable = new LocalizedStringTable(tableName.ToString());
            _localizedStringTable.TableChanged += UpdateStringLocalizedStringTable;
            _localizedStringTable.TableChanged += OnLocalizedStringTableLoadFinished;
            await tcs.Task;
            return;

            void OnLocalizedStringTableLoadFinished(StringTable _)
            {
                tcs.TrySetResult();
                _localizedStringTable.TableChanged -= OnLocalizedStringTableLoadFinished;
            }
        }

        private void OnDestroy()
        {
            if (_localizedStringTable != null)
            {
                _localizedStringTable.TableChanged -= UpdateStringLocalizedStringTable;
            }
        }

        private void UpdateStringLocalizedStringTable(StringTable table)
        {
            _stringTable = table;
            TableReloaded?.Invoke();
        }

        public string GetLocalizedString(string key)
        {
            return _stringTable.GetEntry(key).GetLocalizedString();
        }

        public string GetLocalizedString(string key, (string, string) args)
        {
            Args.Clear();
            Args.Add(args.Item1, args.Item2);
            return _stringTable.GetEntry(key).GetLocalizedString(Args);
        }

        public void PrintConversation(string key)
        {
            var conversationData = GetConversationData(key);
            DialogueHelper.PrintConversation(conversationData);
        }
        
        public ConversationData GetConversationData(string key)
        {
            var entry = _stringTable.GetEntry(key);
            return new ConversationData(entry.KeyId, entry.GetLocalizedString());
        }


        public async UniTask<int> ShowChoiceAsync(string questionKey, string answerKey)
        {
            var questionEntry = _stringTable.GetEntry(questionKey);
            var question = new ConversationData(questionEntry.KeyId, questionEntry.GetLocalizedString(), false);
            
            var answerString = _stringTable.GetEntry(answerKey).GetLocalizedString();
            var answer = answerString.Split('\n');
            
            var choiceData = new ChoiceData(question, answer);
            return await DialogueHelper.ShowChoiceAsync(choiceData);
        }
    }
}