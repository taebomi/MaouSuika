using Lofelt.NiceVibrations;
using SOSG.Monster;
using SOSG.System.Vibration;
using UnityEngine;

public class GashaponExplodeEffectGenerator : MonoBehaviour
{
    [Header("Event SO - Listener")] [SerializeField]
    private GashaponExplodeEffectRequestEventSO explodeEffectRequestEventSO;

    [Header("Prefab")] [SerializeField] private GashaponExplodeEffectBase commonEffectPrefab;
    [SerializeField] private GashaponExplodeEffectBase uncommonEffectPrefab;
    [SerializeField] private GashaponExplodeEffectBase rareEffectPrefab;
    [SerializeField] private GashaponExplodeEffectBase epicEffectPrefab;

    private void OnEnable()
    {
        explodeEffectRequestEventSO.OnRequestGashaponExplodeEffect += GetEffect;
    }

    private void OnDisable()
    {
        explodeEffectRequestEventSO.OnRequestGashaponExplodeEffect -= GetEffect;
    }

    private GashaponExplodeEffectBase GetEffect(int level)
    {
        GashaponExplodeEffectBase effectPrefab;
        HapticPatterns.PresetType presetType;
        switch (MonsterController.GetGrade(level))
        {
            case MonsterGrade.Common:
                presetType = HapticPatterns.PresetType.SoftImpact;
                effectPrefab = commonEffectPrefab;
                break;
            case MonsterGrade.Uncommon:
                presetType = HapticPatterns.PresetType.MediumImpact;
                effectPrefab = uncommonEffectPrefab;
                break;
            case MonsterGrade.Rare:
                presetType = HapticPatterns.PresetType.HeavyImpact;
                effectPrefab = rareEffectPrefab;
                break;
            default:
                presetType = HapticPatterns.PresetType.None;
                effectPrefab = epicEffectPrefab;
                break;
        }

        if (presetType is not HapticPatterns.PresetType.None)
        {
            VibrationEventBus.PlayPreset(presetType);
        }
        else
        {
            VibrationEventBus.PlayConstant(0.75f, 0.75f, 0.75f);
        }

        var effect = Instantiate(effectPrefab, transform);
        return effect;
    }
}