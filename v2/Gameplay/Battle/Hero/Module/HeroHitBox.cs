using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class HeroHitBox : MonoBehaviour, IDamagable
    {
        [SerializeField] private HeroObject hero;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<IDamagable>(out var target))
            {
                hero.OnContacted(target);
            }
        }

        public void TakeDamage(int damage)
        {
            hero.TakeDamage(damage);
        }
    }
}