using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class GameOverRoot : MonoBehaviour
    {
        [SerializeField] private GameOverResultView resultView;

        private Func<CancellationToken, UniTask>[] _sequences;
        private Action _onRestartBtnClicked;

        public bool IsGameOver { get; private set; }

        public void Initialize(Func<CancellationToken, UniTask>[] sequences, Action onRestartBtnClicked)
        {
            _sequences = sequences;
            _onRestartBtnClicked = onRestartBtnClicked;

            resultView.Initialize(OnRestartButtonClicked);
        }

        public void Setup()
        {
            IsGameOver = false;
        }

        public async UniTask PlayAsync(CancellationToken token)
        {
            await UniTask.WhenAll(_sequences.Select(sequence => sequence(token)));
            resultView.SetVisible(true);
        }

        public void OnRestartButtonClicked()
        {
            _onRestartBtnClicked?.Invoke();
        }
    }
}