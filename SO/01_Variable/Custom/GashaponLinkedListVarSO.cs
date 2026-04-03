using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Var/Gashapon/Gashapon LinkedList", fileName = "GashaponLinkedListVarSO", order = 1100)]
    public class GashaponLinkedListVarSO : ScriptableObject
    {
        public readonly LinkedList<Gashapon> Value = new ();

        [TextArea]
        [SerializeField] private string memo;

        public void Add(Gashapon gashapon)
        {
            Value.AddLast(gashapon);
        }

        public void Remove(Gashapon gashapon)
        {
            Value.Remove(gashapon);
        }
    }
}