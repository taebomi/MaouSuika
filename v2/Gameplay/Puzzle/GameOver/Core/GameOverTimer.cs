namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class GameOverTimer
    {
        public float Duration { get; }
        public float RemainedTime { get; private set; }

        public GameOverTimer(float duration)
        {
            Duration = duration;
        }

        public void Reset()
        {
            RemainedTime = Duration;
        }

        /// <summary>
        /// 타이머 완료 시 true 반환
        /// </summary>
        public bool Tick(float deltaTime)
        {
            RemainedTime -= deltaTime;
            if (RemainedTime > 0f) return false;

            RemainedTime = 0f;
            return true;
        }


        public override string ToString()
        {
            var text = string.Empty;
            text += $"[{RemainedTime:F1}s/{Duration}s]";
            return text;
        }
    }
}