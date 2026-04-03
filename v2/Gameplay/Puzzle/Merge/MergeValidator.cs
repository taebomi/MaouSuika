using System.Collections.Generic;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class MergeValidator
    {
        private readonly HashSet<SuikaObject> _mergingSet = new();

        public void Setup()
        {
            _mergingSet.Clear();
        }

        public bool CanMerge(SuikaObject suika1, SuikaObject suika2)
        {
            if (suika1.Tier != suika2.Tier) return false;
            if (_mergingSet.Contains(suika1) || _mergingSet.Contains(suika2)) return false;

            return true;
        }

        public void Lock(SuikaObject suika1, SuikaObject suika2)
        {
            _mergingSet.Add(suika1);
            _mergingSet.Add(suika2);
        }

        public void Unlock(SuikaObject suika1, SuikaObject suika2)
        {
            _mergingSet.Remove(suika1);
            _mergingSet.Remove(suika2);
        }
    }
}