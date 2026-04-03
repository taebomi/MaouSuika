using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.System.Dialogue
{
    [RequireComponent(typeof(RectTransform), typeof(Button))]
    public class ChoiceButton : MonoBehaviour
    {
        private RectTransform _rt;
        private Button _btn;

        public UniTask OnClickAsync(CancellationToken ct) => _btn.OnClickAsync(ct);

        private void Awake()
        {
            _rt = transform as RectTransform;
            _btn = GetComponent<Button>();
        }

        public void SetPosition(Vector2 pos)
        {
            _rt.anchoredPosition = pos;
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void SetText(string text)
        {
            var tmp = GetComponentInChildren<TMP_Text>();
            tmp.text = text;
        }
    }
}