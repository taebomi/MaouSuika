namespace MaouSuika.Gameplay
{
    public readonly struct CameraShakeRequest
    {
        public readonly float Strength;
        public readonly float Duration;

        public CameraShakeRequest(float strength, float duration)
        {
            Strength = strength;
            Duration = duration;
        }
    }
}