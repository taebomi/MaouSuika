namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public enum ShooterInputType
    {
        None,
        Classic, // 좌우 : 각도 조절, 상하 : 파워 조절
        Direct, // 입력값이 Aim
        Drag, // 드래그가 Aim
        VirtualCursor, // 원점에서 가상의 커서가 Aim
    }
}