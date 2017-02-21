namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateSetBooleanTrue : StateMachineBehaviour
    {
        public bool resetOnExit = true;
        [Header("Target's value will be set to true on enter.")]
        public string target;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetBool(this.target, true);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.resetOnExit)
            {
                animator.SetBool(this.target, false);
            }
        }
    }
}

