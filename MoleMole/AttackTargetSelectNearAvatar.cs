namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using System.Collections.Generic;

    [TaskCategory("AttackTarget/Monster")]
    public class AttackTargetSelectNearAvatar : BehaviorDesigner.Runtime.Tasks.Action
    {
        protected BaseMonoMonster _monster;
        public float ChangeAvatarDistanceRatioBias = 0.8f;
        private const float NEAR_FAR_ATTACK_DIS_THRESHOLD = 5f;
        public SharedAttackType TargetType;

        public override void OnAwake()
        {
            this._monster = base.GetComponent<BaseMonoMonster>();
        }

        public override void OnStart()
        {
        }

        public override TaskStatus OnUpdate()
        {
            this.SelectNearestAvatar();
            if ((this._monster.AttackTarget == null) || !this._monster.AttackTarget.IsActive())
            {
                return TaskStatus.Running;
            }
            if (this.TargetType != null)
            {
                if ((this._monster.AttackTarget as BaseMonoAvatar).config.AIArguments.AttackDistance > 5f)
                {
                    this.TargetType.SetValue(AttackType.FarAttack);
                }
                else
                {
                    this.TargetType.SetValue(AttackType.NearAttack);
                }
            }
            return TaskStatus.Success;
        }

        protected void SelectNearestAvatar()
        {
            List<BaseMonoAvatar> list = new List<BaseMonoAvatar>();
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
            {
                if (avatar.IsActive())
                {
                    list.Add(avatar);
                }
            }
            if (list.Count != 0)
            {
                BaseMonoEntity newTarget = null;
                bool flag = false;
                if ((this._monster.AttackTarget != null) && this._monster.AttackTarget.IsActive())
                {
                    if ((this._monster.AttackTarget as BaseMonoAnimatorEntity).denySelect)
                    {
                        flag = false;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                float positiveInfinity = float.PositiveInfinity;
                if (flag)
                {
                    positiveInfinity = Miscs.DistancForVec3IgnoreY(this._monster.AttackTarget.XZPosition, this._monster.XZPosition) * this.ChangeAvatarDistanceRatioBias;
                }
                float num2 = float.PositiveInfinity;
                foreach (BaseMonoAvatar avatar2 in list)
                {
                    if (((avatar2 != null) && avatar2.IsActive()) && !avatar2.denySelect)
                    {
                        if (flag && (Miscs.DistancForVec3IgnoreY(avatar2.XZPosition, this._monster.XZPosition) < positiveInfinity))
                        {
                            newTarget = avatar2;
                        }
                        else if (!flag && (Miscs.DistancForVec3IgnoreY(avatar2.XZPosition, this._monster.XZPosition) < num2))
                        {
                            newTarget = avatar2;
                            num2 = Miscs.DistancForVec3IgnoreY(avatar2.XZPosition, this._monster.XZPosition);
                        }
                    }
                }
                this._monster.SetAttackTarget(newTarget);
            }
        }
    }
}

