using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class SoulOrbTargetNavigator
    {
        private const float MIN_ALIGNMENT = 0.5f;

        private readonly ISoulOrbSpawner _spawner;
        private readonly LayerMask _layerMask;

        public SoulOrbTargetNavigator(LayerMask layerMask, ISoulOrbSpawner spawner)
        {
            _layerMask = layerMask;
            _spawner = spawner;
        }

        public SoulOrb ResolveInitial()
        {
            var activeOrbs = _spawner.ActiveOrbs;
            for (var i = activeOrbs.Count - 1; i >= 0; i--)
            {
                var candidate = activeOrbs[i];
                if (candidate.CanBeTargeted) return candidate;
            }

            return null;
        }

        public SoulOrb ResolveAt(Vector2 worldPosition)
        {
            var collider = Physics2D.OverlapPoint(worldPosition, _layerMask);
            if (collider == null) return null;

            if (!collider.TryGetComponent<SoulOrb>(out var orb)) return null;
            return orb.CanBeTargeted ? orb : null;
        }

        public SoulOrb ResolveNext(SoulOrb current, Vector2 direction)
        {
            if (current == null || !current.CanBeTargeted) return ResolveInitial();


            var normalizedDirection = direction.normalized;

            SoulOrb closest = null;
            var closestDistanceSqr = float.MaxValue;

            foreach (var candidate in _spawner.ActiveOrbs)
            {
                if (candidate == current) continue;
                if (!candidate.CanBeTargeted) continue;

                var offset = candidate.RbPosition - current.RbPosition;
                var distanceSqr = offset.sqrMagnitude;
                if (distanceSqr <= Mathf.Epsilon) continue;

                var alignment = Vector2.Dot(normalizedDirection, offset.normalized);
                if (alignment < MIN_ALIGNMENT) continue;

                if (distanceSqr >= closestDistanceSqr) continue;

                closest = candidate;
                closestDistanceSqr = distanceSqr;
            }

            return closest ?? current;
        }
    }
}