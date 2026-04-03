using System;
using TBM.MaouSuika.Core.Scene;
using UnityEngine;

namespace TBM.MaouSuika.Scenes.Gameplay
{
    public class GameOverUI : MonoBehaviour
    {
        private event Action RetryButtonClicked;

        public void Show(Action onRetryButtonClicked)
        {
            RetryButtonClicked = onRetryButtonClicked;
            gameObject.SetActive(true);
        }

        public void OnRetryButtonClicked()
        {
            RetryButtonClicked?.Invoke();
        }
    }
}