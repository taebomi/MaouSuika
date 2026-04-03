using SOSG.Stage;
using UnityEngine;

public class GashaponShooterAim : MonoBehaviour
{
    [Header("Event SO")]
    [SerializeField] private GashaponShooterControlEventSO aimingEventSO;
    [SerializeField] private BoolEventSO aimingStateChangedEventSO;
    
    [Header("Variable SO - Getter")]
    [SerializeField] private GashaponVarSO loadedCapsuleVarSO;

    [SerializeField] private BoolVariableSO isShooterCooldownVarSO;
    [SerializeField] private BoolVariableSO isShooterCollidedVarSO;

    [Header("Component")] 
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Gradient aimGradient;

    private bool _fixedColorFlag;
    private float _alpha;

    private const float AimSpriteYSize = 2f;
    private const float AimSpriteMinXSize = 1.5f; 
    private const float AimSpriteMaxXSize = 5.25f; // 중앙부터 벽까지 길이

    private void Awake()
    {
        _fixedColorFlag = false;
        _alpha = 1f;
        SetActive(false);
        
        isShooterCooldownVarSO.OnValueChanged += OnShooterCooldownChanged;
        isShooterCollidedVarSO.OnValueChanged += OnShooterCollidedChanged;
        aimingStateChangedEventSO.OnEventRaised += OnAimingStateChanged;
        aimingEventSO.ActionOnControl += UpdateAim;
    }

    private void OnDestroy()
    {
        isShooterCooldownVarSO.OnValueChanged -= OnShooterCooldownChanged;
        isShooterCollidedVarSO.OnValueChanged -= OnShooterCollidedChanged;
        aimingStateChangedEventSO.OnEventRaised -= OnAimingStateChanged;
        aimingEventSO.ActionOnControl -= UpdateAim;
        
    }

    private void OnAimingStateChanged(bool value)
    {
        SetActive(value);
    }

    private void OnShooterCooldownChanged(bool value)
    {
        _alpha = value ? GashaponShooter.CooldownAlpha : 1f;
        var color = sr.color;
        color.a = _alpha;
        sr.color = color;
    }
    
    private void OnShooterCollidedChanged(bool value)
    {
        _fixedColorFlag = value;
        var color = _fixedColorFlag ? GashaponShooter.CollidedColor : sr.color;
        color.a = _alpha;
        sr.color = color;
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void UpdateAim(Vector2 dir, float ratio)
    {
        transform.right = dir;

        var color = _fixedColorFlag ? GashaponShooter.CollidedColor : aimGradient.Evaluate(ratio);
        color.a = _alpha;
        sr.color = color;

        var curCapsuleSize = loadedCapsuleVarSO.value.CurSize;
        var length = curCapsuleSize + ratio * (AimSpriteMaxXSize - curCapsuleSize);
        if (length < AimSpriteMinXSize)
        {
            length = AimSpriteMinXSize;
        }
        sr.size = new Vector2( length, AimSpriteYSize);
    }
}