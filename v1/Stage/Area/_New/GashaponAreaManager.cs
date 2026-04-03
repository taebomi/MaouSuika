using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

// todo namespace Stage.Area로 옮기기
namespace SOSG.Area
{
    public class GashaponAreaManager : MonoBehaviour
    {
        private GashaponArea _curArea;
        private GashaponArea _nextArea;
        private GameObject _curLoadedArea;

        [SerializeField] private Material curMat;
        [SerializeField] private Material nextMat;

        private CancellationTokenSource _destroyCts;

        private void Awake()
        {
            _destroyCts = new CancellationTokenSource();
            transform.DestroyAllChildren();
        }

        private void OnDestroy()
        {
            _destroyCts.CancelAndDispose();
        }

        public async UniTask Initialize(AreaData initAreaData)
        {
            _curLoadedArea = await Addressables.LoadAssetAsync<GameObject>(initAreaData.gashaponAreaRef);
            _curArea = Instantiate(_curLoadedArea, transform).GetComponent<GashaponArea>();
            _curArea.SetMaterial(curMat);

            curMat.color = new Color(1, 1, 1, 1);
            nextMat.color = new Color(1, 1, 1, 0);
        }


        public async UniTask CreateNextAreaAsync(AreaData newAreaData)
        {
            if (_curLoadedArea)
            {
                Addressables.Release(_curLoadedArea);
                _curLoadedArea = null;
            }

            _curLoadedArea = await Addressables.LoadAssetAsync<GameObject>(newAreaData.gashaponAreaRef);
            _nextArea = Instantiate(_curLoadedArea, transform).GetComponent<GashaponArea>();
            _nextArea.SetMaterial(nextMat);
        }

        public async UniTask ChangeAreaAsync()
        {
            var timer = 0f;
            const float duration = 0.5f;
            while (timer < duration && _destroyCts.IsCancellationRequested is false)
            {
                var ease = Easing.OutSine(timer, duration);

                curMat.color = new Color(1, 1, 1, 1 - ease);
                _curArea.SetLightIntensity(1 - ease);

                nextMat.color = new Color(1, 1, 1, ease);
                _nextArea.SetLightIntensity(ease);

                timer += Time.deltaTime;
                await UniTask.Yield();
            }

            (curMat, nextMat) = (nextMat, curMat);
            curMat.color = Color.white;
            nextMat.color = new Color(1f, 1f, 1f, 0f);

            _curArea.Destroy();
            _curArea = _nextArea;
            _nextArea = null;
            
            _curArea.SetLightIntensity(1f);
        }
    }
}