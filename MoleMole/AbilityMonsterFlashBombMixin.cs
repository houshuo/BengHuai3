namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMonsterFlashBombMixin : BaseAbilityMixin
    {
        private float _delayTimer;
        private BaseMonoMonster _monster;
        private Vector3 _position;
        private State _state;
        private MonsterFlashBombMixin config;

        public AbilityMonsterFlashBombMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MonsterFlashBombMixin) config;
            this._monster = (BaseMonoMonster) base.entity;
        }

        public override void Core()
        {
            base.Core();
            if (this._state != State.Idle)
            {
                this._delayTimer -= base.entity.TimeScale * Time.deltaTime;
                if (this._delayTimer <= 0f)
                {
                    this.DoFlashExplode();
                    this._state = State.Idle;
                }
            }
        }

        private void DoFlashExplode()
        {
            bool flag = false;
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllAvatars())
            {
                if (((avatar != null) && avatar.IsActive()) && (Vector3.Angle(this._position - avatar.XZPosition, avatar.transform.forward) <= this.config.Angle))
                {
                    BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(avatar.GetRuntimeID());
                    foreach (string str in this.config.ModifierNames)
                    {
                        actor.abilityPlugin.ApplyModifier(base.instancedAbility, str);
                    }
                    if (Singleton<AvatarManager>.Instance.IsLocalAvatar(avatar.GetRuntimeID()))
                    {
                        flag = true;
                        base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.SuccessActions, base.instancedAbility, base.instancedModifier, null, null);
                    }
                }
            }
            if (!flag)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.FailActions, base.instancedAbility, base.instancedModifier, null, null);
            }
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            this._delayTimer = this.config.DelayTime;
            this._position = base.entity.XZPosition;
            this._state = State.Running;
            base.FireMixinEffect(this.config.TriggerEffect, base.entity, false);
        }

        private enum State
        {
            Idle,
            Running
        }
    }
}

