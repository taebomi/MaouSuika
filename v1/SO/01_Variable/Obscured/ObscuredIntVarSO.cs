using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace SOSG.Stage
{
    [CreateAssetMenu(fileName = "ScoreVarSO", menuName = "TaeBoMi/Stage/Score Var")]
    public class ObscuredIntVarSO : ScriptableObject
    {
        [field: SerializeField] public ObscuredInt Value { get; private set; }
        public event Action<int> OnValueChanged;

#if SOSG_DEBUG
        [SerializeField, TextArea] private string memo;
        [SerializeField] private bool isDebugMode;
#endif

        public void Initialize(int value)
        {
            Value = value;
        }

        public static ObscuredIntVarSO operator +(ObscuredIntVarSO a, int b)
        {
            var result = a.Value + b;
            a.Value = result;
            a.OnValueChanged?.Invoke(result);
            return a;
        }

        public static implicit operator int(ObscuredIntVarSO value)
        {
            return value.Value;
        }
    }
}