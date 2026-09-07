namespace MaouSuika.Gameplay
{
    public enum UnitFacing
    {
        Left,
        Right,
    }

    public static class UnitFacingExtensions
    {
        public static float ToSign(this UnitFacing facing) => facing is UnitFacing.Left ? -1 : 1;
    }
}