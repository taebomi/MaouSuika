using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Bootstrap;
using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public abstract class SceneControllerBase : MonoBehaviour
    {
        public abstract UniTask InitializeSceneAsync(object initPayload);
        public abstract void ProcessScene();
    }
}