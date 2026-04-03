using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Stage.Area;
using UnityEngine;

namespace SOSG.Stage.Map
{
    public class GashaponMapManager : MonoBehaviour
    {
        [SerializeField] private StageAreaSO stageAreaSO;
        
        [SerializeField] private Material curMat;
        [SerializeField] private Material nextMat;

        private GashaponArea _curArea;
        private GashaponArea _nextArea;

        private CancellationTokenSource _fadingCts;

        private void Awake()
        {
            transform.DestroyAllChildren();
            
            stageAreaSO.ActionOnAreaChanged += OnAreaChanged;
        }

        private void OnDestroy()
        {
            stageAreaSO.ActionOnAreaChanged -= OnAreaChanged;

            _fadingCts?.CancelAndDispose();
        }

        public void Initialize(AreaData areaData)
        {
            var curArea = areaData.gashaponAreaPrefab;
            _curArea = Instantiate(curArea, transform);
        }

        private void OnAreaChanged(AreaData areaData) => ChangeArea(areaData).Forget();

        private async UniTaskVoid ChangeArea(AreaData areaData)
        {
            _nextArea = Instantiate(areaData.gashaponAreaPrefab, transform);
            _curArea.SetMaterial(curMat);
            _nextArea.SetMaterial(nextMat);
            
            _fadingCts?.CancelAndDispose();
            _fadingCts = new CancellationTokenSource();
            await PlayFadingEffect(_fadingCts.Token);

            Destroy(_curArea.gameObject);
            _curArea = _nextArea;
            _nextArea = null;
        }

        private async UniTask PlayFadingEffect(CancellationToken ct)
        {
            var timer = 0f;
            const float duration = 1f;
            while (timer < duration && _fadingCts.IsCancellationRequested is false)
            {
                var ease = Easing.InOutSine(timer, duration);

                curMat.color = new Color(1f, 1f, 1f, 1f - ease);
                nextMat.color = new Color(1f, 1f, 1f, ease);

                _curArea.SetLightIntensity(1f - ease);
                _nextArea.SetLightIntensity(ease);


                timer += Time.deltaTime;
                await UniTask.Yield(ct);
            }

            curMat.color = new Color(1f, 1f, 1f, 0f);
            curMat.color = new Color(1f, 1f, 1f, 1f);
        }
    }
}