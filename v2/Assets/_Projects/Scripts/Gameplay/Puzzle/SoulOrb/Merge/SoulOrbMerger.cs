using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using TBM.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbMerger : MonoBehaviour
    {
        private const float MERGE_DURATION = 0.25f;

        private ISoulOrbSpawner _spawner;
        private CancellationTokenSource _mergeCts;

        private readonly Subject<SoulOrbMergeResult> _mergeFinished = new();

        public Observable<SoulOrbMergeResult> MergeFinished => _mergeFinished;

        private bool IsAcceptingMerges => _mergeCts is { IsCancellationRequested: false };


        public void Initialize(ISoulOrbSpawner spawner)
        {
            _spawner = spawner;
        }

        public void Setup()
        {
            _mergeCts.CancelAndDispose();
            _mergeCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
        }

        public void Stop()
        {
            _mergeCts?.Cancel();
        }

        private void OnDestroy()
        {
            _mergeFinished.Dispose();
            _mergeCts.CancelAndDispose();
            _mergeCts = null;
        }

        public bool TryMerge(SoulOrb orbA, SoulOrb orbB)
        {
            if (!IsAcceptingMerges) return false;
            if (orbA.Tier != orbB.Tier) return false;
            if (!orbA.IsMergeable || !orbB.IsMergeable) return false;

            var mergePair = SoulOrbMergePair.Resolve(orbA, orbB);

            mergePair.Absorbed.SetPhase(SoulOrbPhase.MergeAbsorbed);
            mergePair.Anchored.SetPhase(SoulOrbPhase.MergeAnchored);

            MergeAsync(mergePair).Forget();
            return true;
        }

        private async UniTaskVoid MergeAsync(SoulOrbMergePair mergePair)
        {
            await MergeAnimationAsync(mergePair);

            var result = mergePair.Tier == SoulOrbTiers.MAX_TIER
                ? SoulOrbMergeResult.Finale(mergePair)
                : SoulOrbMergeResult.Promote(mergePair);
            var creationOrder = mergePair.CreationOrder;
            var hasLanded = mergePair.HasLanded;

            _spawner.Despawn(mergePair.Absorbed);
            _spawner.Despawn(mergePair.Anchored);
            if (result.ResultTier is { } tier)
                _spawner.Spawn(tier, result.Position, creationOrder, hasLanded, SoulOrbSpawnMode.Field);
            _mergeFinished.OnNext(result);
        }

        private async UniTask MergeAnimationAsync(SoulOrbMergePair mergePair)
        {
            await mergePair.Absorbed.transform.DOMove(mergePair.MergePosition, MERGE_DURATION)
                .SetEase(Ease.InSine)
                .SetLink(mergePair.Absorbed.gameObject)
                .Play()
                .AwaitForComplete(TweenCancelBehaviour.KillAndCancelAwait, _mergeCts.Token);
        }
    }
}