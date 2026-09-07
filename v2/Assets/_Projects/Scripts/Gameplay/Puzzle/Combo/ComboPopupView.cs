using TBM.Pool;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboPopupView : MonoBehaviour
    {
        [SerializeField] private ComboStyleConfigSO styleConfig;
        [SerializeField] private ComboPopupMotionSO normalMotion;
        [SerializeField] private ComboPopupMotionSO highMotion;
        [SerializeField] private ComboPopupMotionSO maxMotion;

        [SerializeField] private ComboPopup prefab;

        private MonoPool<ComboPopup> _pool;

        private void Awake()
        {
            _pool = new SelfReleasingPool<ComboPopup>(prefab, transform, 10);
        }

        public void Play(Vector2 position, int combo)
        {
            var grade = ComboGrades.GradeOf(combo);
            if (grade is ComboGrade.None) return;

            var motion = MotionFor(grade);
            var style = styleConfig.PopupStyleFor(combo, grade);
            var text = grade is ComboGrade.Max ? $"<rainb>×{combo}</rainb>" : $"×{combo}";

            var request = new ComboPopupRequest(position, text, motion, style);
            var popup = _pool.Get();
            popup.Play(request);
        }

        private ComboPopupMotionSO MotionFor(ComboGrade grade)
        {
            return grade switch
            {
                ComboGrade.Normal => normalMotion,
                ComboGrade.High => highMotion,
                ComboGrade.Max => maxMotion,
                _ => null,
            };
        }
        
    }
}
