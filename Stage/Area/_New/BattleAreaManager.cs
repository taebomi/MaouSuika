using Cysharp.Threading.Tasks;
using SOSG.Stage.Area;
using UnityEngine;
using UnityEngine.AddressableAssets;

// todo namespace Stage.Area로 옮기기
namespace SOSG.Area
{
    public class BattleAreaManager : MonoBehaviour
    {
        [SerializeField] private StageAreaManagerSO areaManagerSO;
        [SerializeField] private FloatVarSO overlordXMoveSpeedVarSO;

        private BattleArea _curArea;
        private BattleArea _nextArea;
        private GameObject _curLoadedArea;

        public const float WaitingAreaYPos = -12f;

        private void Awake()
        {
            transform.DestroyAllChildren();
        }

        public async UniTask Initialize(AreaData areaData)
        {
            _curLoadedArea = await Addressables.LoadAssetAsync<GameObject>(areaData.battleAreaRef);
            _curArea = Instantiate(_curLoadedArea, transform).GetComponent<BattleArea>();
            _curArea.Initialize(areaData, overlordXMoveSpeedVarSO);
            _curArea.ScrollArea().Forget();
        }

        public async UniTask CreateNextAreaAsync(AreaData newAreaData)
        {
            if (_curLoadedArea)
            {
                Addressables.Release(_curLoadedArea);
                _curLoadedArea = null;
            }

            _curLoadedArea = await Addressables.LoadAssetAsync<GameObject>(newAreaData.battleAreaRef);
            _nextArea = Instantiate(_curLoadedArea, transform).GetComponent<BattleArea>();
            _nextArea.Initialize(newAreaData, overlordXMoveSpeedVarSO);
            _nextArea.InitializePosition(new Vector3(0f, WaitingAreaYPos));
            _nextArea.ScrollArea().Forget();
        }

        // 현재 Area 아래로 숨기고, 다음 Area 위로 보여줌.
        public async UniTask ChangeAreaAsync()
        {
            await UniTask.WhenAll(_curArea.Disappear(), _nextArea.Appear());
            _curArea.Destroy();
            _curArea = _nextArea;
            _nextArea = null;
        }
    }
}