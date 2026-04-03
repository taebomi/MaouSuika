using TBM.MaouSuika.Core.Scene.LoadingScreen;
using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Core.Scene
{
    public class LoadingScreenController : MonoBehaviour
    {
        // todo + 로딩 연출 추가 시 전략 패턴으로 구현
        [SerializeField] private BasicLoadingScreen basicLoadingScreen;
        
        public void Show(object payload)
        {
            basicLoadingScreen.Show(payload);
        }

        public void UpdateProgress(float progress)
        {
            basicLoadingScreen.UpdateProgress(progress);
        }

        public void Hide()
        {
            basicLoadingScreen.Hide();
        }
    }
}