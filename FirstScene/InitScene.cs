using System;
using Cysharp.Threading.Tasks;
using SOSG.System;
using SOSG.System.Scene;
using UnityEngine;

namespace SOSG.Scene.Init
{
    public class InitScene : MonoBehaviour
    {
        private void Awake()
        {
            SceneSetUpHelper.Completed += OnSetUpFinished;
        }

        private void OnSetUpFinished()
        {
            SceneLoadHelper.LoadScene(SceneName.Main);
        }
    }
}