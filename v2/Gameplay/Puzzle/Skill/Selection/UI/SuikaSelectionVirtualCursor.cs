using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaSelectionVirtualCursor : MonoBehaviour
    {
        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);
        public void SetPosition(Vector3 worldPosition) => transform.position = worldPosition;
    }
}
