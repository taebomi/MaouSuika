using System.Collections;
using TBM.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class MergeActionController
    {
        public IEnumerator PlayMergeAnimationRoutine(SuikaObject source, SuikaObject target)
        {
            const float duration = 0.25f;
            const float invDuration = 1f / duration;

            var t = 0f;
            var sourcePos = source.transform.position;
            var targetTr = target.transform;

            while (t < 1f)
            {
                t += invDuration * Time.deltaTime;

                var clampedT = Mathf.Clamp01(t);
                var easeT = Easing.InSine(clampedT);

                var position = Vector3.Lerp(sourcePos, targetTr.position, easeT);
                source.SetVisualPosition(position);

                yield return null;
            }

            source.SetVisualPosition(targetTr.position);
        }
    }
}