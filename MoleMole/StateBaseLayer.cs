namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateBaseLayer : StateMachineBehaviour
    {
        private BaseMonoAnimatorEntity _entity;

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (this._entity == null)
            {
                this._entity = animator.GetComponent<BaseMonoAnimatorEntity>();
            }
            if (layerIndex == 0)
            {
                this._entity.AddFrameExitedAnimatorStates(stateInfo);
            }
        }
    }
}

