using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.System;
using SOSG.System.Scene;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SOSG.MainScene
{
    public class TitleScreenMapManager : MonoBehaviour
    {
        [SerializeField] private IntEventSO titleTimingEventSO;

        [SerializeField] private AssetReference[] areaRefArr;

        private TitleScreenMap _map;
        private GameObject _mapLoaded;


        private void Awake()
        {
            transform.DestroyAllChildren();

             SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnDestroy()
        {
            titleTimingEventSO.OnEventRaised -= OnTitleTimingEventRaised;

            Addressables.Release(_mapLoaded);
        }

        private async UniTask SetUpAsync()
        {
            var randomAreaRef = areaRefArr[UnityEngine.Random.Range(0, areaRefArr.Length)];
            await LoadArea(randomAreaRef);
            if (!MainSceneManager.SkipIntro)
            {
                titleTimingEventSO.OnEventRaised += OnTitleTimingEventRaised;
            }
            else
            {
                _map.ScrollMap().Forget();
            }
        }

        private void OnTitleTimingEventRaised(int timing)
        {
            if (timing == 0)
            {
                _map.MoveUpNearMap();
            }
            else if (timing == 1)
            {
                _map.MoveUpFarMap();
            }
            else if (timing == 2)
            {
                _map.MoveUpClouds();
            }
            else if (timing == 10)
            {
                _map.ScrollMap().Forget();
            }
        }


        private async UniTask LoadArea(AssetReference areaRef)
        {
            _mapLoaded = await Addressables.LoadAssetAsync<GameObject>(areaRef);

            var tr = transform;
            var pos = tr.position;
            _map = Instantiate(_mapLoaded, pos, Quaternion.identity, tr).GetComponent<TitleScreenMap>();
            _map.SetMainScenePosition(!MainSceneManager.SkipIntro);
        }
    }
}