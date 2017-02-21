namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateClearTrigger : StateMachineBehaviour
    {
        public bool clearOnEnter;
        public bool clearOnExit;
        [Header("Target trigger")]
        public string target;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.clearOnEnter)
            {
                animator.ResetTrigger(this.target);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.clearOnExit)
            {
                animator.ResetTrigger(this.target);
            }
        }
    }
}

