namespace MoleMole
{
    using System;
    using UnityEngine;

    [SharedBetweenAnimators]
    public class StateAvatarRunBS : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<BaseMonoAvatar>().RunBSStart();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<BaseMonoAvatar>().RunBSStop();
        }
    }
}

