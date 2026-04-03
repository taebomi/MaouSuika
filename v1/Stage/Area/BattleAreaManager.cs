using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.Stage.Area
{
    // todo? - 맵 생성 시 어드레서블 사용?
    public class BattleAreaManager : MonoBehaviour
    {
        [Header("SO")]
        [SerializeField] private BiomeTransitionDBSO biomeTransitionDBSO;

        [SerializeField] private BattleStateSO battleStateSO;
        [SerializeField] private StageAreaSO stageAreaSO;
        
        [SerializeField] private Vector3VarSO overlordMoveSpeedVarSO;

        private BattleArea _curBattleArea;
        private BattleArea _nextBattleArea;
        private BattleArea _transitionBattleArea;

        private bool _isAreaChanged;

        private CancellationTokenSource _destroyCts;
        
        

        private void Awake()
        {
            transform.DestroyAllChildren();
            _destroyCts = new CancellationTokenSource();
            battleStateSO.OnStateChanged += OnBattleSceneStateChanged;
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
            battleStateSO.OnStateChanged -= OnBattleSceneStateChanged;
        }

        /// <summary>
        /// 초기 맵 생성
        /// </summary>
        /// <param name="areaData"></param>
        public void Initialize(AreaData areaData)
        {
            var curAreaPrefab = areaData.battleAreaPrefab;
            var pos = new Vector3(-BattleArea.AreaWidth * 0.5f, BattleArea.AreaYPos);
            _curBattleArea = Instantiate(curAreaPrefab, pos, Quaternion.identity, transform);
            _nextBattleArea = Instantiate(curAreaPrefab, _curBattleArea.GetNextAreaPos(), Quaternion.identity,
                transform);
            _curBattleArea.Set(areaData);
            _nextBattleArea.Set(areaData);
        }

        private void OnBattleSceneStateChanged(BattleStateSO.State state)
        {
            if (state is BattleStateSO.State.Move)
            {
                Move().Forget();
            }
        }

        private void CreateNextArea()
        {
            var nextAreaData = stageAreaSO.GetNextAreaData();
            var nextAreaPos = _curBattleArea.GetNextAreaPos();
            _nextBattleArea = Instantiate(nextAreaData.battleAreaPrefab, nextAreaPos, Quaternion.identity, transform);
            _nextBattleArea.Set(nextAreaData);

            if (_curBattleArea.AreaData == nextAreaData)
            {
                return;
            }

            // 다음 area가 현재와 다른 area일 경우
            _isAreaChanged = true;
            if (_curBattleArea.AreaData.biomeType == nextAreaData.biomeType)
            {
                return;  
            }
            
            // 다음 area가 현재와 다른 biome일 경우
            CreateTransitionArea(nextAreaData, nextAreaPos);
        }

        private void CreateTransitionArea(AreaData areaData, Vector3 nextAreaPos)
        {
            var transitionAreaPrefab = biomeTransitionDBSO[areaData.biomeType];
            _transitionBattleArea = Instantiate(transitionAreaPrefab, nextAreaPos, Quaternion.identity, transform);
            _transitionBattleArea.Set(areaData);
        }

        private async UniTaskVoid Move()
        {
            while (battleStateSO.Value is BattleStateSO.State.Move &&
                   _destroyCts.IsCancellationRequested is false)
            {
                var translation = overlordMoveSpeedVarSO.Value * Time.deltaTime;

                _curBattleArea.Move(translation);
                _nextBattleArea.Move(translation);
                if (_transitionBattleArea)
                {
                    _transitionBattleArea.Move(translation);
                }

                var curAreaXPos = _curBattleArea.transform.position.x;
                const float xThreshold = -BattleArea.AreaWidth - 12f;
                if (curAreaXPos < xThreshold)
                {
                    if (_transitionBattleArea)
                    {
                        Destroy(_transitionBattleArea.gameObject);
                    }

                    Destroy(_curBattleArea.gameObject);
                    _curBattleArea = _nextBattleArea;
                    CreateNextArea();
                }

                if (_isAreaChanged && Overlord.XPos > _curBattleArea.GetAreaEndXPoint())
                {
                    _isAreaChanged = false;
                    stageAreaSO.SetCurAreaData(_nextBattleArea.AreaData);
                }

                await UniTask.Yield(_destroyCts.Token);
            }
        }
    }
}