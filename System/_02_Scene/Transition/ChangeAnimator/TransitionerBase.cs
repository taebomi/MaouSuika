using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.Scene
{
    public abstract class TransitionerBase : MonoBehaviour
    {
        public abstract bool WillHideConversation { get; }
        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
        public abstract UniTask ShowAsync();
        public abstract UniTask HideAsync();
    }
}