using MaouSuika.Core;
using UnityEngine;

namespace MaouSuika.Gameplay
{
    [CreateAssetMenu(fileName = "SkillDefinitionSO", menuName = MenuPath.Gameplay.SKILL + "Definition")]
    public class SkillDefinitionSO : ScriptableObject
    {
        [SerializeField] private string id;
        [Min(1)]
        [SerializeField] private int requiredCharge = 1000;

        public string Id => id;
        public int RequiredCharge => requiredCharge;
    }
}