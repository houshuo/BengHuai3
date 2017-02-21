namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;

    [TaskCategory("Move")]
    public class StopMove : BehaviorDesigner.Runtime.Tasks.Action
    {
        protected IAIController _aiController;
        protected IAIEntity _aiEntity;

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
            this._aiController = this._aiEntity.GetActiveAIController();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            this._aiController.TryStop();
            return TaskStatus.Success;
        }
    }
}

