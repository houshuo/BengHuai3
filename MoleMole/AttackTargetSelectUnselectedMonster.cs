namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("AttackTarget/Avatar")]
    public class AttackTargetSelectUnselectedMonster : BehaviorDesigner.Runtime.Tasks.Action
    {
        private BaseMonoAvatar _avatar;
        public bool hasDistanceLimit;
        public SharedBool isNewTarget;
        public float maxDistance;
        public float minDistance;
        public bool muteAnimRetarget = true;

        public override void OnAwake()
        {
            this._avatar = base.GetComponent<BaseMonoAvatar>();
        }

        public override TaskStatus OnUpdate()
        {
            List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
            List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
            List<BaseMonoMonster> list3 = new List<BaseMonoMonster>();
            for (int i = 0; i < allAvatars.Count; i++)
            {
                BaseMonoAvatar avatar = allAvatars[i];
                if ((avatar != null) && avatar.IsActive())
                {
                    BaseMonoMonster item = avatar.AttackTarget as BaseMonoMonster;
                    if (item != null)
                    {
                        list3.Add(item);
                    }
                }
            }
            if (allMonsters.Count <= 0)
            {
                return TaskStatus.Failure;
            }
            BaseMonoMonster monster2 = null;
            float maxValue = float.MaxValue;
            for (int j = 0; j < allMonsters.Count; j++)
            {
                BaseMonoMonster monster3 = allMonsters[j];
                if (monster3.IsActive())
                {
                    float num4 = Miscs.DistancForVec3IgnoreY(monster3.XZPosition, this._avatar.XZPosition);
                    if (!this.hasDistanceLimit || ((num4 >= this.minDistance) && (num4 <= this.maxDistance)))
                    {
                        if (list3.Contains(monster3))
                        {
                            num4 *= 2f;
                        }
                        if (num4 < maxValue)
                        {
                            maxValue = num4;
                            monster2 = monster3;
                        }
                    }
                }
            }
            if (monster2 == null)
            {
                return TaskStatus.Failure;
            }
            BaseMonoEntity attackTarget = this._avatar.AttackTarget;
            BaseMonoMonster newTarget = monster2;
            this.isNewTarget.SetValue(attackTarget != newTarget);
            if (this.muteAnimRetarget)
            {
                this._avatar.SetMuteAnimRetarget(true);
                this._avatar.SetAttackTarget(newTarget);
            }
            else
            {
                this._avatar.GetActiveAIController().TrySetAttackTarget(newTarget);
            }
            return TaskStatus.Success;
        }
    }
}

