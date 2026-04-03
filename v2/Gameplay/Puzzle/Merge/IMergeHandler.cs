using TBM.MaouSuika.Gameplay.Puzzle;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public interface IMergeHandler
    {
        void RequestMerge(SuikaObject suika1, SuikaObject suika2);
    }
}