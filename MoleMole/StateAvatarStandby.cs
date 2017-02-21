namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateAvatarStandby : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
            animator.SetFloat("IdleCD", component.config.StateMachinePattern.IdleCD);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat("IdleCD", 0f);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
            float num = animator.GetFloat("IdleCD") - (Time.deltaTime * component.TimeScale);
            animator.SetFloat("IdleCD", num);
        }
    }
}

