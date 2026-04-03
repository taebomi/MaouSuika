using SOSG.Monster;
using SOSG.Stage.Suika;
using SOSG.Stage.Suika.Merge;
using UnityEngine;

public class UncommonGashaponExplodeEffect : GashaponExplodeEffectBase
{
    [SerializeField] private SuikaMergeEffectDataSO_Uncommon dataSO;

    [SerializeField] private ParticleSystem basePS;
    [SerializeField] private ParticleSystemRenderer basePSRenderer;
    [SerializeField] private ParticleSystem centerPS;
    [SerializeField] private ParticleSystemRenderer centerPSRenderer;
    [SerializeField] private ParticleSystem glowPS;
    [SerializeField] private ParticleSystem dustPS;
    [SerializeField] private ParticleSystemRenderer dustPSRenderer;

    private const float BaseMinStartSize = 1f;
    private const float BaseMaxStartSize = 1.25f;
    private const int BaseBurstCount = 10;

    private const float CenterStartSize = 1.25f;
    
    private const float GlowStartSize = 3f;

    private const int DustBurstCount = 15;

    public override void SetSize(float size)
    {
        var baseMain = basePS.main;
        baseMain.startSize = new ParticleSystem.MinMaxCurve(size * BaseMinStartSize, size * BaseMaxStartSize);
        var baseEmission = basePS.emission;
        var sqrtSize = Mathf.Sqrt(size);
        baseEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(sqrtSize * BaseBurstCount)));

        var centerMain = centerPS.main;
        centerMain.startSize = size * CenterStartSize;

        var glowMain = glowPS.main;
        glowMain.startSize = size * GlowStartSize;

        var dustEmission = dustPS.emission;
        dustEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(sqrtSize * DustBurstCount)));
    }

    public override void SetColor(MonsterDataSO.MergeEffectColor color)
    {
        var curData = dataSO.ColorDataArr[(int)color];
        basePSRenderer.material = curData.baseMaterial;

        centerPSRenderer.material = curData.centerMaterial;

        var glowMain = glowPS.main;
        glowMain.startColor = curData.glowStartColor;

        dustPSRenderer.material = curData.dustMaterial;
        var dustColorOverLifeTime = dustPS.colorOverLifetime;
        dustColorOverLifeTime.color = curData.dustColorOverLifeTime;
    }
}