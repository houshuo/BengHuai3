namespace MoleMole
{
    using System;
    using UnityEngine;

    public class StateRebindAttachPoint : StateMachineBehaviour
    {
        [Header("Rebind AttachPoint to another attach point")]
        public string AttachPoint;
        [Header("Other Attach Point")]
        public string OtherAttachPoint;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            animator.GetComponent<BaseMonoAnimatorEntity>().RebindAttachPoint(this.AttachPoint, this.OtherAttachPoint);
        }
    }
}

