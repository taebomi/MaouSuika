using TBM.MaouSuika.Gameplay.Battle;
using TBM.MaouSuika.Gameplay.Puzzle;
using TBM.MaouSuika.Gameplay.Unit;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Monster
{
    [CreateAssetMenu(fileName = "MonsterDataSO", menuName = "TBM/Monster/Data")]
    public partial class MonsterDataSO : ScriptableObject
    {
        public string id;
        public MonsterGrade grade;

        public BattleMonster battlePrefab;
        public MonsterVisualController visualPrefab;

        public MonsterStats stats;
        
        public Vector2 centerOffset;
        public float size;

        public Sprite icon;

        public Color suikaColor;
        public MergeEffectColor mergeEffectColor;
    }
}