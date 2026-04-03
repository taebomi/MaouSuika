namespace TBM.MaouSuika.Core.Scene
{
    public class SceneLoadPayload
    {
        public readonly object LoadingData;
        public readonly object NextSceneData;

        public SceneLoadPayload(object loadingData, object nextSceneData)
        {
            LoadingData = loadingData;
            NextSceneData = nextSceneData;
        }
    }
}