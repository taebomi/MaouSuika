using UnityEngine;

namespace SOSG.Monster
{
    public class MonsterAnimator : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public void SetUp(MonsterDataSO dataSO)
        {
            animator.runtimeAnimatorController = dataSO.animatorOverrideController;
        }

        public void Move()
        {
            animator.SetBool(AnimatorCache.IsMove, true);
        }
        
    }
}