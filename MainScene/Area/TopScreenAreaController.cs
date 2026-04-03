using Cysharp.Threading.Tasks;
using SOSG.Area;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SOSG.MainScene
{
    public class TopScreenAreaController : MonoBehaviour
    {
        [SerializeField] private IntEventSO bgmTimingEventSO;
        [SerializeField] private FloatVarSO overlordMoveXSpeedVarSO;

        private GameObject _loadedArea;

        private BattleArea _battleArea;

        private const float InitPosY = -12f;

        private void Awake()
        {
            transform.DestroyAllChildren();

            bgmTimingEventSO.OnEventRaised += OnBgmTimingEventRaised;
        }

        private void OnDestroy()
        {
            bgmTimingEventSO.OnEventRaised -= OnBgmTimingEventRaised;

            if (_loadedArea)
            {
                Destroy(_battleArea.gameObject);
                Addressables.Release(_loadedArea);
            }
        }

        public async UniTask LoadArea(AreaData areaData)
        {
            _loadedArea = await Addressables.LoadAssetAsync<GameObject>(areaData.battleAreaRef);
            _battleArea = Instantiate(_loadedArea, transform).GetComponent<BattleArea>();
            _battleArea.Initialize(areaData, overlordMoveXSpeedVarSO);
            if (!MainSceneManager.SkipIntro)
            {
                _battleArea.InitializePosition(new Vector3(0f, InitPosY));
            }
            else
            {
                _battleArea.ScrollArea().Forget();
            }
        }

        private void OnBgmTimingEventRaised(int timing)
        {
            _battleArea.OnMainSceneEvent(timing);
        }
    }
}