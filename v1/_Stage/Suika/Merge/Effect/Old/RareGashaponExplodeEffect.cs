using SOSG.Monster;
using SOSG.Stage.Suika;
using SOSG.Stage.Suika.Merge;
using UnityEngine;
using UnityEngine.Serialization;

public class RareGashaponExplodeEffect : GashaponExplodeEffectBase
{
    [FormerlySerializedAs("dataSO")]
    [SerializeField] private SuikaMergeEffectDataSO_Rare dataSOSO;

    [SerializeField] private ParticleSystem basePS;
    [SerializeField] private ParticleSystemRenderer basePSRenderer;
    [SerializeField] private ParticleSystem starPS;
    [SerializeField] private ParticleSystemRenderer starPSRenderer;
    [SerializeField] private ParticleSystem flicksPS;
    [SerializeField] private ParticleSystemRenderer flicksPSRenderer;
    [SerializeField] private ParticleSystem glowPS;

    // Base
    private const float BaseMinStartSpeed = 5f;
    private const float BaseMaxStartSpeed = 7.5f;

    private const int BaseBurstCount = 13;

    // Star
    private const float StarRadius = 0.5f;
    private const int StarBurstCount = 6;

    // Flicks
    private const float FlicksRadius = 0.5f;
    private const int FlicksBurstCount = 13;

    // Glow
    private const float GlowSize = 3f;


    public override void SetSize(float size)
    {
        var baseMain = basePS.main;
        baseMain.startSpeed = new ParticleSystem.MinMaxCurve(BaseMinStartSpeed * size, BaseMaxStartSpeed * size);
        var baseEmission = basePS.emission;
        baseEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(size * BaseBurstCount)));

        var starShape = starPS.shape;
        starShape.radius = StarRadius * size;
        var starEmission = starPS.emission;
        starEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(size * StarBurstCount)));

        var flicksShape = flicksPS.shape;
        flicksShape.radius = FlicksRadius * size;
        var flicksEmission = flicksPS.emission;
        flicksEmission.SetBurst(0, new ParticleSystem.Burst(0f, (short)(size * FlicksBurstCount)));

        var glowMain = glowPS.main;
        glowMain.startSize = GlowSize * size;
    }

    public override void SetColor(MonsterDataSO.MergeEffectColor color)
    {
        var curData = dataSOSO.ColorDataArr[(int)color];

        basePSRenderer.material = curData.material;
        starPSRenderer.material = curData.material;
        flicksPSRenderer.material = curData.material;

        var baseColorOverLifeTime = basePS.colorOverLifetime;
        baseColorOverLifeTime.color = curData.baseColorOverLifeTime;
        var starColorOverLifeTime = starPS.colorOverLifetime;
        starColorOverLifeTime.color = curData.starColorOverLifeTime;
        var flicksColorOverLifeTime = flicksPS.colorOverLifetime;
        flicksColorOverLifeTime.color = curData.flicksColorOverLifeTime;
        var glowStartColor = glowPS.main;
        glowStartColor.startColor = curData.glowStartColor;
    }
}