namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskIcon("{SkinColor}WaitIcon.png"), TaskDescription("Wait a specified amount of time. The task will return running until the task is done waiting. It will return success after the wait time has elapsed.")]
    public class Wait : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        private float _timer;
        public float randomAddRange;
        public float waitTime = 1f;

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
            this._timer = this.waitTime + (this.randomAddRange * UnityEngine.Random.value);
        }

        public override TaskStatus OnUpdate()
        {
            if (this._timer < 0f)
            {
                return TaskStatus.Success;
            }
            this._timer -= Time.deltaTime * this._aiEntity.TimeScale;
            return TaskStatus.Running;
        }
    }
}

