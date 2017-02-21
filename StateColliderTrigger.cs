using MoleMole;
using System;
using UnityEngine;

public class StateColliderTrigger : StateMachineBehaviour
{
    private Collider animCollider;
    private Collider hitCollider;
    public bool isActive;
    private bool lastActiveAnim;
    private bool lastActiveHit;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.animCollider = animator.GetComponent<Collider>();
        if (this.animCollider != null)
        {
            this.lastActiveAnim = this.animCollider.enabled;
            this.animCollider.enabled = this.isActive;
        }
        BaseMonoMonster component = animator.GetComponent<BaseMonoMonster>();
        if (component != null)
        {
            this.hitCollider = component.hitbox;
            if (this.hitCollider != null)
            {
                this.lastActiveHit = this.hitCollider.enabled;
                this.hitCollider.enabled = this.isActive;
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (this.animCollider != null)
        {
            this.animCollider.enabled = this.lastActiveAnim;
        }
        if (this.hitCollider != null)
        {
            this.hitCollider.enabled = this.lastActiveHit;
        }
    }
}

