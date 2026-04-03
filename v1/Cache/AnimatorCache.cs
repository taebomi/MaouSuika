using UnityEngine;

public static class AnimatorCache
{
    public static readonly int IsRight = Animator.StringToHash(nameof(IsRight));
    public static readonly int IsMove = Animator.StringToHash(nameof(IsMove));
    public static readonly int Loop = Animator.StringToHash(nameof(Loop));

    // Trigger
    public static readonly int EventTrigger = Animator.StringToHash(nameof(EventTrigger));
    
    public static readonly int JumpTrigger = Animator.StringToHash(nameof(JumpTrigger));
    public static readonly int RotateJumpTrigger = Animator.StringToHash(nameof(RotateJumpTrigger));
    public static readonly int SummonTrigger = Animator.StringToHash(nameof(SummonTrigger));

    public static readonly int RestartTrigger = Animator.StringToHash(nameof(RestartTrigger));
    public static readonly int DisappearTrigger = Animator.StringToHash(nameof(DisappearTrigger));
    public static readonly int EmphasizeTrigger = Animator.StringToHash(nameof(EmphasizeTrigger));
    
    
    
    // State
    public static readonly int Jump = Animator.StringToHash(nameof(Jump));
    public static readonly int RotateJump = Animator.StringToHash(nameof(RotateJump));
    public static readonly int Die = Animator.StringToHash("Die");
    
    
}