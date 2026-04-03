using UnityEngine;

namespace SOSG.Stage.Suika
{
    [CreateAssetMenu(fileName = "SuikaVarSO", menuName = "SOSG/Suika/Suika Var")]
    public class SuikaVarSO : ScriptableObject
    {
        public SuikaObject value;

#if UNITY_EDITOR
        [TextArea, SerializeField] private string memo;
#endif
    }
}