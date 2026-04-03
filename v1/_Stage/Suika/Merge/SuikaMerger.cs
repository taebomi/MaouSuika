using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using FMODUnity;
using SOSG.Stage.Suika.Combo;
using SOSG.Monster;
using SOSG.Stage.Suika.Merge;
using SOSG.System.Audio;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika
{
    public class SuikaMerger : MonoBehaviour
    {
        [SerializeField] private SuikaMergeEffectPoolSO effectPoolSO;

        [SerializeField] private SuikaCreator suikaCreator;
        [SerializeField] private Combo.ComboSystem comboSystem;

        [SerializeField] private EventReference mergeSfx;

        public event Action<MergedInfo> Merged;

        public struct MergedInfo
        {
            public Vector3 Pos;
            public float Size;
            public int Tier;
        }


        public void Merge(SuikaObject suika1, SuikaObject suika2)
        {
            if (suika1.CreationOrder < suika2.CreationOrder)
            {
                MergeAsync(suika2, suika1).Forget();
            }
            else
            {
                MergeAsync(suika1, suika2).Forget();
            }
        }

        private async UniTaskVoid MergeAsync(SuikaObject from, SuikaObject to)
        {
            var creationOrder = to.CreationOrder;

            from.OnMerging();
            to.OnMerging();

            await MoveSuika(from, to, destroyCancellationToken);

            from.Deactivate();
            to.Deactivate();

            var curTier = from.Tier;
            var newTier = Mathf.Clamp(curTier + 1, SuikaUtility.MinTier, SuikaUtility.MaxTier);
            var newPos = (Vector3)to.PhysicsComponent.Rb.position;
            var size = 0f;

            PlayMergeSfx();
            
            // Create Merge Effect
            var grade = MonsterUtility.GetGrade(curTier);
            var effect = effectPoolSO.Get(grade, newPos);
            effect.SetColor(to.Data.mergeEffectColor);
            effect.SetSize(to.Size);

            if (curTier is not SuikaUtility.MaxTier)
            {
                // Create New Suika
                var newSuika = suikaCreator.GetSuika(newTier, newPos, creationOrder);
                newSuika.OnMerged();
                size = newSuika.Size;
            }

            Merged?.Invoke(new MergedInfo
            {
                Pos = newPos,
                Size = size,
                Tier = curTier,
            });
        }

        private async UniTask MoveSuika(SuikaObject from, SuikaObject to, CancellationToken ct)
        {
            var timer = 0f;
            const float duration = 0.25f;
            var fromRb = from.PhysicsComponent.Rb;
            var startPos = fromRb.position;
            var destPos = to.PhysicsComponent.Rb.position;

            while (timer < duration && ct.IsCancellationRequested is false)
            {
                var easing = Easing.InSine(timer, duration);
                fromRb.position = Vector2.Lerp(startPos, destPos, easing);
                timer += Time.deltaTime;
                await UniTask.Yield(ct);
            }
        }


        private void PlayMergeSfx()
        {
            var pitch = 0.65f + comboSystem.CurCombo * 0.1f;
            AudioSystemHelper.PlaySfx(mergeSfx, pitch);
        }
    }
}