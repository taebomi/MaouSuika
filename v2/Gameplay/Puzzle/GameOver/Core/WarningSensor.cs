using TBM.Core;
using TBM.MaouSuika.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class WarningSensor : MonoBehaviour
    {
        private const int CHECK_SUIKA_NUM = 5;
        private const float CHECK_INTERVAL = 0.2f;

        [SerializeField] private BoxCollider2D zoneCollider;

        private StateDebouncer _debouncer;

        private float _checkedTime;

        private ContactFilter2D _suikaFilter;
        private Collider2D[] _suikaColliders;

        public bool IsDanger => _debouncer.CurrentState;


        private void Awake()
        {
            _suikaFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = Layers.SUIKA_MASK,
            };
            _suikaColliders = new Collider2D[CHECK_SUIKA_NUM];
        }

        public void Initialize(float setThreshold, float unsetThreshold)
        {
            _debouncer = new StateDebouncer(setThreshold, unsetThreshold);
        }

        public void ResetSensor()
        {
            _checkedTime = 0f;
            _debouncer.Setup(IsGroundedSuikaExists());
        }


        public void Tick(float deltaTime)
        {
            _checkedTime += deltaTime;
            if (_checkedTime < CHECK_INTERVAL) return;

            var isExists = IsGroundedSuikaExists();
            _debouncer.Update(isExists, _checkedTime);
            _checkedTime = 0f;
        }

        public override string ToString()
        {
            var text = string.Empty;
            if (_debouncer == null) return "Not Initialized.";

            text += $"State[{_debouncer.CurrentState}] / Time[{_debouncer.Timer}]";
            return text;
        }

        private bool IsGroundedSuikaExists()
        {
            var num = zoneCollider.Overlap(_suikaFilter, _suikaColliders);
            for (var i = 0; i < num; i++)
            {
                if (!_suikaColliders[i].TryGetComponent<SuikaObject>(out var suika))
                {
                    continue;
                }

                if (suika.IsGrounded)
                {
                    return true;
                }
            }

            return false;
        }
    }
}