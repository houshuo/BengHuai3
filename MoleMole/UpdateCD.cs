namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    public class UpdateCD : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public SharedFloat CD;
        public float defaultTime;
        public bool isRandom;
        public bool keepUpdating;
        public float maxRandTime;
        public float minRandTime;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("if NoUpdate < 0, CD will never update (for unique monster)")]
        public SharedFloat NoUpdate;

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
            if (this.isRandom)
            {
                this.CD.Value = UnityEngine.Random.Range(this.minRandTime, this.maxRandTime);
            }
            else
            {
                this.CD.Value = this.defaultTime;
            }
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if ((this.keepUpdating || (this.CD.Value >= 0f)) && (this.NoUpdate.Value >= 0f))
            {
                this.CD.Value -= Time.deltaTime * this._aiEntity.TimeScale;
            }
            return TaskStatus.Running;
        }
    }
}

