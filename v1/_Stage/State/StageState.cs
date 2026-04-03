
namespace SOSG.Stage.State
{
    public enum StageState
    {
        None,
        SetUp,
        Ready,
        Start, // 첫 시작
        Pause, // 일시정지
        Resume, // 재개
        GameOver,
    }
}