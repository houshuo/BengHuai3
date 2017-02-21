namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class GetDistance : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public SharedFloat Distance;

        public override void OnAwake()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoAvatar)
            {
                this._aiEntity = (BaseMonoAvatar) component;
            }
            else if (component is BaseMonoMonster)
            {
                this._aiEntity = (BaseMonoMonster) component;
            }
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if ((this._aiEntity.AttackTarget != null) && this._aiEntity.AttackTarget.IsActive())
            {
                Vector3 xZPosition = this._aiEntity.XZPosition;
                Vector3 b = this._aiEntity.AttackTarget.XZPosition;
                this.Distance.SetValue(Vector3.Distance(xZPosition, b));
            }
            return TaskStatus.Success;
        }
    }
}

