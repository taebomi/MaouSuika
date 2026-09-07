using DG.Tweening;
using MaouSuika.Core;
using TMPro;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public abstract class ComboPopupMotionSO : ScriptableObject
    {
        public abstract Sequence CreateSequence(Transform target, TMP_Text tmp, float holdTime);
    }
}