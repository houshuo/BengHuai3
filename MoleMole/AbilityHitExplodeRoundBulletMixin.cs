namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityHitExplodeRoundBulletMixin : AbilityHitExplodeBulletMixin
    {
        private BaseMonoEntity _attackTarget;
        private List<TraceDelay> _traceBullets;
        private HitExplodeRoundBulletMixin config;

        public AbilityHitExplodeRoundBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeRoundBulletMixin) config;
            this._traceBullets = new List<TraceDelay>();
        }

        public override void Core()
        {
            base.Core();
            for (int i = 0; i < this._traceBullets.Count; i++)
            {
                if (this._traceBullets[i] != null)
                {
                    TraceDelay delay = this._traceBullets[i];
                    AbilityTriggerBullet actor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(delay.bulletID);
                    if (actor != null)
                    {
                        if (delay.holdTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(false);
                            actor.triggerBullet.SetupTracing(actor.triggerBullet.transform.position, 99f, 0f, false);
                            delay.holdTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else if (delay.lifeTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(true);
                            if ((this._attackTarget != null) && (Vector3.Distance(delay.center, this._attackTarget.transform.position) > this.config.CenterTraceRadial))
                            {
                                Vector3 vector4 = this._attackTarget.transform.position - delay.center;
                                delay.centerDir = vector4.normalized;
                            }
                            delay.center.y = actor.triggerBullet.transform.position.y;
                            delay.center += (Vector3) (((delay.centerDir * this.config.CenterSpeed) * Time.deltaTime) * base.entity.TimeScale);
                            Vector3 vector6 = actor.triggerBullet.transform.position - delay.center;
                            Vector3 normalized = vector6.normalized;
                            Vector3 vector2 = (Vector3) (Quaternion.AngleAxis(this.config.RadAngle, Vector3.up) * normalized);
                            Vector3 targetPosition = actor.triggerBullet.transform.position + ((Vector3) (vector2 * base.instancedAbility.Evaluate(this.config.BulletSpeed)));
                            actor.triggerBullet.SetupTracing(targetPosition, 99f, 0f, false);
                            delay.lifeTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else
                        {
                            this._traceBullets[i] = null;
                        }
                    }
                }
            }
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
            int num = this._traceBullets.SeekAddPosition<TraceDelay>();
            TraceDelay delay = new TraceDelay {
                bulletID = bullet.runtimeID,
                lifeTimer = this.config.LifeTime,
                holdTimer = this.config.HoldTime,
                center = base.entity.transform.position,
                centerDir = base.entity.transform.forward
            };
            this._traceBullets[num] = delay;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            base.OnAbilityTriggered(evt);
            this._attackTarget = base.entity.GetAttackTarget();
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            this._traceBullets.Clear();
        }

        private class TraceDelay
        {
            public uint bulletID;
            public Vector3 center;
            public Vector3 centerDir;
            public float holdTimer;
            public float lifeTimer;
        }
    }
}

