using System;
using DG.Tweening;
using Febucci.UI;
using TBM.Pool;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [RequireComponent(typeof(TMP_Text))]
    public class ComboPopup : MonoBehaviour, ISelfReleasingPoolable
    {
        private TMP_Text _tmp;
        private TextAnimator_TMP _textAnimator;

        private Sequence _sequence;
        private Action _onComplete;

        public void Initialize(Action onComplete)
        {
            _tmp = GetComponent<TMP_Text>();
            _textAnimator = GetComponent<TextAnimator_TMP>();
            _onComplete = onComplete;
        }

        public void Play(ComboPopupRequest request)
        {
            var style = request.Style;
            
            transform.position = request.Position;
            transform.localScale = Vector3.one * style.Scale;
            transform.localRotation = Quaternion.identity;

            _textAnimator.SetText(request.Text);
            _tmp.color = style.Color;
            _tmp.alpha = 1f;

            _sequence = request.Motion.CreateSequence(transform, _tmp, style.HoldTime);
            _sequence
                .OnComplete(() => _onComplete())
                .SetLink(gameObject);
            gameObject.SetActive(true);
            _sequence.Play();
        }
    }
}