namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterForceFollowMixin : BaseAbilityMixin
    {
        private bool _isApproching;
        private bool _isFollow;
        private bool _isKeepingAway;
        private BaseMonoMonster _monster;
        private BaseMonoEntity _targetEntity;
        private MonsterForceFollowMixin config;

        public AbilityMonsterForceFollowMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterForceFollowMixin) config;
            this._monster = base.entity as BaseMonoMonster;
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
                if (this._monster.GetCurrentNormalizedTime() > this.config.NormalizeTimeEnd)
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

        private bool CheckShouldFollow()
        {
            if (this._targetEntity != null)
            {
                float num = Vector3.Distance(base.entity.transform.position, this._targetEntity.transform.position);
                if (num > (!this._isApproching ? this.config.MaxDistance : this.config.TargetDistance))
                {
                    this._isApproching = true;
                    this._isKeepingAway = false;
                    return true;
                }
                if (num < (!this._isKeepingAway ? this.config.MinDistance : this.config.TargetDistance))
                {
                    this._isApproching = false;
                    this._isKeepingAway = true;
                    return true;
                }
            }
            return false;
        }

        public override void Core()
        {
            if (this._isFollow)
            {
                if (!this.CheckAllowFollow())
                {
                    this.EndFollow();
                }
                else if (!this.CheckShouldFollow())
                {
                    this._monster.SetHasAdditiveVelocity(false);
                    this._monster.SetAdditiveVelocity(Vector3.zero);
                }
                else
                {
                    float num = Time.deltaTime * base.entity.TimeScale;
                    if (num != 0f)
                    {
                        Vector3 vector4 = base.entity.transform.position - this._targetEntity.transform.position;
                        Vector3 normalized = vector4.normalized;
                        Vector3 vector2 = this._targetEntity.transform.position + ((Vector3) (normalized * this.config.TargetDistance));
                        Vector3 velocity = (Vector3) ((vector2 - base.entity.transform.position) / num);
                        if (velocity.magnitude > this.config.FollowSpeed)
                        {
                            velocity = (Vector3) (velocity.normalized * this.config.FollowSpeed);
                        }
                        velocity.y = 0f;
                        this._monster.SetHasAdditiveVelocity(true);
                        this._monster.SetAdditiveVelocity(velocity);
                    }
                }
            }
        }

        private void EndFollow()
        {
            this._monster.SetHasAdditiveVelocity(false);
            this._monster.SetAdditiveVelocity(Vector3.zero);
            this._isFollow = false;
            this._isApproching = false;
            this._isKeepingAway = false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this.StartFollow();
        }

        private void StartFollow()
        {
            this._targetEntity = base.entity.GetAttackTarget();
            Singleton<EventManager>.Instance.FireEvent(new EvtTeleport(base.actor.runtimeID), MPEventDispatchMode.Normal);
            this._isFollow = true;
            this._isApproching = false;
            this._isKeepingAway = false;
        }
    }
}

