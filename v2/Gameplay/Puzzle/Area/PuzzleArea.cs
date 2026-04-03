using TBM.Extensions;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class PuzzleArea : MonoBehaviour
    {
        [SerializeField] private Camera cam;

        public bool IsInCamera(Vector2 screenPoint)
        {
            return cam.IsInside(screenPoint);
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPos)
        {
            return cam.ScreenToWorldPoint(screenPos);
        }

        public Vector3 WorldToScreenPoint(Vector3 worldPos)
        {
            return cam.WorldToScreenPoint(worldPos);
        }
    }
}