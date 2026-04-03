using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace SOSG.System.Scene
{
    /// <summary>
    /// 씬 시작 시 Awake에서만 호출할 것
    /// </summary>
    public static class SceneSetUpHelper
    {
        private static SceneSetUpTasker tasker;
        private static SceneSetUpTasker Tasker
        {
            get
            {
                if (ReferenceEquals(tasker, null))
                {
                    tasker = GameManager.Instance.SceneSetUpTasker;
                }

                return tasker;
            }
        }

        public static event Action Completed
        {
            add
            {
                if (Tasker.IsCompleted)
                {
                    value?.Invoke();
                    return;
                }

                Tasker.Completed += value;
            }
            remove => Tasker.Completed -= value;
        }

        public static bool IsCompleted => Tasker.IsCompleted;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize()
        {
            tasker = null;
        }

        public static void AddTask(Action setUpTask)
        {
            Tasker.AddTask(setUpTask);
        }

        public static void AddTask(Func<UniTask> setUpAsyncTask)
        {
            Tasker.AddTask(setUpAsyncTask);
        }
    }
}