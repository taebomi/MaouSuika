using System;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    [Serializable]
    public struct DragAimConfig
    {
        public float dragRange;
        public bool isSlingshotMode;

        public static readonly DragAimConfig Default = new(3f, false);

        public DragAimConfig(float dragRange, bool isSlingshotMode)
        {
            this.dragRange = dragRange;
            this.isSlingshotMode = isSlingshotMode;
        }
    }

    [Serializable]
    public struct DirectAimConfig
    {
        public bool isSlingshotMode;

        public static readonly DirectAimConfig Default = new(false);

        public DirectAimConfig(bool isSlingshotMode)
        {
            this.isSlingshotMode = isSlingshotMode;
        }
    }

    [Serializable]
    public struct ClassicAimConfig
    {
        public float angleChangeSpeed;
        public float powerChangeSpeed;

        public static readonly ClassicAimConfig Default = new(120f, 2f);

        public ClassicAimConfig(float angleChangeSpeed, float powerChangeSpeed)
        {
            this.angleChangeSpeed = angleChangeSpeed;
            this.powerChangeSpeed = powerChangeSpeed;
        }
    }

    [Serializable]
    public struct VirtualCursorAimConfig
    {
        public float cursorMoveSpeed;

        public static readonly VirtualCursorAimConfig Default = new(2f);

        public VirtualCursorAimConfig(float cursorMoveSpeed)
        {
            this.cursorMoveSpeed = cursorMoveSpeed;
        }
    }
}