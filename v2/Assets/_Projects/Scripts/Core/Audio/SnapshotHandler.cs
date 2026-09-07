using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace MaouSuika.Core
{
    public class SnapshotHandler : IDisposable
    {
        private readonly Dictionary<GUID, EventInstance> _snapshots = new();

        public void Dispose()
        {
            foreach (var (_, snapshot) in _snapshots)
            {
                if (!snapshot.isValid()) continue;

                snapshot.stop(STOP_MODE.IMMEDIATE);
                snapshot.release();
            }

            _snapshots.Clear();
        }

        public void Start(EventReference snapshotRef)
        {
            if (snapshotRef.IsNull) return;

            if (_snapshots.TryGetValue(snapshotRef.Guid, out var snapshot)) return;

            snapshot = RuntimeManager.CreateInstance(snapshotRef);
            _snapshots.Add(snapshotRef.Guid, snapshot);
            snapshot.start();
        }

        public void Stop(EventReference snapshotRef)
        {
            if (snapshotRef.IsNull) return;

            if (!_snapshots.Remove(snapshotRef.Guid, out var snapshot)) return;
            if (!snapshot.isValid()) return;

            snapshot.stop(STOP_MODE.ALLOWFADEOUT);
            snapshot.release();
        }

        public void Set(EventReference snapshotRef, string paramName, float value)
        {
            if (snapshotRef.IsNull) return;
            if (!_snapshots.TryGetValue(snapshotRef.Guid, out var snapshot)) return;

            snapshot.setParameterByName(paramName, value);
        }
    }
}