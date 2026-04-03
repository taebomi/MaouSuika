using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class AimVisualizer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bodySr;
        [SerializeField] private SpriteRenderer tailSr;

        public void HideAim()
        {
            if (!gameObject.activeSelf) return;

            gameObject.SetActive(false);
        }

        public void ShowAim(Vector2 direction, AimVisualData visualData)
        {
            if (!gameObject.activeSelf) gameObject.SetActive(true);

            transform.right = direction;
            bodySr.size = visualData.Size;
            bodySr.color = visualData.Color;
            tailSr.color = visualData.Color;
        }
    }
}