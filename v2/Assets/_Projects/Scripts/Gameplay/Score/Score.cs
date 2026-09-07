using System;
using CodeStage.AntiCheat.ObscuredTypes;
using R3;

namespace MaouSuika.Gameplay
{
    public class Score : IDisposable
    {
        private ObscuredInt _current;

        private readonly Subject<int> _totalAfterAdded = new();

        public int Current => _current;
        public Observable<int> TotalAfterAdded => _totalAfterAdded;

        public void Add(int amount)
        {
            var current = _current + amount;

            _current = current;
            _totalAfterAdded.OnNext(current);
        }

        public void Dispose()
        {
            _totalAfterAdded?.Dispose();
        }
    }
}