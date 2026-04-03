using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterLoaderModule : MonoBehaviour
    {
        [SerializeField] private ShooterLoaderConfigSO style;

        private SuikaObject _loadedSuika;
        private SuikaSystem _suikaSystem;
        private SuikaQueueSystem _queueSystem;

        public bool IsLoaded => _loadedSuika != null;

        public void Initialize(SuikaSystem suikaSystem, SuikaQueueSystem queueSystem)
        {
            _suikaSystem = suikaSystem;
            _queueSystem = queueSystem;
        }

        public float LoadedRadius
        {
            get
            {
                if (IsLoaded) return _loadedSuika.Radius;

                Logger.Error($"Loaded Suika Not Exists.");
                return 0f;
            }
        }

        public void LoadNext()
        {
            if (IsLoaded)
            {
                Logger.Warning($"Already Loaded.");
                return;
            }

            var nextTier = _queueSystem.Dequeue();
            _loadedSuika = _suikaSystem.Spawn(transform.position, nextTier);
            _loadedSuika.SetState_Loaded();
        }

        public SuikaObject ExtractForFire()
        {
            var suika = _loadedSuika;
            _loadedSuika = null;
            return suika;
        }

        public void Tick(float deltaTime, ShooterState state)
        {
            if (!IsLoaded) return;

            if (state == ShooterState.None)
            {
                _loadedSuika.Rotate(style.RotationSpeed * deltaTime);
            }
        }

        public void Clear()
        {
            if (!IsLoaded) return;

            _suikaSystem.Despawn(_loadedSuika);
            _loadedSuika = null;
        }

        public void UpdateVisual(ShooterState state)
        {
            if (!IsLoaded) return;

            var visualData = style.Evaluate(_loadedSuika.MonsterData.suikaColor, state);
            _loadedSuika.SetColor(visualData.CoreColor, visualData.DetailColor);
        }
    }
}