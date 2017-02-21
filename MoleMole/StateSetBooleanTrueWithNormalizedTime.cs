namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateSetBooleanTrueWithNormalizedTime : StateMachineBehaviour
    {
        [Header("Target's value will be set to true Between normalizedTimeBegin and normalizedTimeEnd, else set false")]
        public float normalizedTimeBegin;
        public float normalizedTimeEnd = 1f;
        public bool resetOnExit = true;
        public string target;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.resetOnExit)
            {
                animator.SetBool(this.target, false);
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if ((stateInfo.normalizedTime >= this.normalizedTimeBegin) && (stateInfo.normalizedTime <= this.normalizedTimeEnd))
            {
                animator.SetBool(this.target, true);
            }
            else
            {
                animator.SetBool(this.target, false);
            }
        }
    }
}

