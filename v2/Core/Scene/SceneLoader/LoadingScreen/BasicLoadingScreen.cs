using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Core.Scene.LoadingScreen
{
    public class BasicLoadingScreen : LoadingScreenBase
    {
        [SerializeField] private Image loadingBar;
        
        public override void Show(object payload)
        {
            gameObject.SetActive(true);
        }

        public override void UpdateProgress(float progress)
        {
            loadingBar.transform.localScale = new Vector3(1f - progress, 1f, 1f);
        }

        public override void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}