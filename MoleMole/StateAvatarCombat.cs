namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateAvatarCombat : StateMachineBehaviour
    {
        public float CD = 2f;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat("CombatToStandByCD", this.CD);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.SetFloat("CombatToStandByCD", 0f);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
            BaseMonoAvatar component = animator.GetComponent<BaseMonoAvatar>();
            float @float = animator.GetFloat("CombatToStandByCD");
            animator.SetFloat("CombatToStandByCD", @float - (Time.deltaTime * component.TimeScale));
        }
    }
}

