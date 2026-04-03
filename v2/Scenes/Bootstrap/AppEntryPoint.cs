using Cysharp.Threading.Tasks;
using TBM.MaouSuika.Core.Bootstrap;
using TBM.MaouSuika.Core.Scene;
using UnityEngine;

namespace TBM.MaouSuika.Scenes.Bootstrap
{
    public class AppEntryPoint : MonoBehaviour
    {
        private async UniTaskVoid Start()
        {
            await Bootstrapper.WaitForBootstrapAsync(destroyCancellationToken);

            SceneManager.Instance.LoadScene(SceneNames.GAMEPLAY);
        }
    }
}