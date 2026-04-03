using Cysharp.Threading.Tasks;
using UnityEngine;

public interface IPlayerGameOverHandler
{
    UniTask OnGameOverAsync();
}
