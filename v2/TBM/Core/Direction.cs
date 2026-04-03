using UnityEngine;

namespace TBM.Core.Direction
{
    // 4방향 전용 간단 enum
    public enum Direction4
    {
        Up = 0,
        Right = 1,
        Down = 2,
        Left = 3,
    }

    public static class Direction4Extensions
    {
        public static Vector2 ToVector2(this Direction4 dir) => dir switch
        {
            Direction4.Up => Vector2.up,
            Direction4.Right => Vector2.right,
            Direction4.Down => Vector2.down,
            Direction4.Left => Vector2.left,
            _ => Vector2.zero
        };

        // 반대 방향
        public static Direction4 Opposite(this Direction4 dir)
            => (Direction4)(((int)dir + 2) & 3);
    }
}