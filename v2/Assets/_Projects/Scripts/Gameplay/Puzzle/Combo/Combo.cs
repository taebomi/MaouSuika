using System;
using R3;

namespace MaouSuika.Gameplay
{
    public class Combo : IDisposable
    {
        private readonly ReactiveProperty<int> _current = new(0);

        private bool _addedSinceCheckpoint;

        public ReadOnlyReactiveProperty<int> Current => _current;

        public int CurrentValue => _current.Value;

        public int Add()
        {
            _addedSinceCheckpoint = true;
            _current.Value++;
            return _current.Value;
        }

        public void CheckPoint()
        {
            if (_addedSinceCheckpoint)
            {
                _addedSinceCheckpoint = false;
                return;
            }

            _current.Value = 0;
        }

        public void Dispose()
        {
            _current?.Dispose();
        }
    }
}