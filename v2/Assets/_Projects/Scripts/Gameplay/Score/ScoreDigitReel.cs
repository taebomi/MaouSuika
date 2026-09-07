using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class ScoreDigitReel : MonoBehaviour
    {
        [SerializeField] private RectTransform strip;
        [SerializeField] private float cellSize;

        public void SetDigit(float digit)
        {
            strip.anchoredPosition = new Vector2(0f, (10f - digit) * cellSize);
        }

        public void SetSize(float size)
        {
            transform.localScale = new Vector3(size, size, size);
        }
    }
}