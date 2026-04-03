using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaHighlightController : MonoBehaviour
    {
        [SerializeField] private GameObject outlineObject;

        private void Awake()
        {
            outlineObject.SetActive(false);
        }

        public void Show() => outlineObject.SetActive(true);
        public void Hide() => outlineObject.SetActive(false);
    }
}
