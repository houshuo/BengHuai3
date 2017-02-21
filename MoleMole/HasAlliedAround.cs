namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class HasAlliedAround : BehaviorDesigner.Runtime.Tasks.Action
    {
        public float range = 10f;

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            BaseMonoAnimatorEntity component = base.GetComponent<BaseMonoAnimatorEntity>();
            if (component is BaseMonoMonster)
            {
                BaseMonoMonster monster = component as BaseMonoMonster;
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                for (int i = 0; i < allMonsters.Count; i++)
                {
                    if (((allMonsters[i] != monster) && allMonsters[i].IsActive()) && (Vector3.Distance(allMonsters[i].XZPosition, monster.XZPosition) < this.range))
                    {
                        return TaskStatus.Success;
                    }
                }
                return TaskStatus.Failure;
            }
            if (component is BaseMonoAvatar)
            {
                BaseMonoAvatar avatar = component as BaseMonoAvatar;
                List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
                for (int j = 0; j < allAvatars.Count; j++)
                {
                    if (((allAvatars[j] != avatar) && allAvatars[j].IsActive()) && (Vector3.Distance(allAvatars[j].XZPosition, avatar.XZPosition) < this.range))
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            return TaskStatus.Failure;
        }
    }
}

