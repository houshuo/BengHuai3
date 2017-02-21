namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityGrenadesMixin : AbilityHitExplodeBulletMixin
    {
        private List<TraceDelay> _traceBullets;
        private GrenadesMixin config;

        public AbilityGrenadesMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (GrenadesMixin) config;
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
                        if (!delay.isStuck)
                        {
                            actor.triggerBullet.SetCollisionEnabled(false);
                            if ((actor.triggerBullet.transform.position.y < 0.03f) && (delay.SpeedY < 0f))
                            {
                                delay.SpeedY *= -this.config.Elasticity;
                                delay.hitGroundTime++;
                                delay.isTriggered = true;
                                if ((delay.SpeedY < 0.1f) || (delay.hitGroundTime >= 3))
                                {
                                    delay.isStuck = true;
                                    delay.SpeedY = 0f;
                                }
                            }
                            actor.triggerBullet.speedAdd = new Vector3(0f, delay.SpeedY, 0f);
                            actor.triggerBullet.SetupTracing();
                            delay.SpeedY -= (this.config.Gravity * Time.deltaTime) * base.entity.TimeScale;
                        }
                        else
                        {
                            actor.triggerBullet.speedAdd = Vector3.zero;
                            actor.triggerBullet.SetupTracing(actor.triggerBullet.transform.position, 100f, 0f, false);
                        }
                        if (delay.isTriggered)
                        {
                            if (delay.delayTime > 0f)
                            {
                                delay.delayTime -= Time.deltaTime * base.entity.TimeScale;
                            }
                            else
                            {
                                actor.triggerBullet.SetCollisionEnabled(true);
                                if (delay.isStuck)
                                {
                                    this._traceBullets[i] = null;
                                }
                            }
                        }
                        Debug.DrawLine(delay.tarPos, actor.triggerBullet.transform.position, Color.blue);
                    }
                }
            }
        }

        private void InitBulletDirAndPos(AbilityTriggerBullet bullet, Vector3 foward)
        {
            int num = this._traceBullets.SeekAddPosition<TraceDelay>();
            bullet.triggerBullet.transform.forward = foward;
            bullet.triggerBullet.SetCollisionEnabled(false);
            TraceDelay delay = new TraceDelay {
                bulletID = bullet.runtimeID,
                delayTime = this.config.DelayTime,
                tarPos = bullet.triggerBullet.transform.position + ((Vector3) ((foward * this.config.Offset) * UnityEngine.Random.value)),
                SpeedY = 0f,
                isTriggered = false,
                isStuck = false,
                hitGroundTime = 0
            };
            this._traceBullets[num] = delay;
            float speed = base.actor.Evaluate(this.config.BulletSpeed);
            float num3 = Vector3.Distance(this._traceBullets[num].tarPos, bullet.triggerBullet.transform.position);
            float y = bullet.triggerBullet.transform.position.y;
            float gravity = this.config.Gravity;
            while (num3 < ((1f * Mathf.Sqrt((2f * y) / gravity)) * speed))
            {
                bullet.triggerBullet.speed *= 0.8f;
                speed = bullet.triggerBullet.speed;
                if (speed < 3f)
                {
                    break;
                }
            }
            this._traceBullets[num].SpeedY = ((num3 * gravity) / (3f * speed)) * UnityEngine.Random.Range((float) 0.5f, (float) 2f);
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
            int num = this._traceBullets.SeekAddPosition<TraceDelay>();
            bullet.triggerBullet.transform.forward = base.entity.transform.forward;
            bullet.triggerBullet.SetCollisionEnabled(false);
            TraceDelay delay = new TraceDelay {
                bulletID = bullet.runtimeID,
                delayTime = this.config.DelayTime,
                tarPos = bullet.triggerBullet.transform.position,
                SpeedY = 0f,
                isTriggered = false,
                isStuck = false,
                hitGroundTime = 0
            };
            this._traceBullets[num] = delay;
            BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
            if (attackTarget != null)
            {
                Vector3 vector = new Vector3(UnityEngine.Random.Range(-this.config.Offset, this.config.Offset), 0f, UnityEngine.Random.Range(-this.config.Offset, this.config.Offset));
                this._traceBullets[num].tarPos = attackTarget.transform.position + vector;
                Vector3 vector2 = this._traceBullets[num].tarPos - base.entity.transform.position;
                bullet.triggerBullet.transform.forward = vector2.normalized;
            }
            float speed = base.actor.Evaluate(this.config.BulletSpeed);
            float num3 = Vector3.Distance(this._traceBullets[num].tarPos, base.entity.transform.position);
            float y = bullet.triggerBullet.transform.position.y;
            float gravity = this.config.Gravity;
            while (num3 < ((4.2f * Mathf.Sqrt((2f * y) / gravity)) * speed))
            {
                bullet.triggerBullet.speed *= 0.8f;
                speed = bullet.triggerBullet.speed;
                if (speed < 3f)
                {
                    break;
                }
            }
            this._traceBullets[num].SpeedY = (num3 * gravity) / (6f * speed);
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            string bulletTypeName = this.config.BulletTypeName;
            int grenadeAmount = this.config.GrenadeAmount;
            string hitAnimEventID = this.config.HitAnimEventID;
            GrenadesMixinArgument abilityArgument = evt.abilityArgument as GrenadesMixinArgument;
            if (abilityArgument != null)
            {
                grenadeAmount = abilityArgument.BulletAmount;
                if (!string.IsNullOrEmpty(abilityArgument.HitAnimEventID))
                {
                    hitAnimEventID = abilityArgument.HitAnimEventID;
                }
            }
            for (int i = 0; i < grenadeAmount; i++)
            {
                AbilityTriggerBullet bullet = Singleton<DynamicObjectManager>.Instance.CreateAbilityLinearTriggerBullet(bulletTypeName, base.actor, base.instancedAbility.Evaluate(this.config.BulletSpeed), this.config.Targetting, this.config.IgnoreTimeScale, Singleton<DynamicObjectManager>.Instance.GetNextSyncedDynamicObjectRuntimeID(), -1f);
                if ((this.config.BulletEffect != null) && (this.config.BulletEffect.EffectPattern != null))
                {
                    Singleton<EffectManager>.Instance.TriggerEntityEffectPattern(this.config.BulletEffect.EffectPattern, bullet.triggerBullet, this.config.BulletEffectGround);
                }
                BaseMonoEntity entity = null;
                Vector3 forward = base.entity.transform.forward;
                if (evt.otherID != 0)
                {
                    BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(evt.otherID);
                    if ((actor != null) && (actor.entity != null))
                    {
                        entity = actor.entity;
                    }
                }
                if (entity != null)
                {
                    bullet.triggerBullet.transform.position = entity.GetAttachPoint("RootNode").position;
                    forward = entity.transform.forward;
                    float angle = ((360 * i) / (this.config.GrenadeAmount + 1)) - 180;
                    forward = (Vector3) (Quaternion.AngleAxis(angle, Vector3.up) * forward);
                }
                this.InitBulletDirAndPos(bullet, forward);
                base._bulletAttackDatas.Add(bullet.runtimeID, DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(base.actor, hitAnimEventID));
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            this._traceBullets.Clear();
        }

        private class TraceDelay
        {
            public uint bulletID;
            public float delayTime;
            public int hitGroundTime;
            public bool isStuck;
            public bool isTriggered;
            public float SpeedY;
            public Vector3 tarPos;
        }
    }
}

