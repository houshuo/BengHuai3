namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class SetStateAnimatorPattern : BehaviorDesigner.Runtime.Tasks.Action
    {
        public string animatorEventPatternName;
        public string stateName;

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoMonster)
            {
                (component as BaseMonoMonster).SetSoleAnimatorEventPattern(Animator.StringToHash(this.stateName), this.animatorEventPatternName);
            }
            return TaskStatus.Success;
        }
    }
}

