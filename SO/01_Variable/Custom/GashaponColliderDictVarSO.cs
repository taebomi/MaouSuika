using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Var/Gashapon Collider Dict", fileName = "GashaponColliderDictVarSO",
        order = 1250)]
    public class GashaponColliderDictVarSO : ScriptableObject
    {
        public readonly Dictionary<int, Gashapon> Dict = new();

        [TextArea, SerializeField] private string memo;

        public bool TryGetValue(int key, out Gashapon value) => Dict.TryGetValue(key, out value);

        public Gashapon this[int key]
        {
            get => Dict[key];
            set => Dict[key] = value;
        }
    }
}