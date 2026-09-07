using System;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [RequireComponent(typeof(Collider2D))]
    public class WarningZone : MonoBehaviour
    {
        [SerializeField] private int bufferSize = 5;
        [SerializeField] private float checkInterval = 0.25f;

        private BoxCollider2D _zoneCollider;

        private float _timer;

        private ContactFilter2D _soulOrbFilter;
        private Collider2D[] _soulOrbs;

        public bool IsOccupied { get; private set; }

        private void Awake()
        {
            _zoneCollider = GetComponent<BoxCollider2D>();
            _soulOrbFilter = new ContactFilter2D()
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("SoulOrb"),
                useTriggers = false,
            };
            _soulOrbs = new Collider2D[bufferSize];
        }

        public void Setup()
        {
            IsOccupied = false;
            _timer = 0f;
        }

        public void Tick(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer < checkInterval) return;
            _timer = 0f;

            IsOccupied = ContainsLandedSoulOrb();
        }

        private bool ContainsLandedSoulOrb()
        {
            var count = _zoneCollider.Overlap(_soulOrbFilter, _soulOrbs);
            if (count <= 0) return false;

            for (var i = 0; i < count; i++)
            {
                if (!_soulOrbs[i].TryGetComponent<SoulOrb>(out var soulOrb)) continue;

                if (soulOrb.HasLanded) return true;
            }

            return false;
        }
    }
}