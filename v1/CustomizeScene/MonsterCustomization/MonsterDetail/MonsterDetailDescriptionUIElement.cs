using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterDetailDescriptionUIElement : MonoBehaviour
    {
        [SerializeField] private GameObject descriptionTextContainerGameObject;
        [SerializeField] private GameObject notUnlockedDescGameObject;
        
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Scrollbar scrollbar;

        public void Initialize(string notUnlockedText)
        {
            notUnlockedDescGameObject.GetComponent<TMP_Text>().text = notUnlockedText;
        }
        
        public void SetDescription(string description)
        {
            descriptionTextContainerGameObject.SetActive(true);
            notUnlockedDescGameObject.SetActive(false);
            descriptionText.text = description;
            scrollbar.value = 1f;
        }

        public void SetNotUnlocked()
        {
            descriptionTextContainerGameObject.SetActive(false);
            notUnlockedDescGameObject.SetActive(true);
        }
    }
}