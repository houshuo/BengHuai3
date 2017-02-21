namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateAvatarDefaultEmpty : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<BaseMonoAvatar>().transform.localScale = Vector3.zero;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<BaseMonoAvatar>().transform.localScale = Vector3.one;
        }
    }
}

