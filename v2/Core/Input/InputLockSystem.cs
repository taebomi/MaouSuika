using System.Collections.Generic;

namespace TBM.MaouSuika.Core.Input
{
    public class InputLockSystem
    {
        private readonly Dictionary<string, InputChannel> _locks = new();
        private InputChannel _lockedChannel = InputChannel.None;

        public bool IsAllowed(InputChannel channel)
        {
            return (_lockedChannel & channel) == 0;
        }

        public void Lock(string reason, InputChannel channelToLock)
        {
            if (_locks.TryGetValue(reason, out var lockedChannel) && lockedChannel == channelToLock) return;

            _locks[reason] = channelToLock;
            RecalculateLockedChannel();
        }

        public void LockAll(string reason) => Lock(reason, InputChannel.All);

        public void Unlock(string reason)
        {
            if (!_locks.Remove(reason)) return;

            RecalculateLockedChannel();
        }

        public void Clear()
        {
            _locks.Clear();
            _lockedChannel = InputChannel.None;
        }

        private void RecalculateLockedChannel()
        {
            _lockedChannel = InputChannel.None;
            foreach (var lockedChannel in _locks.Values)
            {
                _lockedChannel |= lockedChannel;
            }
        }
    }
}