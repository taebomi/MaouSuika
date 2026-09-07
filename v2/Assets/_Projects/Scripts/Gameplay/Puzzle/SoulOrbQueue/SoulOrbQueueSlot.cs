using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MaouSuika.Gameplay
{
    public class SoulOrbQueueSlot : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image outlineImage;

        public void Set(SoulOrbQueueSlotData data)
        {
            iconImage.sprite = data.Icon;
            outlineImage.color = data.Color;
        }
    }
}