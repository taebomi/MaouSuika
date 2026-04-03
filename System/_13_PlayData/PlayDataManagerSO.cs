using System;
using UnityEngine;

namespace SOSG.System.PlayData
{
    [CreateAssetMenu(fileName = "PlayDataManagerSO", menuName = "TaeBoMi/Play Data/Play Data Manager")]
    public class PlayDataManagerSO : ScriptableObject
    {
        public Action OnSaveRequested;

        public void RequestSave() => OnSaveRequested?.Invoke();
    }
}