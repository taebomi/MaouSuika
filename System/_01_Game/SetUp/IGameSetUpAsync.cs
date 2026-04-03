using Cysharp.Threading.Tasks;

namespace SOSG.System
{
    public interface IGameSetUpAsync
    {
        UniTask SetUpAsync();
    }
}