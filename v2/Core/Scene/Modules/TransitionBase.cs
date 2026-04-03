using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public abstract class TransitionBase : MonoBehaviour
    {
        public async UniTask CoverAsync(CancellationToken token)
        {
            gameObject.SetActive(true);
            await PerformShowAsync(token);
        }

        public async UniTask RevealAsync(CancellationToken token)
        {
            await PerformHideAsync(token);
            gameObject.SetActive(false);
        }

        protected abstract UniTask PerformShowAsync(CancellationToken token);
        protected abstract UniTask PerformHideAsync(CancellationToken token);

#if UNITY_EDITOR
        [Button, DisableInEditorMode]
        private void Debug_Show() => CoverAsync(CancellationToken.None).Forget();

        [Button, DisableInEditorMode]
        private void Debug_Hide() => RevealAsync(CancellationToken.None).Forget();

#endif
    }
}