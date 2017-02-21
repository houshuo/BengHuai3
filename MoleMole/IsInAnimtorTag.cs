namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    public class IsInAnimtorTag : Conditional
    {
        private BaseMonoAnimatorEntity _entity;
        public List<AvatarData.AvatarTagGroup> animatorTagNamesAvatar;
        public List<MonsterData.MonsterTagGroup> animatorTagNamesMonster;

        public override void OnAwake()
        {
            this._entity = base.GetComponent<BaseMonoAnimatorEntity>();
        }

        public override TaskStatus OnUpdate()
        {
            if (this._entity is BaseMonoAvatar)
            {
                BaseMonoAvatar avatar = this._entity as BaseMonoAvatar;
                for (int i = 0; i < this.animatorTagNamesAvatar.Count; i++)
                {
                    if (avatar.IsAnimatorInTag(this.animatorTagNamesAvatar[i]))
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            else if (this._entity is BaseMonoMonster)
            {
                BaseMonoMonster monster = this._entity as BaseMonoMonster;
                for (int j = 0; j < this.animatorTagNamesMonster.Count; j++)
                {
                    if (monster.IsAnimatorInTag(this.animatorTagNamesMonster[j]))
                    {
                        return TaskStatus.Success;
                    }
                }
            }
            return TaskStatus.Failure;
        }
    }
}

