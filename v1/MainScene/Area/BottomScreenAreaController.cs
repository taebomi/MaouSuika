using Cysharp.Threading.Tasks;
using SOSG.Stage.Area;
using UnityEngine;
using UnityEngine.AddressableAssets;
using AreaData = SOSG.Area.AreaData;

namespace SOSG.MainScene
{
    public class BottomScreenAreaController : MonoBehaviour
    {
        private GameObject _loadedArea;
        private GashaponArea _gashaponArea;

        private void Awake()
        {
            transform.DestroyAllChildren();
        }

        private void OnDestroy()
        {
            if (_loadedArea)
            {
                Addressables.Release(_loadedArea);
            }
        }

        public async UniTask LoadArea(AreaData areaData)
        {
            _loadedArea = await Addressables.LoadAssetAsync<GameObject>(areaData.gashaponAreaRef);
            Instantiate(_loadedArea, transform);
        }
    }
}