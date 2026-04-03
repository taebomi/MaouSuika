using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TaeBoMi;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SOSG.System.Scene
{
    public class SceneSetUpTasker : MonoBehaviour
    {
        private readonly List<Action> _taskList = new();
        private readonly List<Func<UniTask>> _asyncTaskList = new();

        public event Action Completed;
        public bool IsCompleted { get; private set; }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnDestroy()
        {
            ResetSystem();
        }

        public void AddTask(Action task)
        {
            if (IsCompleted)
            {
                Debug.LogWarning($"# SceneSetUpTasker: Task is already completed. Task will be ignored.");
                return;
            }
            _taskList.Add(task);
        }

        
        public void AddTask(Func<UniTask> asyncTask)
        {
            if (IsCompleted)
            {
                Debug.LogWarning($"# SceneSetUpTasker: Task is already completed. Task will be ignored.");
                return;
            }
            _asyncTaskList.Add(asyncTask);
        }

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode) =>
            RunSetUpAsync().Forget();

        private void OnSceneUnloaded(UnityEngine.SceneManagement.Scene scene) => ResetSystem();

        private void ResetSystem()
        {
            _taskList.Clear();
            _asyncTaskList.Clear();
            IsCompleted = false;
            Completed = null;
        }

        private async UniTask RunSetUpAsync()
        {
            TBMUtility.Log($"# Wait For Game Set Up");
            await GameManager.WaitForSetUpCompleted();

            foreach (var task in _taskList)
            {
                task.Invoke();
            }

            foreach (var asyncTask in _asyncTaskList)
            {
                await asyncTask.Invoke();
            }

            IsCompleted = true;
            TBMUtility.Log($"# Scene Set Up Finished");
            Completed?.Invoke();
        }
    }
}