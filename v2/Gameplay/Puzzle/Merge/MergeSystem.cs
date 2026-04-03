using System;
using System.Collections;
using TBM.MaouSuika.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public delegate void MergeActionHandler(SuikaObject from, SuikaObject to, MergeEvent mergeEvent);

    public class MergeSystem : MonoBehaviour
    {
        [SerializeField] private MergeFeedbackController feedbackController;

        private readonly MergeActionController _actionController = new();
        private readonly MergeValidator _validator = new();

        public event MergeActionHandler MergeActionFinished;

        public void Setup()
        {
            _validator.Setup();
        }

        public void RequestMerge(SuikaObject suika1, SuikaObject suika2)
        {
            if (!_validator.CanMerge(suika1, suika2)) return;

            StartCoroutine(ProcessMergeRoutine(suika1, suika2));
        }

        public void PlayMergeFeedback(MergeEvent evt, int comboCount)
        {
            feedbackController.Play(evt, comboCount);
        }

        private IEnumerator ProcessMergeRoutine(SuikaObject suika1, SuikaObject suika2)
        {
            _validator.Lock(suika1, suika2);

            var (sourceSuika, targetSuika) = GetMergeParticipants(suika1, suika2);
            sourceSuika.SetState_MergeSource();
            targetSuika.SetState_MergeTarget();

            yield return _actionController.PlayMergeAnimationRoutine(sourceSuika, targetSuika);

            _validator.Unlock(suika1, suika2);

            DispatchMergeActionFinished(sourceSuika, targetSuika);
        }

        private void DispatchMergeActionFinished(SuikaObject sourceSuika, SuikaObject targetSuika)
        {
            var resultTier = targetSuika.Tier + 1;

            var mergeEvent = new MergeEvent
            {
                Pos = targetSuika.RbPos,
                Tier = targetSuika.Tier,
                CreationOrder = targetSuika.CreationOrder,
                Size = targetSuika.Size,
                IsGrounded = sourceSuika.IsGrounded || targetSuika.IsGrounded,

                EffectGrade = GameRule.Puzzle.Suika.ToMergeEffectGrade(resultTier),
                EffectColor = targetSuika.Data.MergeEffectColor,

                HasResult = targetSuika.Tier < GameRule.Puzzle.Suika.MAX_TIER,
                ResultTier = resultTier,
            };

            MergeActionFinished?.Invoke(sourceSuika, targetSuika, mergeEvent);
        }

        private (SuikaObject source, SuikaObject target) GetMergeParticipants(SuikaObject suika1, SuikaObject suika2)
        {
            if (suika1.CreationOrder == suika2.CreationOrder)
            {
                Logger.Error($"Suika creation order[{suika1.CreationOrder}] is same. ");
                return (suika1, suika2);
            }

            return suika1.CreationOrder > suika2.CreationOrder
                ? (suika1, suika2)
                : (suika2, suika1);
        }
    }
}