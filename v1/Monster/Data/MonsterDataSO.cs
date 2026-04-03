using Cysharp.Threading.Tasks;
using SOSG.Area;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;

namespace SOSG.Monster
{
    /// <summary>
    /// Addressable로 관리되는 몬스터 데이터.
    /// MonsterDataSOLoader를 사용하여 로드할 것.
    /// </summary>
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Data/Monster", fileName = "MonsterDataSO", order = 9000)]
    public class MonsterDataSO : ScriptableObject
    {
        [Header("Default Data")]
        public string id;
        public MonsterGrade grade;
        public Biome habitat;

        [Header("Sprite Data")]
        public Vector3 bodyCenterPos;
        public float xMaxLength;
        public AnimatorOverrideController animatorOverrideController;

        public Sprite icon;
        public Sprite previewSprite;

        [Header("Gashapon Data")]
        public Color gashaponColor;
        public MergeEffectColor mergeEffectColor;
        public float minGashaponSize;

        [Header("Battle Data")]
        public Stats stats;

        [Header("should remove")]
        public Vector3 footPos;
        public float ySize;
        public float speed;

        public Color capsuleColor;

        public enum MergeEffectColor
        {
            Red,
            Yellow,
            Green,
            Blue,
            Purple,
        }
    }
}