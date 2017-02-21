namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class AbilityHitExplodeTracingPositionBulletMixin : AbilityHitExplodeBulletMixin
    {
        private List<TraceDelay> _traceBullets;
        private HitExplodeTracePositionBulletMixin config;

        public AbilityHitExplodeTracingPositionBulletMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (HitExplodeTracePositionBulletMixin) config;
            this._traceBullets = new List<TraceDelay>();
        }

        private Vector3 AdjustLevelCollision(Vector3 origin, Vector3 offset)
        {
            float num = 0.2f;
            Vector3 vector = offset;
            int num2 = 4;
            for (int i = 0; i < num2; i++)
            {
                RaycastHit hit;
                Ray ray = new Ray(origin + ((Vector3) (Vector3.up * num)), vector.normalized);
                if (!Physics.Raycast(ray, out hit, vector.magnitude, (((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER) | (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
                {
                    break;
                }
                vector = (Vector3) (Quaternion.AngleAxis(360f / ((float) num2), Vector3.up) * vector);
            }
            return (origin + vector);
        }

        private Vector3 CalculateTraceTargetPosition(bool baseOnTarget, float distance, float angle)
        {
            Vector3 xZPosition = base.entity.XZPosition;
            BaseMonoEntity attackTarget = null;
            if (baseOnTarget)
            {
                attackTarget = base.entity.GetAttackTarget();
            }
            else
            {
                attackTarget = base.entity;
            }
            if (attackTarget != null)
            {
                xZPosition = this.AdjustLevelCollision(attackTarget.XZPosition, (Vector3) ((Quaternion.Euler(0f, angle, 0f) * attackTarget.transform.forward) * distance));
            }
            Debug.DrawLine(xZPosition, xZPosition + ((Vector3) (Vector3.up * 5f)), Color.red, 3f);
            return xZPosition;
        }

        public override void Core()
        {
            base.Core();
            for (int i = 0; i < this._traceBullets.Count; i++)
            {
                if (this._traceBullets[i] != null)
                {
                    TraceDelay delay = this._traceBullets[i];
                    delay.traceTimer -= Time.deltaTime * base.entity.TimeScale;
                    if (delay.traceTimer <= 0f)
                    {
                        this._traceBullets[i] = null;
                        AbilityTriggerBullet actor = Singleton<EventManager>.Instance.GetActor<AbilityTriggerBullet>(delay.bulletID);
                        if (((actor != null) && (actor.triggerBullet != null)) && actor.triggerBullet.IsActive())
                        {
                            actor.triggerBullet.SetupTracing(this.CalculateTraceTargetPosition(this.config.BaseOnTarget, this.config.Distance, this.config.Angle), this.config.TracingLerpCoef, this.config.TracingLerpCoefAcc, false);
                        }
                    }
                }
            }
        }

        protected override void InitBulletForward(AbilityTriggerBullet bullet)
        {
            int num = this._traceBullets.SeekAddPosition<TraceDelay>();
            TraceDelay delay = new TraceDelay {
                traceTimer = this.config.TraceStartDelay,
                bulletID = bullet.runtimeID
            };
            this._traceBullets[num] = delay;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            base.OnAbilityTriggered(evt);
        }

        public override void OnRemoved()
        {
            base.OnRemoved();
            this._traceBullets.Clear();
        }

        private class TraceDelay
        {
            public uint bulletID;
            public float traceTimer;
        }
    }
}

