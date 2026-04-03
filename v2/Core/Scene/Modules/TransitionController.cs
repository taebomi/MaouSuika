using System.Threading;
using AYellowpaper.SerializedCollections;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public class TransitionController : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<TransitionType, TransitionBase> transitions;

        public async UniTask CoverAsync(TransitionType type, CancellationToken token)
        {
            if (type is TransitionType.None) return;

            if (!transitions.TryGetValue(type, out var transition))
            {
                Logger.Warning($"Transition {type} is not found.");
                return;
            }

            await transition.CoverAsync(token);
        }

        public async UniTask RevealAsync(TransitionType type, CancellationToken token)
        {
            if (type is TransitionType.None) return;

            if (!transitions.TryGetValue(type, out var transition))
            {
                Logger.Warning($"Transition {type} is not found.");
                return;
            }

            await transition.RevealAsync(token);
        }
    }
}