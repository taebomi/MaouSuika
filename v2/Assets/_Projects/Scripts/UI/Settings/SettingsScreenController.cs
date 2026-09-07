using System;
using MaouSuika.Core;
using MaouSuika.Core.Save;
using MaouSuika.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace MaouSuika.UI
{
    public class SettingsScreenController : MonoBehaviour
    {
        [SerializeField] private Button saveButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button resetButton;

        [SerializeField] private SettingsView view;

        private SettingsData _beforeEdit;
        private SettingsData _draft;
        private Action _closed;

        private void Awake()
        {
            saveButton.onClick.AddListener(Save);
            cancelButton.onClick.AddListener(Cancel);
            resetButton.onClick.AddListener(ResetToDefaults);
        }

        public void Initialize(Action closed)
        {
            _closed = closed;
        }

        private void OnDestroy()
        {
            saveButton.onClick.RemoveListener(Save);
            cancelButton.onClick.RemoveListener(Cancel);
            resetButton.onClick.RemoveListener(ResetToDefaults);
        }

        public void Show()
        {
            _beforeEdit = SettingsManager.Instance.Capture();
            _draft = _beforeEdit.Copy();

            view.Bind(_draft);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            view.Unbind();
            gameObject.SetActive(false);
        }

        private void Save()
        {
            _draft.Normalize();

            SaveManager.Instance.SaveSettings(_draft);
            SettingsManager.Instance.Apply(_draft);

            Close();
        }

        private void Cancel()
        {
            SettingsManager.Instance.Apply(_beforeEdit);
            Close();
        }

        private void ResetToDefaults()
        {
            var localizationSettings = _draft.localizationSettings.Copy();
            _draft = new SettingsData
            {
                localizationSettings = localizationSettings,
            };

            view.Bind(_draft);
            AudioManager.Instance.ApplySettings(_draft.audioSettings);
        }

        private void Close()
        {
            _closed?.Invoke();
        }

        public bool TryHandleBack()
        {
            if (!gameObject.activeInHierarchy) return false;

            Cancel();
            return true;
        }
    }
}
