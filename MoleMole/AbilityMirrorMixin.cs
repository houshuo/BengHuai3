namespace MoleMole
{
    using BehaviorDesigner.Runtime;
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AbilityMirrorMixin : BaseAbilityMixin
    {
        private int _curMirrorIx;
        private EntityTimer _delayTimer;
        private int _mirrorAmount;
        private MirrorData[] _mirrorDatas;
        private float _mirrorLifespan;
        private State _state;
        private MirrorMixin config;

        public AbilityMirrorMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (MirrorMixin) config;
            this._mirrorLifespan = instancedAbility.Evaluate(this.config.MirrorLastingTime);
            this._mirrorAmount = instancedAbility.Evaluate(this.config.MirrorAmount);
            this._delayTimer = new EntityTimer(this.config.DelayTime);
            this._mirrorDatas = new MirrorData[this._mirrorAmount];
            for (int i = 0; i < this._mirrorAmount; i++)
            {
                this._mirrorDatas[i] = new MirrorData();
            }
            this._state = State.Idle;
        }

        public override void Core()
        {
            if (this._state == State.SpawningMirrors)
            {
                this._delayTimer.Core(1f);
                if (this._delayTimer.isTimeUp)
                {
                    this.SpawnSingleMirror(this._curMirrorIx);
                    this._curMirrorIx++;
                    if (this._curMirrorIx >= this._mirrorAmount)
                    {
                        this._state = State.MirrorActive;
                    }
                    else
                    {
                        this._delayTimer.timespan = this.config.PerMirrorDelayTime;
                        this._delayTimer.Reset(true);
                    }
                }
            }
            if ((this._state == State.SpawningMirrors) || (this._state == State.MirrorActive))
            {
                int num = 0;
                for (int i = 0; i < this._mirrorAmount; i++)
                {
                    if (this._mirrorDatas[i].mirrorRuntimeID != 0)
                    {
                        num++;
                        MirrorData data1 = this._mirrorDatas[i];
                        data1.mirrorLifetime += Time.deltaTime * base.entity.TimeScale;
                        if (this._mirrorDatas[i].mirrorLifetime > this._mirrorLifespan)
                        {
                            this.KillSingleMirror(i);
                        }
                        else if ((this._mirrorDatas[i].mirrorLifetime > (this._mirrorLifespan - this.config.AheadTime)) && !this._mirrorDatas[i].destroyAheadActionTriggered)
                        {
                            BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(this._mirrorDatas[i].mirrorRuntimeID);
                            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.MirrorAheadDestroyActions, base.instancedAbility, base.instancedModifier, other, null);
                            this._mirrorDatas[i].destroyAheadActionTriggered = true;
                        }
                    }
                }
                if ((this._state == State.MirrorActive) && (num == 0))
                {
                    for (int j = 0; j < this.config.SelfModifiers.Length; j++)
                    {
                        base.actor.abilityPlugin.TryRemoveModifier(base.instancedAbility, this.config.SelfModifiers[j]);
                    }
                    this._state = State.Idle;
                }
            }
        }

        private void KillSingleMirror(int ix)
        {
            BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(this._mirrorDatas[ix].mirrorRuntimeID);
            if (!this._mirrorDatas[ix].destroyAheadActionTriggered)
            {
                base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.MirrorAheadDestroyActions, base.instancedAbility, base.instancedModifier, other, null);
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.MirrorDestroyActions, base.instancedAbility, base.instancedModifier, other, null);
            other.isAlive = 0;
            other.entity.SetDied(KillEffect.KillImmediately);
            this._mirrorDatas[ix].Reset();
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtKilled) && this.ListenKilled((EvtKilled) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            for (int i = 0; i < this._mirrorDatas.Length; i++)
            {
                MirrorData data = this._mirrorDatas[i];
                if (data.mirrorRuntimeID == evt.targetID)
                {
                    this.KillSingleMirror(i);
                    return true;
                }
            }
            return false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if ((this._state == State.SpawningMirrors) || (this._state == State.MirrorActive))
            {
                for (int j = 0; j < this._mirrorDatas.Length; j++)
                {
                    if (this._mirrorDatas[j].mirrorRuntimeID != 0)
                    {
                        this.KillSingleMirror(j);
                    }
                }
            }
            this._state = State.SpawningMirrors;
            this._curMirrorIx = 0;
            this._delayTimer.SetActive(true);
            this._mirrorLifespan = base.instancedAbility.Evaluate(this.config.MirrorLastingTime);
            if (this.config.ApplyAttackerWitchTimeRatio && (evt.TriggerEvent != null))
            {
                EvtEvadeSuccess triggerEvent = evt.TriggerEvent as EvtEvadeSuccess;
                if (triggerEvent != null)
                {
                    MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(triggerEvent.attackerID);
                    if (actor != null)
                    {
                        ConfigMonsterAnimEvent event2 = SharedAnimEventData.ResolveAnimEvent(actor.config, triggerEvent.skillID);
                        if (event2 != null)
                        {
                            this._mirrorLifespan *= event2.AttackProperty.WitchTimeRatio;
                        }
                    }
                }
            }
            for (int i = 0; i < this.config.SelfModifiers.Length; i++)
            {
                base.actor.abilityPlugin.ApplyModifier(base.instancedAbility, this.config.SelfModifiers[i]);
            }
        }

        public override void OnAdded()
        {
            this._delayTimer.SetActive(false);
            this._state = State.Idle;
            for (int i = 0; i < this._mirrorDatas.Length; i++)
            {
                this._mirrorDatas[i].Reset();
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base.actor.runtimeID);
            this._curMirrorIx = 0;
        }

        public override void OnRemoved()
        {
            for (int i = 0; i < this._mirrorDatas.Length; i++)
            {
                if (this._mirrorDatas[i].mirrorRuntimeID != 0)
                {
                    this.KillSingleMirror(i);
                }
            }
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base.actor.runtimeID);
        }

        private void SpawnSingleMirror(int ix)
        {
            if (base.actor is AvatarActor)
            {
                uint num = Singleton<AvatarManager>.Instance.CreateAvatarMirror((BaseMonoAvatar) base.entity, base.entity.XZPosition, base.entity.transform.forward, this.config.MirrorAIName, base.instancedAbility.Evaluate(this.config.HPRatioOfParent));
                this._mirrorDatas[ix].mirrorRuntimeID = num;
            }
            else if (base.actor is MonsterActor)
            {
                uint num2 = Singleton<MonsterManager>.Instance.CreateMonsterMirror((BaseMonoMonster) base.entity, base.entity.XZPosition, base.entity.transform.forward, this.config.MirrorAIName, base.instancedAbility.Evaluate(this.config.HPRatioOfParent), false);
                this._mirrorDatas[ix].mirrorRuntimeID = num2;
            }
            BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(this._mirrorDatas[ix].mirrorRuntimeID);
            if (this.config.MirrorAbilities != null)
            {
                foreach (string str in this.config.MirrorAbilities)
                {
                    if (string.IsNullOrEmpty(this.config.MirrorAbilitiesOverrideName))
                    {
                        other.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(str));
                    }
                    else
                    {
                        other.abilityPlugin.AddAbility(AbilityData.GetAbilityConfig(str, this.config.MirrorAbilitiesOverrideName));
                    }
                }
            }
            BehaviorDesigner.Runtime.BehaviorTree component = other.entity.GetComponent<BehaviorDesigner.Runtime.BehaviorTree>();
            if (component != null)
            {
                component.SetVariableValue("MirrorCreationIndex", ix);
            }
            base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.MirrorCreateActions, base.instancedAbility, base.instancedModifier, other, null);
        }

        private class MirrorData
        {
            public bool destroyAheadActionTriggered;
            public float mirrorLifetime;
            public uint mirrorRuntimeID;
            public int mirrorSkillCount;

            public void Reset()
            {
                this.mirrorRuntimeID = 0;
                this.mirrorSkillCount = 0;
                this.mirrorLifetime = 0f;
                this.destroyAheadActionTriggered = false;
            }
        }

        private enum State
        {
            Idle,
            SpawningMirrors,
            MirrorActive
        }
    }
}

