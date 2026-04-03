using SOSG.Monster;
using SOSG.Stage.Suika;
using SOSG.Stage.Suika.Merge;
using UnityEngine;

public class CommonGashaponExplodeEffect : GashaponExplodeEffectBase
{
    [SerializeField] private SuikaMergeEffectDataSO_Common dataSO;

    [SerializeField] private ParticleSystemRenderer basePSRenderer;
    [SerializeField] private ParticleSystem basePS;
    [SerializeField] private ParticleSystem glowPS;

    private const int BaseEmission = 40;
    private const float BaseRadius = 0.75f;
    private const float GlowSize = 5f;

    public override void SetSize(float size)
    {
        var baseEmission = basePS.emission;
        baseEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(size * BaseEmission)));
        var baseShape = basePS.shape;
        baseShape.radius = size * BaseRadius;
        var glowMain = glowPS.main;
        glowMain.startSize = size * GlowSize;
    }

    public override void SetColor(MonsterDataSO.MergeEffectColor color)
    {
        var colorData = dataSO.ColorDataArr[(int)color];

        basePSRenderer.material = colorData.baseMaterial;

        var colorOverLifeTime = basePS.colorOverLifetime;
        colorOverLifeTime.color = colorData.baseColorOverLifeTime;

        // todo light 설정

        var glowMain = glowPS.main;
        glowMain.startColor = colorData.glowStartColor;
    }
}