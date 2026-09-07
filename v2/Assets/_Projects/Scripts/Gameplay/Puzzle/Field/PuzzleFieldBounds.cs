using UnityEngine;

namespace MaouSuika.Gameplay
{
    public class PuzzleFieldBounds : MonoBehaviour
    {
        [SerializeField] private Collider2D leftWall;
        [SerializeField] private Collider2D rightWall;
        [SerializeField] private Transform spawnLine;

        public float SpawnY => spawnLine.position.y;

        public float ClampCenterX(float x, float radius)
        {
            var minX = leftWall.bounds.max.x + radius;
            var maxX = rightWall.bounds.min.x - radius;

            return Mathf.Clamp(x, minX, maxX);
        }
    }
}