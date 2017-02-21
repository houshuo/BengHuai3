namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class AbilityDodgeTeleportMixin : BaseAbilityMixin
    {
        protected float _cdTimer;
        private bool _fadingOut;
        private Vector3 _speed;
        private float _timer;
        private DodgeTeleportMixin config;
        private const float SCREEN_EDGE_RATIO = 0.2f;

        public AbilityDodgeTeleportMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (DodgeTeleportMixin) config;
        }

        protected bool CheckAllowTeleport()
        {
            if (this._cdTimer <= 0f)
            {
                if (base.actor == null)
                {
                    return false;
                }
                string currentSkillID = base.actor.entity.CurrentSkillID;
                if (string.IsNullOrEmpty(currentSkillID))
                {
                    return false;
                }
                if (this.config.TeleportSkillIDs == null)
                {
                    return false;
                }
                if (base.actor.abilityState.ContainsState(AbilityState.Paralyze) || base.actor.abilityState.ContainsState(AbilityState.Stun))
                {
                    return false;
                }
                if (base.actor.abilityPlugin.EvaluateAbilityPredicate(this.config.Predicates, base.instancedAbility, base.instancedModifier, base.actor, null))
                {
                    int index = 0;
                    int length = this.config.TeleportSkillIDs.Length;
                    while (index < length)
                    {
                        if (currentSkillID == this.config.TeleportSkillIDs[index])
                        {
                            return true;
                        }
                        index++;
                    }
                }
            }
            return false;
        }

        protected virtual void ClearTargetAttackTarget(uint sourceID)
        {
        }

        public override void Core()
        {
            float num = Time.deltaTime * base.entity.TimeScale;
            if (this._cdTimer > 0f)
            {
                this._cdTimer -= num;
            }
            if (this._timer > 0f)
            {
                this._timer -= num;
                BaseMonoAnimatorEntity animatorEntity = base.entity as BaseMonoAnimatorEntity;
                if (animatorEntity != null)
                {
                    if (this._timer <= 0f)
                    {
                        animatorEntity.SetHasAdditiveVelocity(false);
                        animatorEntity.SetAdditiveVelocity(Vector3.zero);
                        animatorEntity.PopHighspeedMovement();
                    }
                    if (this.config.NeedFade)
                    {
                        this.ProcessFade(animatorEntity);
                    }
                }
            }
        }

        protected virtual Vector3 GetDirection(TeleportDirectionMode mode, float angle)
        {
            Vector3 forward = Vector3.forward;
            if (mode == TeleportDirectionMode.UseAngle)
            {
                forward = (Vector3) (Quaternion.Euler(0f, angle, 0f) * base.entity.transform.forward);
            }
            else if (mode == TeleportDirectionMode.CameraCenter)
            {
                bool flag = false;
                float x = Camera.main.WorldToScreenPoint(base.entity.transform.position).x;
                if ((x < (Screen.width * 0.2f)) || (x > (Screen.width * 0.8f)))
                {
                    flag = true;
                }
                if (flag)
                {
                    Vector3 xZPosition = base.entity.XZPosition;
                    Vector3 position = Camera.main.transform.position;
                    position = new Vector3(position.x, 0f, position.z);
                    Vector3 rhs = Camera.main.transform.forward;
                    rhs = new Vector3(rhs.x, 0f, rhs.z);
                    Vector3 lhs = position - xZPosition;
                    if (Vector3.Cross(lhs, rhs).y > 0f)
                    {
                        forward = (Vector3) (Quaternion.Euler(0f, 90f, 0f) * lhs);
                    }
                    else
                    {
                        forward = (Vector3) (Quaternion.Euler(0f, 270f, 0f) * lhs);
                    }
                }
                else if (UnityEngine.Random.value > 0.5f)
                {
                    forward = (Vector3) (Quaternion.Euler(0f, 90f, 0f) * base.entity.transform.forward);
                }
                else
                {
                    forward = (Vector3) (Quaternion.Euler(0f, 270f, 0f) * base.entity.transform.forward);
                }
            }
            return forward.normalized;
        }

        private Vector3 GetSpawnPointPos(string spawnName)
        {
            MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
            int namedSpawnPointIx = stageEnv.GetNamedSpawnPointIx(spawnName);
            return stageEnv.spawnPoints[namedSpawnPointIx].transform.position;
        }

        private Vector3 GetSpawnPointPos(float distance, bool fromTarget = false)
        {
            Vector3 position = base.entity.transform.position;
            if (fromTarget)
            {
                BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
                if (attackTarget != null)
                {
                    position = attackTarget.transform.position;
                }
            }
            MonoStageEnv stageEnv = Singleton<StageManager>.Instance.GetStageEnv();
            int index = -1;
            float num2 = 100f;
            for (int i = 0; i < Singleton<StageManager>.Instance.GetStageEnv().spawnPoints.Length; i++)
            {
                float num4 = Mathf.Abs((float) (Vector3.Distance(stageEnv.spawnPoints[i].transform.position, position) - distance));
                if (num4 < num2)
                {
                    index = i;
                    num2 = num4;
                }
            }
            return stageEnv.spawnPoints[index].transform.position;
        }

        protected Vector3 GetTeleportPosition(TeleportDirectionMode mode, float angle, float distance)
        {
            Vector3 direction;
            switch (mode)
            {
                case TeleportDirectionMode.UseAngle:
                case TeleportDirectionMode.CameraCenter:
                case TeleportDirectionMode.UseSteerAngle:
                    direction = this.GetDirection(this.config.DirectionMode, base.instancedAbility.Evaluate(this.config.Angle));
                    return (base.entity.transform.position + ((Vector3) (direction * distance)));

                case TeleportDirectionMode.FromTarget:
                {
                    BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
                    if (attackTarget != null)
                    {
                        Vector3 vector2 = base.entity.transform.position - attackTarget.transform.position;
                        direction = vector2.normalized;
                        return (attackTarget.transform.position + ((Vector3) (direction * distance)));
                    }
                    return base.entity.transform.position;
                }
                case TeleportDirectionMode.SpawnPoint:
                    return this.GetSpawnPointPos(this.config.SpawnPoint);

                case TeleportDirectionMode.SpawnPointByDistance:
                    return this.GetSpawnPointPos(this.config.Distance, false);

                case TeleportDirectionMode.SpawnPointByDistanceFromTarget:
                    return this.GetSpawnPointPos(this.config.Distance, true);
            }
            return base.entity.transform.position;
        }

        protected void IgnoreHitDamage(EvtBeingHit evtHit)
        {
            if (evtHit != null)
            {
                evtHit.attackData.Reject(AttackResult.RejectType.RejectAll);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (this.CheckAllowTeleport())
            {
                this.Teleport();
            }
        }

        protected virtual void OnPostBeingHit(EvtBeingHit evtHit)
        {
            if ((this.config.CanHitTrigger && (evtHit.attackData.hitType == AttackResult.ActorHitType.Ranged)) && this.CheckAllowTeleport())
            {
                this.IgnoreHitDamage(evtHit);
                this.Teleport();
                this.ClearTargetAttackTarget(evtHit.sourceID);
            }
            if (this._timer > 0f)
            {
                this.IgnoreHitDamage(evtHit);
            }
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                this.OnPostBeingHit((EvtBeingHit) evt);
            }
            return true;
        }

        private void ProcessFade(BaseMonoAnimatorEntity animatorEntity)
        {
            if (this._fadingOut && (this._timer < 0.03f))
            {
                this._fadingOut = false;
                animatorEntity.SetTrigger("DodgeFadeIn");
            }
        }

        private void Teleport()
        {
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.TeleportActions, base.instancedAbility, base.instancedModifier, null, null);
            Vector3 position = this.GetTeleportPosition(this.config.DirectionMode, base.instancedAbility.Evaluate(this.config.Angle), this.config.Distance);
            this.TeleportTo(position, this.config.TeleportTime);
            if (base.instancedAbility.Evaluate(this.config.CDTime) > 0f)
            {
                this._cdTimer = base.instancedAbility.Evaluate(this.config.CDTime);
            }
            Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(base.actor.runtimeID), MPEventDispatchMode.Normal);
        }

        protected void TeleportTo(Vector3 position, float time)
        {
            this._timer = time;
            this._speed = (Vector3) ((position - base.entity.gameObject.transform.position) / this._timer);
            BaseMonoAnimatorEntity entity = base.entity as BaseMonoAnimatorEntity;
            if (entity != null)
            {
                if (this.config.NeedFade)
                {
                    entity.SetTrigger("DodgeFadeOut");
                    this._fadingOut = true;
                }
                entity.SetHasAdditiveVelocity(true);
                entity.SetAdditiveVelocity(this._speed);
                entity.PushHighspeedMovement();
            }
        }
    }
}

