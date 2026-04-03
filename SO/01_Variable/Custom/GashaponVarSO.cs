using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(menuName = "TaeBoMi/Custom Var/Gashapon", fileName = "GashaponVarSO", order = 1100)]
    public class GashaponVarSO : ScriptableObject
    {
        public Gashapon value;

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
#endif
    }
}