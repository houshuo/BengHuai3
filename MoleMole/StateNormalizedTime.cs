namespace MoleMole
{
    using System;
    using UnityEngine;

    [SharedBetweenAnimators]
    public class StateNormalizedTime : StateMachineBehaviour
    {
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat(InLevelData.NORMALIZED_TIME_NAME_PARAM, 0f);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (!animator.IsInTransition(0) || (animator.GetCurrentAnimatorStateInfo(0).fullPathHash == stateInfo.fullPathHash))
            {
                animator.SetFloat(InLevelData.NORMALIZED_TIME_NAME_PARAM, stateInfo.normalizedTime);
            }
        }
    }
}

