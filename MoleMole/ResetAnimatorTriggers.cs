namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ResetAnimatorTriggers : BehaviorDesigner.Runtime.Tasks.Action
    {
        private Animator _animator;
        public List<string> triggerNamesList;

        public override void OnAwake()
        {
            this._animator = base.GetComponent<Animator>();
        }

        public override TaskStatus OnUpdate()
        {
            for (int i = 0; i < this.triggerNamesList.Count; i++)
            {
                this._animator.ResetTrigger(this.triggerNamesList[i]);
            }
            return TaskStatus.Success;
        }
    }
}

