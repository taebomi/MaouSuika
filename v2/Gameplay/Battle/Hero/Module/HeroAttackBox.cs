using System.Collections.Generic;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class HeroAttackBox : MonoBehaviour
    {
        private readonly List<IDamagable> _targets = new();

        public bool HasTargets => _targets.Count > 0;
        // 물리 멀티스레딩으로 인해 스냅샷 제공
        public IReadOnlyList<IDamagable> Targets => _targets.ToArray(); 

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                _targets.Add(target);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                _targets.Remove(target);
            }
        }
    }
}