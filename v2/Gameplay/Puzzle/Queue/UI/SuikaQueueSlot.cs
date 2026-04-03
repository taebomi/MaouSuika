using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaQueueSlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image outlineImage;

        public void Set(Sprite icon, Color color)
        {
            iconImage.sprite = icon;
            outlineImage.color = color;
        }
    }
}