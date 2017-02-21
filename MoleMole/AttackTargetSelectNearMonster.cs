namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("AttackTarget/Avatar")]
    public class AttackTargetSelectNearMonster : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;
        public bool hasDistanceLimit;
        public SharedBool isNewTarget;
        public float maxDistance;
        public float minDistance;

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            if ((this._avatar.AttackTarget != null) && this._avatar.AttackTarget.IsActive())
            {
                this.isNewTarget.SetValue(false);
                return TaskStatus.Success;
            }
            BaseMonoEntity attackTarget = this._avatar.AttackTarget;
            BaseMonoEntity entity2 = this.SelectTarget();
            if ((attackTarget == null) && (entity2 != null))
            {
                this.isNewTarget.SetValue(true);
            }
            if (entity2 != null)
            {
                this._avatar.GetActiveAIController().TrySetAttackTarget(entity2);
                return TaskStatus.Success;
            }
            return TaskStatus.Failure;
        }

        protected BaseMonoEntity SelectTarget()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            BaseMonoEntity entity = null;
            float maxValue = float.MaxValue;
            for (int i = 0; i < allMonsters.Count; i++)
            {
                BaseMonoMonster monster = allMonsters[i];
                if (monster.IsActive() && !monster.denySelect)
                {
                    List<BaseMonoAbilityEntity> allHitboxEnabledBodyParts = monster.GetAllHitboxEnabledBodyParts();
                    if (allHitboxEnabledBodyParts.Count > 0)
                    {
                        foreach (BaseMonoAbilityEntity entity2 in allHitboxEnabledBodyParts)
                        {
                            float num3 = Miscs.DistancForVec3IgnoreY(entity2.XZPosition, this._avatar.XZPosition);
                            if ((!this.hasDistanceLimit || ((num3 >= this.minDistance) && (num3 <= this.maxDistance))) && (num3 < maxValue))
                            {
                                maxValue = num3;
                                entity = entity2;
                            }
                        }
                    }
                    else
                    {
                        float num4 = Miscs.DistancForVec3IgnoreY(monster.XZPosition, this._avatar.XZPosition);
                        if ((!this.hasDistanceLimit || ((num4 >= this.minDistance) && (num4 <= this.maxDistance))) && (num4 < maxValue))
                        {
                            maxValue = num4;
                            entity = monster;
                        }
                    }
                }
            }
            if ((entity != null) && entity.IsActive())
            {
                return entity;
            }
            return null;
        }
    }
}

