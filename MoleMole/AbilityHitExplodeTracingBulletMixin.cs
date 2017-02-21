namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityHitExplodeTracingBulletMixin : AbilityHitExplodeBulletMixin
    {
        private BaseMonoEntity _attackTarget;
        private List<TraceDelay> _traceBullets;
        private HitExplodeTracingBulletMixin config;

        public AbilityHitExplodeTracingBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeTracingBulletMixin) config;
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
                        BaseMonoEntity subAttackTarget;
                        if (delay.subAttackTarget != null)
                        {
                            subAttackTarget = delay.subAttackTarget;
                        }
                        else
                        {
                            subAttackTarget = this._attackTarget;
                        }
                        if (delay.lineTimer > 0f)
                        {
                            delay.lineTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else if (delay.turnTimer > 0f)
                        {
                            if (Mathf.Approximately(delay.turnTimer, this.config.TraceStartDelay) && (subAttackTarget != null))
                            {
                                Vector3 position = actor.triggerBullet.transform.position;
                                Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
                                Vector2 lhs = new Vector2(onUnitSphere.x, onUnitSphere.z);
                                if (Vector2.Dot(lhs, new Vector2(actor.triggerBullet.transform.forward.x, actor.triggerBullet.transform.forward.z)) < 0f)
                                {
                                    lhs = (Vector2) (lhs * -1f);
                                }
                                onUnitSphere = new Vector3(lhs.x, onUnitSphere.y, lhs.y);
                                Vector3 vector4 = (Vector3) (onUnitSphere * Vector3.Distance(actor.triggerBullet.transform.position, subAttackTarget.transform.position));
                                position += vector4;
                                Mathf.Clamp(position.y, -this.config.RandomHeight, this.config.RandomHeight);
                                actor.triggerBullet.SetupTracing(position, this.config.TracingLerpCoef, 0f, false);
                            }
                            delay.turnTimer -= Time.deltaTime * base.entity.TimeScale;
                        }
                        else
                        {
                            if (subAttackTarget != null)
                            {
                                Vector3 vector5;
                                if (this.config.TraceRootNode)
                                {
                                    vector5 = subAttackTarget.GetAttachPoint("RootNode").position;
                                }
                                else
                                {
                                    vector5 = subAttackTarget.transform.position;
                                }
                                if (this.config.RandomOffsetDistance > 0f)
                                {
                                    Vector3 vector9 = new Vector3(UnityEngine.Random.value - 0.5f, UnityEngine.Random.value - 0.5f);
                                    Vector3 vector6 = (Vector3) (vector9.normalized * this.config.RandomOffsetDistance);
                                    vector6 += (Vector3) (Vector3.up * UnityEngine.Random.Range(0f, this.config.RandomHeight));
                                    vector5 += vector6;
                                }
                                if (!this.config.TraceY)
                                {
                                    vector5.y = actor.triggerBullet.transform.position.y;
                                }
                                actor.triggerBullet.SetupTracing(vector5, this.config.TracingLerpCoef, 0f, this.config.PassBy);
                            }
                            this._traceBullets[i] = null;
                        }
                    }
                }
            }
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
        }

        protected override void InitBulletForwardWithArgument(AbilityTriggerBullet bullet, HitExplodeTracingBulletMixinArgument arg, uint otherID)
        {
            base.InitBulletForwardWithArgument(bullet, arg, otherID);
            if (this.config.IsRandomInit)
            {
                if (this.config.IsRandomInitCone)
                {
                    bullet.triggerBullet.transform.forward = (Vector3) ((Quaternion.AngleAxis((float) UnityEngine.Random.Range(-40, 40), base.entity.transform.up) * Quaternion.AngleAxis((float) -UnityEngine.Random.Range(-10, 40), base.entity.transform.right)) * base.entity.transform.forward);
                }
                else
                {
                    bullet.triggerBullet.transform.forward = (Vector3) (Quaternion.AngleAxis((float) -UnityEngine.Random.Range(20, 0x2d), base.entity.transform.right) * base.entity.transform.forward);
                }
            }
            BaseMonoEntity entity = null;
            if (otherID != 0)
            {
                entity = Singleton<EventManager>.Instance.GetEntity(otherID);
            }
            int num = this._traceBullets.SeekAddPosition<TraceDelay>();
            TraceDelay delay = new TraceDelay {
                lineTimer = this.config.TurnStartDelay,
                turnTimer = this.config.TraceStartDelay,
                bulletID = bullet.runtimeID,
                subAttackTarget = entity
            };
            this._traceBullets[num] = delay;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            base.OnAbilityTriggered(evt);
            if (this.config.IsRandomTarget)
            {
                this.SelectRandomTarget();
            }
            else
            {
                this._attackTarget = base.entity.GetAttackTarget();
            }
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            this._traceBullets.Clear();
        }

        private void SelectRandomTarget()
        {
            if (base.entity is BaseMonoAvatar)
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                if (allMonsters.Count > 0)
                {
                    this._attackTarget = allMonsters[UnityEngine.Random.Range(0, allMonsters.Count)];
                }
            }
            else if (base.entity is BaseMonoMonster)
            {
                this._attackTarget = base.entity.GetAttackTarget();
            }
        }

        private class TraceDelay
        {
            public uint bulletID;
            public float lineTimer;
            public BaseMonoEntity subAttackTarget;
            public float turnTimer;
        }
    }
}

