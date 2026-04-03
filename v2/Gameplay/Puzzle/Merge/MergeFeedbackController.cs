using FMODUnity;
using TBM.MaouSuika.Core;
using TBM.MaouSuika.Core.Audio;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class MergeFeedbackController : MonoBehaviour
    {
        [SerializeField] private SuikaMergeEffectPool mergeEffectPool;
        [SerializeField] private EventReference mergeSfx;

        public void Play(MergeEvent mergeEvent, int combo)
        {
            CreateMergeEffect(mergeEvent);
            PlayMergeSfx(combo);
        }

        private void PlayMergeSfx(int combo)
        {
            const float basePitch = 0.65f;
            const float pitchMultiplier = 0.1f;
            const float maxPitch = 1.5f;

            var pitch = basePitch + combo * pitchMultiplier;
            pitch = Mathf.Clamp(pitch, basePitch, maxPitch);
            AudioManager.Instance.PlaySfx(mergeSfx, pitch);
        }


        private void CreateMergeEffect(MergeEvent mergeEvent)
        {
            var effect = mergeEffectPool.Get(mergeEvent.EffectGrade);
            effect.transform.position = mergeEvent.Pos;
            effect.Setup(mergeEvent.EffectColor, mergeEvent.Size);
        }
    }
}