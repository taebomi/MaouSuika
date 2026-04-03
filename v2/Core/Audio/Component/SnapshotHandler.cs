using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Core.Audio
{
    public class SnapshotHandler : MonoBehaviour
    {
        private readonly Dictionary<GUID, EventInstance> _snapshots = new();

        private void OnDestroy()
        {
            foreach (var (_, snapshot) in _snapshots)
            {
                if (!snapshot.isValid()) continue;

                snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                snapshot.release();
            }

            _snapshots.Clear();
        }

        public void SetActive(EventReference snapshotRef, bool value)
        {
            if (value) Activate(snapshotRef);
            else Deactivate(snapshotRef);
        }

        public void SetParameter(EventReference snapshotRef, string paramName, float value)
        {
            if (snapshotRef.IsNull) return;
            if (!_snapshots.TryGetValue(snapshotRef.Guid, out var snapshot)) return;


            snapshot.setParameterByName(paramName, value);
        }

        private void Activate(EventReference snapshotRef)
        {
            if (snapshotRef.IsNull)
            {
                Logger.Warning($"Event Reference is null.");
                return;
            }

            if (_snapshots.TryGetValue(snapshotRef.Guid, out var snapshot)) return;

            snapshot = RuntimeManager.CreateInstance(snapshotRef);
            _snapshots.Add(snapshotRef.Guid, snapshot);
            snapshot.start();
        }

        private void Deactivate(EventReference snapshotRef)
        {
            if (snapshotRef.IsNull)
            {
                Logger.Warning($"Event Reference is null.");
                return;
            }

            if (!_snapshots.Remove(snapshotRef.Guid, out var snapshot)) return;

            if (!snapshot.isValid()) return;

            snapshot.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            snapshot.release();
        }


#if UNITY_EDITOR

        [SerializeField] private EventReference debug_eventRef;

        [Button]
        private void Debug_SetActive(bool value)
        {
            SetActive(debug_eventRef, value);
        }

        [Button]
        private void Debug_SetParameter(string paramName, float value)
        {
            SetParameter(debug_eventRef, paramName, value);
        }
#endif
    }
}