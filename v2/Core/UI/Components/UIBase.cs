using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace TBM.MaouSuika.Core.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        #region Show/Hide

        public virtual UniTask ShowAsync()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        public virtual UniTask HideAsync()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }

        #endregion


        #region Navigation

        [Title("Navigation")]
        [SerializeField] private Selectable defaultNaviTarget;
        private Selectable _lastNaviTarget;

        public Selectable GetNaviTarget()
        {
            if (_lastNaviTarget) return _lastNaviTarget;

            return defaultNaviTarget;
        }

        public void SetNaviTarget(Selectable target)
        {
            _lastNaviTarget = target;
        }

        public void ResetNaviTarget()
        {
            _lastNaviTarget = defaultNaviTarget;
        }

        #endregion
    }
}