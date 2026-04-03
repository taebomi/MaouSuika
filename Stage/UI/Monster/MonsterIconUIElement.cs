using SOSG.Monster;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.UI
{
    public class MonsterIconUIElement : MonoBehaviour
    {
        [SerializeField] private Image outlineImage;
        [SerializeField] private Image insideImage;
        [SerializeField] private Image monsterIconImage;
    
        public void Set(MonsterDataSO data)
        {
            outlineImage.color = data.capsuleColor;
            insideImage.color = data.capsuleColor;
            monsterIconImage.sprite = data.icon;
        }
    }
}