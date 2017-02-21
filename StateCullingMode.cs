using System;
using UnityEngine;

public class StateCullingMode : StateMachineBehaviour
{
    public AnimatorCullingMode cullingMode;
    private AnimatorCullingMode lastMode;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        this.lastMode = animator.cullingMode;
        animator.cullingMode = this.cullingMode;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.cullingMode = this.lastMode;
    }
}

