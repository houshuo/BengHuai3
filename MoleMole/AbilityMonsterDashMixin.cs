namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterDashMixin : BaseAbilityMixin
    {
        private BaseMonoMonster _monster;
        private Vector3 _startPos;
        private Vector3 _targetPos;
        private float _timer;
        private MonsterDashMixin config;

        public AbilityMonsterDashMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterDashMixin) config;
            this._monster = base.entity as BaseMonoMonster;
            this._timer = -1f;
        }

        private bool CheckAllowFollow()
        {
            MonsterActor actor = base.actor as MonsterActor;
            if (actor != null)
            {
                string currentSkillID = actor.monster.CurrentSkillID;
                if (string.IsNullOrEmpty(currentSkillID))
                {
                    return false;
                }
                if (this.config.SkillIDs == null)
                {
                    return false;
                }
                if (base.actor.abilityState.ContainsState(AbilityState.Paralyze) || base.actor.abilityState.ContainsState(AbilityState.Stun))
                {
                    return false;
                }
                int index = 0;
                int length = this.config.SkillIDs.Length;
                while (index < length)
                {
                    if (currentSkillID == this.config.SkillIDs[index])
                    {
                        return true;
                    }
                    index++;
                }
            }
            return false;
        }

        public override void Core()
        {
            if (this._timer > 0f)
            {
                if (!this.CheckAllowFollow())
                {
                    this.EndDash();
                }
                else
                {
                    this._timer -= Time.deltaTime * base.entity.TimeScale;
                    if (this._timer <= 0f)
                    {
                        this.EndDash();
                    }
                    else if (Vector3.Distance(base.entity.transform.position, this._startPos) >= Vector3.Distance(this._startPos, this._targetPos))
                    {
                        this.EndDash();
                    }
                }
            }
        }

        private void EndDash()
        {
            this._monster.SetNeedOverrideVelocity(false);
            this._monster.SetOverrideVelocity(Vector3.zero);
            this._monster.PopHighspeedMovement();
            this._timer = -1f;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this.StartDash();
        }

        private void StartDash()
        {
            float dashTime = this.config.DashTime;
            if (dashTime != 0f)
            {
                this._timer = dashTime;
                BaseMonoEntity attackTarget = base.entity.GetAttackTarget();
                if (attackTarget != null)
                {
                    this._startPos = base.entity.transform.position;
                    float num2 = Vector3.Distance(this._startPos, attackTarget.transform.position) - this.config.TargetDistance;
                    this._targetPos = this._startPos + ((Vector3) (base.entity.transform.forward * num2));
                    Vector3 velocity = (Vector3) ((this._targetPos - this._startPos) / dashTime);
                    this._monster.SetNeedOverrideVelocity(true);
                    this._monster.SetOverrideVelocity(velocity);
                    this._monster.PushHighspeedMovement();
                    Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(base.actor.runtimeID), MPEventDispatchMode.Normal);
                    Debug.DrawLine(this._startPos, this._targetPos, Color.yellow, 2f);
                }
            }
        }
    }
}

