using UnityEngine;

namespace MaouSuika.Gameplay
{
    public partial class ComboRoot : MonoBehaviour
    {
        [SerializeField] private ComboPopupView comboPopupView;
        [SerializeField] private ComboIndicatorView comboIndicatorView;

        private Combo _combo;

        public int Current => _combo.CurrentValue;

        public void Setup()
        {
            _combo?.Dispose();
            _combo = new Combo();
            comboIndicatorView.Setup(_combo.Current);
        }

        private void OnDestroy()
        {
            _combo?.Dispose();
        }

        public void CheckPoint() => _combo.CheckPoint();

        public int Add(Vector2 position)
        {
            var combo = _combo.Add();
            comboPopupView.Play(position, combo);
            return combo;
        }
    }
}
