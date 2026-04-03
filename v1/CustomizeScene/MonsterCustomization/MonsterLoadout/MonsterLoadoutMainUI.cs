using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Monster;
using TaeBoMi;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace SOSG.Customization.Monster
{
    public class MonsterLoadoutMainUI : MonoBehaviour
    {
        [FormerlySerializedAs("manager")] [SerializeField] private MonsterCustomizationSceneManager sceneManager;
        [SerializeField] private MonsterLoadout monsterLoadout;

        [SerializeField] private RectTransform[] gradeContainer;
        [SerializeField] private Image[] gradeContainerImageArr;

        [SerializeField] private Sprite[] gradeContainerSpriteArr;

        private int _curIdx;
        private bool _isBlinking;

        private CancellationTokenSource _blinkingCts;

        private void Awake()
        {
            _isBlinking = false;
            _blinkingCts = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _blinkingCts?.CancelAndDispose();
        }

        public void SetGrade(MonsterGrade grade)
        {
            _curIdx = TBMUtility.GetEnumIndex(grade);
            
            for (var i = 0; i < gradeContainer.Length; i++)
            {
                gradeContainerImageArr[i].sprite = i == _curIdx
                    ? gradeContainerSpriteArr[i * 2 + 1]
                    : gradeContainerSpriteArr[i * 2];
            }
        }

        public void StartBlink()
        {
            if (_isBlinking)
            {
                return;
            }

            _isBlinking = true;
            BlinkRepeatAsync(_blinkingCts.Token).Forget();
        }

        public void StopBlink()
        {
            if (_isBlinking is false)
            {
                return;
            }

            _isBlinking = false;
            _blinkingCts.CancelAndDispose();
            _blinkingCts = new CancellationTokenSource();
        }

        private async UniTaskVoid BlinkRepeatAsync(CancellationToken ct)
        {
            var timer = 0;
            while (_isBlinking && ct.IsCancellationRequested is false)
            {
                timer++;
                gradeContainerImageArr[_curIdx].sprite = timer % 2 == 0
                    ? gradeContainerSpriteArr[_curIdx * 2 + 1]
                    : gradeContainerSpriteArr[_curIdx * 2];
                await UniTask.Delay(500, cancellationToken: ct);
            }
        }
    }
}