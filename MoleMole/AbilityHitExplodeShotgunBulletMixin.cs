namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityHitExplodeShotgunBulletMixin : AbilityHitExplodeBulletMixin
    {
        private BaseMonoEntity _attackTarget;
        private int _hitCount;
        private HitExplodeShotgunBulletMixin config;

        public AbilityHitExplodeShotgunBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeShotgunBulletMixin) config;
        }

        public override void Core()
        {
            base.Core();
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
            float num = 10f;
            if (this._attackTarget != null)
            {
                num = Vector3.Distance(this._attackTarget.XZPosition, base.actor.entity.XZPosition);
            }
            float t = Mathf.InverseLerp(this.config.ScatterDistanceMax, this.config.ScatterDistanceMin, num);
            bullet.triggerBullet.transform.forward = base.actor.entity.transform.forward;
            Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
            insideUnitCircle.x *= Mathf.Tan(0.01745329f * Mathf.Lerp(this.config.ScatterAngleMinX, this.config.ScatterAngleMaxX, t));
            insideUnitCircle.y *= Mathf.Tan(0.01745329f * Mathf.Lerp(this.config.ScatterAngleMinY, this.config.ScatterAngleMaxY, t));
            Transform transform = bullet.triggerBullet.transform;
            transform.forward += Quaternion.FromToRotation(Vector3.forward, bullet.triggerBullet.transform.forward) * insideUnitCircle;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (!(evt is EvtBulletHit))
            {
                return false;
            }
            if (!((EvtBulletHit) evt).hitEnvironment)
            {
                if (this._hitCount == 0)
                {
                    return false;
                }
                this._hitCount--;
            }
            return base.ListenEvent(evt);
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            for (int i = 0; i < this.config.BulletNum; i++)
            {
                base.OnAbilityTriggered(evt);
            }
            this._attackTarget = base.entity.GetAttackTarget();
            this._hitCount = this.config.MaxHitNum;
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
        }
    }
}

