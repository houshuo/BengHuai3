namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public abstract class BaseAvatarController : BaseController
    {
        public BaseAvatarController(BaseMonoEntity avatar) : base(2, avatar)
        {
            this.avatar = (BaseMonoAvatar) avatar;
            this.controlData = new AvatarControlData();
        }

        public virtual void SetActive(bool isActive)
        {
            this.active = isActive;
        }

        protected void TryAttack()
        {
            this.controlData.useAttack = true;
            this.controlData.hasAnyControl = true;
        }

        public void TryClearAttackTarget()
        {
            this.controlData.hasSetAttackTarget = true;
            this.controlData.attackTarget = null;
            this.controlData.hasAnyControl = true;
        }

        protected void TryHoldAttack()
        {
            this.controlData.useHoldAttack = true;
            this.controlData.hasAnyControl = true;
        }

        protected void TryOrderMove(bool orderMove)
        {
            this.controlData.hasOrderMove = true;
            this.controlData.orderMove = orderMove;
            this.controlData.hasAnyControl = true;
        }

        public void TrySetAttackTarget(BaseMonoEntity attackTarget)
        {
            this.controlData.hasSetAttackTarget = true;
            this.controlData.attackTarget = attackTarget;
            this.controlData.hasAnyControl = true;
        }

        protected void TrySteer(Vector3 dir, float lerpRatio)
        {
            this.controlData.hasSteer = true;
            this.controlData.lerpRatio = lerpRatio;
            this.controlData.steerDirection = dir.normalized;
            this.controlData.hasAnyControl = true;
        }

        protected void TryUseSkill(int skillIx)
        {
            this.controlData.useSkills[skillIx] = true;
            this.controlData.hasAnyControl = true;
        }

        public bool active { get; protected set; }

        public BaseMonoAvatar avatar { get; private set; }

        public AvatarControlData controlData { get; private set; }
    }
}

