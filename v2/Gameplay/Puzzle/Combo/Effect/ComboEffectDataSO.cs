using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [CreateAssetMenu(fileName = "ComboEffectDataSO", menuName = "TBM/Combo/Effect Data")]
    public class ComboEffectDataSO : ScriptableObject
    {
        [SerializeField] private Color[] colors;
        [SerializeField] private float baseSize;
        [SerializeField] private float[] baseSizes;

        public Color GetColor(int combo)
        {
            return default;
        }
        
        public float GetBaseSize(int combo)
        {
            return default;
        }
    }
}