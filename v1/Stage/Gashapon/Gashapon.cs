using System;
using FMODUnity;
using SOSG.Monster;
using SOSG.Stage;
using SOSG.System.Audio;
using SOSG.System.Vibration;
using TaeBoMi.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gashapon : MonoBehaviour
{
    [Header("Event SO - Sender")] [SerializeField]
    private GashaponMergeEventSO mergeEventSO;

    [Header("Variable SO - Setter")] [SerializeField]
    private GashaponColliderDictVarSO capsuleColliderDictVarSO;

    [SerializeField] private GashaponLinkedListVarSO activeGashaponListVarSO;

    [Header("Variable SO - Getter")] [SerializeField]
    private GashaponVarSO lastShotGashaponVarSO;

    [Header("Data SO")] [SerializeField] private SpriteDataSO levelSpriteDataSO;


    [Header("Sfx")] [SerializeField] private EventReference collidedSfx;

    [Header("Components")]
    [Header("Sprite Renderer")]
    [SerializeField] private SpriteRenderer bgBodySr;

    [SerializeField] private SpriteRenderer fgGlassSr;
    [SerializeField] private SpriteRenderer fgSr;
    [SerializeField] private SpriteRenderer lightSr;
    [SerializeField] private SpriteRenderer outlineSr;
    [SerializeField] private SpriteRenderer monsterBodySr;

    [Header("Transform")]
    [SerializeField] private Transform capsuleTr;
    [SerializeField] private Transform lightTr;

    [SerializeField] private Transform monsterTr;

    // [SerializeField] private Transform maskTr;
    [SerializeField] private Transform monsterBodyTr;

    [field: Header("Etc")]
    [field: SerializeField]
    public Rigidbody2D Rb { get; private set; }

    [field: SerializeField] public Collider2D Coll { get; private set; }
    [SerializeField] private Animator monsterAni;

    public bool IsGrounded { get; private set; }
    public bool IsMerging { get; private set; }
    public int CurLevel { get; private set; }
    public float CurSize { get; private set; }

    public MonsterDataSO MonsterDataSO { get; private set; }
    private Quaternion _shotRotation;


    private IObjectPool<Gashapon> _pool;

    public enum State
    {
        None,
        Loaded,
        Shot,
        Merging,
        GameOver,
    }

    private void Awake()
    {
        IsMerging = false;
    }

    private void OnEnable()
    {
        capsuleColliderDictVarSO.Dict.Add(gameObject.GetInstanceID(), this);
        capsuleColliderDictVarSO.Dict.Add(Coll.GetInstanceID(), this);
        activeGashaponListVarSO.Add(this);
    }

    private void OnDisable()
    {
        capsuleColliderDictVarSO.Dict.Remove(gameObject.GetInstanceID());
        capsuleColliderDictVarSO.Dict.Remove(Coll.GetInstanceID());
        activeGashaponListVarSO.Remove(this);
    }

    private void Update()
    {
        monsterTr.rotation = _shotRotation;
        lightTr.rotation = Quaternion.identity;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (capsuleColliderDictVarSO.Dict.TryGetValue(other.gameObject.GetInstanceID(), out var collidedCapsule))
        {
            if (collidedCapsule.IsGrounded)
            {
                IsGrounded = true;
            }

            OnCapsuleCollided(collidedCapsule);
        }

        if (lastShotGashaponVarSO.value == this)
        {
            AudioSystemHelper.PlayBgm(collidedSfx);
            VibrationEventBus.PlayConstant(0.05f, 0.1f, 0.075f);
        }

        if (other.gameObject.CompareTag("Ground"))
        {
            IsGrounded = true;
        }
    }

    public void Initialize(IObjectPool<Gashapon> pool)
    {
        _pool = pool;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public void Set(MonsterDataSO monsterDataSO, float size, int level)
    {
        MonsterDataSO = monsterDataSO;
        
        IsGrounded = false;
        IsMerging = false;
        CurLevel = level;
        CurSize = size;

        Rb.bodyType = RigidbodyType2D.Dynamic;

        capsuleTr.localScale = new Vector3(size, size, 1f);
        capsuleTr.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        var monsterSize = monsterDataSO.ySize;
        var calibratedMonsterSize = 1f;
        if (size < monsterSize)
        {
            calibratedMonsterSize = size / monsterSize;
            monsterBodyTr.transform.localScale = new Vector3(calibratedMonsterSize, calibratedMonsterSize, 1f);
        }
        else
        {
            monsterBodyTr.transform.localScale = new Vector3(1f, 1f, 1f);
        }


        monsterAni.runtimeAnimatorController = monsterDataSO.animatorOverrideController;
        monsterBodyTr.localPosition = monsterDataSO.bodyCenterPos * calibratedMonsterSize;

        _shotRotation = Quaternion.Euler(0f, 0f,
            TaeBoMiCache.RightRotation[Random.Range(0, TaeBoMiCache.RightRotation.Length)]);
        
        UpdateColor(MonsterDataSO.capsuleColor, false);
    }

    public void SetLoadedState()
    {
        ActivatePhysics(false);
        bgBodySr.sortingLayerID = fgGlassSr.sortingLayerID =
            fgSr.sortingLayerID = lightSr.sortingLayerID = outlineSr.sortingLayerID =
                monsterBodySr.sortingLayerID = TaeBoMiCache.ForegroundSortingLayerID;
    }

    public void ActivatePhysics(bool value)
    {
        Rb.simulated = value;
    }

    public void Shoot(Vector2 vel)
    {
        ActivatePhysics(true);
        Rb.linearVelocity = vel;
        bgBodySr.sortingLayerID = fgGlassSr.sortingLayerID =
            fgSr.sortingLayerID = lightSr.sortingLayerID = outlineSr.sortingLayerID =
                monsterBodySr.sortingLayerID = TaeBoMiCache.GashaponSortingLayerID;
    }

    public void SetShooterState(GashaponShooter.State state)
    {
        switch (state)
        {
            case GashaponShooter.State.None:
            case GashaponShooter.State.Shootable:
                var color = MonsterDataSO.capsuleColor;
                color.a = 1f;
                UpdateColor(color, false);
                break;
            case GashaponShooter.State.Cooldown:
                color = MonsterDataSO.capsuleColor;
                color.a = GashaponShooter.CooldownAlpha;
                UpdateColor(color, false);
                break;
            case GashaponShooter.State.Collided:
                color = GashaponShooter.CollidedColor;
                color.a = 1f;
                UpdateColor(color, true);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void UpdateColor(Color color, bool applyAll)
    {
        outlineSr.color = bgBodySr.color = fgSr.color = color;
        if (applyAll)
        {
            fgGlassSr.color = lightSr.color = monsterBodySr.color = color;
        }
        else
        {
            fgGlassSr.color = lightSr.color = monsterBodySr.color = new Color(1f, 1f, 1f, color.a);
        }
    }

    public void SetMergeState()
    {
        IsMerging = true;

        Rb.bodyType = RigidbodyType2D.Kinematic;
        Rb.linearVelocity = Vector2.zero;
    }

    public void Deactivate()
    {
        if (_pool is not null)
        {
            gameObject.SetActive(false);
            _pool.Release(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private bool IsSameLevel(Gashapon other) => CurLevel == other.CurLevel;

    private void OnCapsuleCollided(Gashapon other)
    {
        if (IsSameLevel(other) is false)
        {
            return;
        }

        if (IsMerging || other.IsMerging)
        {
            return;
        }

        mergeEventSO.RaiseEvent(this, other);
    }
}