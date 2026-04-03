namespace TBM.MaouSuika.Core.Scene
{
    /// <summary>
    /// 기본 설정이 아닌 커스텀 연출과 Payload 전달 용도 
    /// </summary>
    public struct SceneLoadOptions
    {
        public TransitionType? TransitionIn;
        public TransitionType? TransitionOut;
        public SceneLoadPayload Payload;
    }
}