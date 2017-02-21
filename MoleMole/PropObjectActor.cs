namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    public class PropObjectActor : BaseAbilityActor
    {
        private float _opacity;
        private ActorTriggerFieldPlugin _triggerFieldPlugin;
        public ConfigPropObject config;
        public List<LDDropDataItem> dropDataItems;
        public const float GOODS_DROP_MAX_DISTANCE = 1f;
        public bool needDropReward = true;
        private BaseMonoPropObject prop;

        private bool AlmostEqualOrBigger(float a, float b)
        {
            return ((Mathf.Abs(a) + 0.001f) > Mathf.Abs(b));
        }

        public virtual void BeingHit(AttackResult attackResult, BeHitEffect beHitEffect, uint sourceID)
        {
            this.prop.BeHit(attackResult.frameHalt, attackResult.hitEffect, attackResult.hitEffectAux, attackResult.killEffect, beHitEffect, attackResult.aniDamageRatio, attackResult.hitCollision.hitDir, attackResult.retreatVelocity, sourceID);
        }

        private AttackResult.HitCollsion CalcHitCollision(ConfigPropObject.E_RetreatType retreatType, BaseMonoEntity victimEntity)
        {
            AttackResult.HitCollsion collsion = new AttackResult.HitCollsion();
            if (retreatType == ConfigPropObject.E_RetreatType.Spike)
            {
                RaycastHit hit;
                collsion.hitPoint = victimEntity.GetAttachPoint("RootNode").position;
                Vector3 origin = victimEntity.transform.position + ((Vector3) (Vector3.up * 0.5f));
                Vector3 direction = this.prop.transform.position - origin;
                if (Physics.Raycast(origin, direction, out hit, 10f, ((int) 1) << InLevelData.PROP_LAYER))
                {
                    collsion.hitDir = hit.normal;
                }
                return collsion;
            }
            collsion.hitPoint = base.entity.GetAttachPoint("RootNode").position;
            collsion.hitDir = base.entity.XZPosition - this.prop.XZPosition;
            return collsion;
        }

        public override void ForceKill(uint killerID, KillEffect killEffect)
        {
            base.isAlive = 0;
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID), MPEventDispatchMode.Normal);
            this.prop.SetDied(KillEffect.KillNow);
        }

        private Vector3 GetDropPosition()
        {
            return new Vector3(this.prop.XZPosition.x, 1.5f, this.prop.XZPosition.z);
        }

        public float GetPropObjectCurrentOpacity()
        {
            MeshRenderer componentInChildren = base.gameObject.GetComponentInChildren<MeshRenderer>();
            if (componentInChildren != null)
            {
                return componentInChildren.sharedMaterial.GetFloat("_Opaqueness");
            }
            return 1f;
        }

        public override void Init(BaseMonoEntity entity)
        {
            this.prop = (BaseMonoPropObject) entity;
            this.config = this.prop.config;
            base.commonConfig = this.config.CommonConfig;
            base.Init(entity);
            for (int i = 0; i < this.config.Abilities.Length; i++)
            {
                ConfigEntityAbilityEntry entry = this.config.Abilities[i];
                base.appliedAbilities.Add(Tuple.Create<ConfigAbility, Dictionary<string, object>>(AbilityData.GetAbilityConfig(entry.AbilityName, entry.AbilityOverride), new Dictionary<string, object>()));
            }
            if (this.config.PropArguments.IsTriggerField)
            {
                this._triggerFieldPlugin = new ActorTriggerFieldPlugin(this);
                base.AddPlugin(this._triggerFieldPlugin);
            }
            for (int j = 0; j < 0x1b; j++)
            {
                AbilityState state = (AbilityState) (((int) 1) << j);
                if (((state & (AbilityState.Undamagable | AbilityState.MaxMoveSpeed | AbilityState.Immune | AbilityState.CritUp | AbilityState.Shielded | AbilityState.PowerUp | AbilityState.AttackSpeedUp | AbilityState.MoveSpeedUp | AbilityState.Endure)) != AbilityState.None) || ((state & (AbilityState.Tied | AbilityState.TargetLocked | AbilityState.Fragile | AbilityState.Weak | AbilityState.AttackSpeedDown | AbilityState.MoveSpeedDown | AbilityState.Frozen | AbilityState.Poisoned | AbilityState.Burn | AbilityState.Paralyze | AbilityState.Stun | AbilityState.Bleed)) != AbilityState.None))
                {
                    base.SetAbilityStateImmune(state, true);
                }
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtLevelBuffState>(base.runtimeID);
            this._opacity = 1f;
        }

        public void InitProp(float HP, float attack)
        {
            base.baseMaxHP = base.maxHP = base.HP = HP;
            if (!this.config.PropArguments.UseOwnerAttack)
            {
                base.attack = attack;
            }
            else
            {
                BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(base.ownerID);
                if (actor == null)
                {
                    this.ForceKill(0x21800001, KillEffect.KillImmediately);
                }
                else
                {
                    base.attack = actor.attack;
                }
            }
        }

        protected virtual void Kill(uint killerID, string killerAnimEventID)
        {
            base.isAlive = 0;
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID, killerID, killerAnimEventID), MPEventDispatchMode.Normal);
            this.prop.SetDied(KillEffect.KillNow);
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            bool flag = base.ListenEvent(evt);
            if (evt is EvtLevelBuffState)
            {
                return this.OnLevelBuffState((EvtLevelBuffState) evt);
            }
            return flag;
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            DamageModelLogic.ResolveAttackDataByAttackee(this, evt.attackData);
            return true;
        }

        private bool OnBeingHitResolve(EvtBeingHit evt)
        {
            evt.Resolve();
            AttackResult attackResult = DamageModelLogic.ResolveAttackDataFinal(this, evt.attackData);
            if (attackResult.hitCollision == null)
            {
                AttackResult.HitCollsion collsion = new AttackResult.HitCollsion {
                    hitPoint = this.prop.RootNode.position,
                    hitDir = -this.prop.transform.forward
                };
                attackResult.hitCollision = collsion;
            }
            if (!evt.attackData.isAnimEventAttack)
            {
                return false;
            }
            if (base.isAlive != 0)
            {
                float totalDamage = attackResult.GetTotalDamage();
                float newValue = base.HP - totalDamage;
                if (newValue <= 0f)
                {
                    newValue = 0f;
                }
                DelegateUtils.UpdateField(ref this.HP, newValue, newValue - base.HP, base.onHPChanged);
                if (base.HP == 0f)
                {
                    if (base.abilityState.ContainsState(AbilityState.Limbo))
                    {
                        this.BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
                    }
                    else
                    {
                        this.BeingHit(attackResult, BeHitEffect.KillingBeHit, evt.sourceID);
                        this.Kill(evt.sourceID, evt.animEventID);
                    }
                }
                else
                {
                    this.BeingHit(attackResult, BeHitEffect.NormalBeHit, evt.sourceID);
                }
            }
            if ((attackResult.attackEffectPattern != null) && ((attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal) || (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyAttack)))
            {
                AttackPattern.ActAttackEffects(attackResult.attackEffectPattern, this.prop, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
            }
            if ((attackResult.beHitEffectPattern != null) && ((attackResult.hitEffectPattern == AttackResult.HitEffectPattern.Normal) || (attackResult.hitEffectPattern == AttackResult.HitEffectPattern.OnlyBeHit)))
            {
                AttackPattern.ActAttackEffects(attackResult.beHitEffectPattern, this.prop, attackResult.hitCollision.hitPoint, attackResult.hitCollision.hitDir);
            }
            if (evt.attackData.isAnimEventAttack)
            {
                EvtAttackLanded landed = new EvtAttackLanded(evt.sourceID, base.runtimeID, evt.animEventID, attackResult);
                Singleton<EventManager>.Instance.FireEvent(landed, MPEventDispatchMode.Normal);
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackLanded, landed));
            }
            else
            {
                Singleton<EventManager>.Instance.FireEvent(new EvtDamageLanded(evt.sourceID, base.runtimeID, attackResult), MPEventDispatchMode.Normal);
            }
            return true;
        }

        public override bool OnEventResolves(BaseEvent evt)
        {
            bool flag = base.OnEventResolves(evt);
            if (evt is EvtFieldEnter)
            {
                return this.OnFieldEnter((EvtFieldEnter) evt);
            }
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHitResolve((EvtBeingHit) evt);
            }
            if (evt is EvtHittingOther)
            {
                return this.OnHittingOtherResolve((EvtHittingOther) evt);
            }
            return flag;
        }

        public override bool OnEventWithPlugins(BaseEvent evt)
        {
            bool flag = base.OnEventWithPlugins(evt);
            if (evt is EvtBeingHit)
            {
                return this.OnBeingHit((EvtBeingHit) evt);
            }
            if (evt is EvtHittingOther)
            {
                return this.OnHittingOther((EvtHittingOther) evt);
            }
            if (evt is EvtFieldHit)
            {
                return this.OnFieldHit((EvtFieldHit) evt);
            }
            if (evt is EvtPropObjectForceKilled)
            {
                return this.OnForceKilled((EvtPropObjectForceKilled) evt);
            }
            return flag;
        }

        private bool OnFieldEnter(EvtFieldEnter evt)
        {
            if (this.config.PropArguments.TriggerHitWhenFieldEnter)
            {
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(evt.otherID);
                if (entity != null)
                {
                    Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(base.runtimeID, entity.GetRuntimeID(), this.config.PropArguments.AnimEventIDForHit), MPEventDispatchMode.Normal);
                }
            }
            if (this.config.PropArguments.DieWhenFieldEnter)
            {
                this.Kill(Singleton<LevelManager>.Instance.levelActor.runtimeID, string.Empty);
            }
            return true;
        }

        private bool OnFieldHit(EvtFieldHit evt)
        {
            for (int i = 0; i < this._triggerFieldPlugin.insideIDs.Count; i++)
            {
                uint runtimeID = this._triggerFieldPlugin.insideIDs[i];
                BaseMonoEntity entity = Singleton<EventManager>.Instance.GetEntity(runtimeID);
                if (entity != null)
                {
                    if (!entity.IsActive())
                    {
                        this._triggerFieldPlugin.insideIDs.Remove(runtimeID);
                    }
                    else
                    {
                        Singleton<EventManager>.Instance.FireEvent(new EvtHittingOther(base.runtimeID, this._triggerFieldPlugin.insideIDs[i], evt.animEventID), MPEventDispatchMode.Normal);
                    }
                }
            }
            return true;
        }

        private bool OnForceKilled(EvtPropObjectForceKilled evt)
        {
            this.ForceKill(Singleton<LevelManager>.Instance.levelActor.runtimeID, KillEffect.KillNow);
            return true;
        }

        private bool OnHittingOther(EvtHittingOther evt)
        {
            if (evt.attackData == null)
            {
                evt.attackData = DamageModelLogic.CreateAttackDataFromAttackerAnimEvent(this, evt.animEventID);
            }
            if ((evt.hitCollision == null) && (Singleton<EventManager>.Instance.GetActor(evt.toID) != null))
            {
                BaseMonoEntity victimEntity = Singleton<EventManager>.Instance.GetEntity(evt.toID);
                evt.hitCollision = this.CalcHitCollision(this.config.PropArguments.RetreatType, victimEntity);
            }
            evt.attackData.hitCollision = evt.hitCollision;
            return true;
        }

        private bool OnHittingOtherResolve(EvtHittingOther evt)
        {
            evt.Resolve();
            Singleton<EventManager>.Instance.FireEvent(new EvtBeingHit(evt.toID, base.runtimeID, evt.animEventID, evt.attackData), MPEventDispatchMode.Normal);
            return true;
        }

        private bool OnLevelBuffState(EvtLevelBuffState evt)
        {
            if (evt.levelBuff == LevelBuffType.WitchTime)
            {
                if (evt.state == LevelBuffState.Start)
                {
                    return this.OnWitchTimeStart();
                }
                if (evt.state == LevelBuffState.Stop)
                {
                    return this.OnWitchTimeStop();
                }
            }
            return false;
        }

        public override void OnRemoval()
        {
            base.OnRemoval();
            if (((Singleton<LevelManager>.Instance != null) && (Singleton<LevelManager>.Instance.levelActor.levelState == LevelActor.LevelState.LevelRunning)) && ((this.dropDataItems != null) && this.needDropReward))
            {
                if (this.dropDataItems.Count == 1)
                {
                    this.dropDataItems[0].CreateDropGoods(this.GetDropPosition(), Vector3.forward, true);
                }
                else if (this.dropDataItems.Count > 1)
                {
                    foreach (LDDropDataItem item in this.dropDataItems)
                    {
                        item.CreateDropGoods(this.GetDropPosition(), Vector3.forward, true);
                    }
                }
            }
        }

        private bool OnWitchTimeStart()
        {
            return true;
        }

        private bool OnWitchTimeStop()
        {
            return true;
        }

        public override void PostInit()
        {
            base.PostInit();
            base.abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.DoNotRemoveUntilDestroyed;
        }

        public void SetPorpObjectOpacity(float opacity)
        {
            List<MeshRenderer> list = new List<MeshRenderer>(base.gameObject.GetComponentsInChildren<MeshRenderer>());
            List<SkinnedMeshRenderer> list2 = new List<SkinnedMeshRenderer>(base.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>());
            if (list != null)
            {
                foreach (MeshRenderer renderer in list)
                {
                    renderer.material.SetFloat("_Opaqueness", opacity);
                }
            }
            if (list2 != null)
            {
                foreach (SkinnedMeshRenderer renderer2 in list2)
                {
                    renderer2.material.SetFloat("_Opaqueness", opacity);
                }
            }
        }

        public float Opacity
        {
            get
            {
                return this._opacity;
            }
            set
            {
                this.SetPorpObjectOpacity(value);
                this._opacity = value;
            }
        }
    }
}

