using UnityEngine;

namespace TBM.MaouSuika.Core.Scene
{
    public abstract class LoadingScreenBase : MonoBehaviour
    {
        public abstract void Show(object payload);
        public abstract void UpdateProgress(float progress);
        public abstract void Hide();
    }
}