using TBM.MaouSuika.Core;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class ShooterBlockSensor
    {
        public bool IsBlocked { get; private set; }

        private readonly Collider2D[] _tempBuffer = new Collider2D[1];
        private readonly ContactFilter2D _contactFilter;

        // ReSharper disable once ConvertConstructorToMemberInitializers
        // LayerCache 마스크 초기화 순서 몰라서 생성자에서 초기화해줌
        public ShooterBlockSensor()
        {
            _contactFilter = new ContactFilter2D
            {
                useLayerMask = true,
                layerMask = LayerMask.GetMask("Suika"),
                useTriggers = false,
            };
        }

        public void UpdateBlockState(Vector3 pos, float radius)
        {
            var blockedCount = Physics2D.OverlapCircle(pos, radius, _contactFilter, _tempBuffer);
            IsBlocked = blockedCount > 0;
        }
    }
}