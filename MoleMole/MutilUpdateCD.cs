namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MutilUpdateCD : BehaviorDesigner.Runtime.Tasks.Action
    {
        private IAIEntity _aiEntity;
        public List<CDInfo> cdList;

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
            foreach (CDInfo info in this.cdList)
            {
                info.InitOnAwake();
            }
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            foreach (CDInfo info in this.cdList)
            {
                if ((info.keepUpdating || (info.CD.Value >= 0f)) && (info.NoUpdate.Value >= 0f))
                {
                    info.CD.Value -= Time.deltaTime * this._aiEntity.TimeScale;
                }
            }
            return TaskStatus.Running;
        }

        public class CDInfo
        {
            public SharedFloat CD;
            public float defaultTime;
            public bool isRandom;
            public bool keepUpdating;
            public float maxRandTime;
            public float minRandTime;
            [BehaviorDesigner.Runtime.Tasks.Tooltip("if NoUpdate < 0, CD will never update (for unique monster)")]
            public SharedFloat NoUpdate;

            public void InitOnAwake()
            {
                if (this.isRandom)
                {
                    this.CD.Value = UnityEngine.Random.Range(this.minRandTime, this.maxRandTime);
                }
                else
                {
                    this.CD.Value = this.defaultTime;
                }
            }
        }
    }
}

