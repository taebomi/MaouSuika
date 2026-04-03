using TBM.MaouSuika.Gameplay.Battle;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Battle
{
    public class HeroSystem : MonoBehaviour
    {
        [SerializeField] private HeroObject hero;

        public void Setup()
        {
            hero.Setup();
        }
    }
}