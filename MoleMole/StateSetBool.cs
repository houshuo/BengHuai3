namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateSetBool : StateMachineBehaviour
    {
        public bool enterValue;
        public bool exitValue;
        public bool setOnEnter;
        public bool setOnExit;
        [Header("Target Bool")]
        public string target;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.setOnEnter)
            {
                animator.SetBool(Animator.StringToHash(this.target), this.enterValue);
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this.setOnExit)
            {
                animator.SetBool(Animator.StringToHash(this.target), this.exitValue);
            }
        }
    }
}

