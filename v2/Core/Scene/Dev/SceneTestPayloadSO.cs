using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public abstract class SceneTestPayloadSO : ScriptableObject
    {
        public abstract object CreatePayload();
    }
}