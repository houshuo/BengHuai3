namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityHitExplodeRandomBulletMixin : AbilityHitExplodeBulletMixin
    {
        private BaseMonoEntity _attackTarget;
        private float _offsetAngle;
        private List<TraceDelay> _traceBullets;
        private HitExplodeRandomBulletMixin config;

        public AbilityHitExplodeRandomBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._offsetAngle = UnityEngine.Random.Range((float) 0f, (float) 6.283185f);
            this.config = (HitExplodeRandomBulletMixin) config;
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
                        if (this._attackTarget != null)
                        {
                            delay.dummyPos += Vector3.ClampMagnitude(this._attackTarget.transform.position - delay.dummyPos, this.config.TraceSpeed * base.entity.TimeScale);
                        }
                        if (delay.holdTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(false);
                            actor.triggerBullet.SetupTracing(actor.triggerBullet.transform.position, this.config.SteerCoef, 0f, false);
                            actor.triggerBullet.IgnoreTimeScale = this.config.IgnoreTimeScale;
                            delay.holdTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else if (delay.lifeTimer > 0f)
                        {
                            actor.triggerBullet.SetCollisionEnabled(true);
                            Vector3 vector3 = delay.dummyPos - delay.startPos;
                            Vector3 normalized = vector3.normalized;
                            Vector3 targetPosition = actor.triggerBullet.transform.position + ((Vector3) ((normalized * base.instancedAbility.Evaluate(this.config.BulletSpeed)) * this.config.LifeTime));
                            targetPosition.y = 0f;
                            targetPosition += delay.targetOffset;
                            actor.triggerBullet.SetupTracing(targetPosition, this.config.SteerCoef, 0f, false);
                            actor.triggerBullet.IgnoreTimeScale = this.config.IgnoreTimeScale;
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
            Vector3 position = new Vector3(UnityEngine.Random.Range(-this.config.RandomPosX, this.config.RandomPosX), UnityEngine.Random.Range(-this.config.RandomPosY, this.config.RandomPosY), UnityEngine.Random.Range(-this.config.RandomPosZ, this.config.RandomPosZ));
            Transform transform = bullet.triggerBullet.transform;
            transform.position += bullet.triggerBullet.transform.TransformPoint(position) - bullet.triggerBullet.transform.localPosition;
            Transform transform2 = bullet.triggerBullet.transform;
            transform2.localRotation *= Quaternion.Euler(10f, 0f, UnityEngine.Random.Range(-2, 3) * 10f);
            this._offsetAngle += 1.795196f;
            Vector3 vector2 = new Vector3(this.config.TargetOffset * Mathf.Sin(this._offsetAngle), 0f, this.config.TargetOffset * Mathf.Cos(this._offsetAngle));
            TraceDelay delay = new TraceDelay {
                bulletID = bullet.runtimeID,
                holdTimer = this.config.HoldTime,
                lifeTimer = this.config.LifeTime,
                startPos = bullet.triggerBullet.transform.position,
                dummyPos = bullet.triggerBullet.transform.position,
                targetOffset = vector2
            };
            this._traceBullets[num] = delay;
            if (this._attackTarget != null)
            {
                this._traceBullets[num].dummyPos = this._attackTarget.transform.position;
            }
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
            public Vector3 dummyPos;
            public float holdTimer;
            public float lifeTimer;
            public Vector3 startPos;
            public Vector3 targetOffset;
        }
    }
}

