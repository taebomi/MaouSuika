using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SOSG.System.Dialogue
{
    public class LineBoxPortrait : MonoBehaviour
    {
        [SerializeField] private Image portraitImage;

        [SerializeField] private Sprite[] emotionSpriteArr;

        private OverlordEmotion _curEmotion;

        private int _lastSpriteIdx;

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public void ResetPortrait()
        {
            SetEmotion(OverlordEmotion.Normal);
            CloseMouth();
        }

        public void CloseMouth()
        {
            portraitImage.sprite = emotionSpriteArr[(int)_curEmotion * 3];
            _lastSpriteIdx = 0;
        }

        public void ShowRandomSprite()
        {
            int randomIdx;
            do
            {
                randomIdx = Random.Range(0, 3);
            } while (randomIdx == _lastSpriteIdx);

            portraitImage.sprite = emotionSpriteArr[(int)_curEmotion * 3 + randomIdx];
            _lastSpriteIdx = randomIdx;
        }

        public void SetEmotion(string emotion)
        {
            _curEmotion = emotion switch
            {
                "happy" => OverlordEmotion.Happy,
                "sad" => OverlordEmotion.Sad,
                "angry" => OverlordEmotion.Angry,
                "surprised" => OverlordEmotion.Surprised,
                "normal" => OverlordEmotion.Normal,
                "pathetic" => OverlordEmotion.Pathetic,
                _ => throw new ArgumentOutOfRangeException(nameof(emotion), emotion, null)
            };
        }

        public void SetEmotion(OverlordEmotion emotion) => _curEmotion = emotion;
    }
}