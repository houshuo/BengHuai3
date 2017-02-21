namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class MPActorAbilityPlugin : ActorAbilityPlugin
    {
        protected BaseAbilityEntityIdentiy _abilityIdentity;
        private FlatBufferBuilder _invokeTableBuilder;
        private List<Offset<AbilityInvokeEntry>> _invokeTableOffsets;
        private static MetaArg_AbilityControl _metaAbilityControl = new MetaArg_AbilityControl();
        private static MetaArg_ModifierChange _metaModifierChange = new MetaArg_ModifierChange();
        private static MetaArg_Command_ModifierChangeRequest _metaModifierReq = new MetaArg_Command_ModifierChangeRequest();
        public const int INVOCATION_META_LOCALID = 0xff;

        public MPActorAbilityPlugin(BaseAbilityActor abilityActor) : base(abilityActor)
        {
            this._invokeTableBuilder = new FlatBufferBuilder(0x80);
            this._invokeTableOffsets = new List<Offset<AbilityInvokeEntry>>();
        }

        public void ActTimeSlow_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.TimeSlowHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void ActTimeSlow_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.TimeSlowHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        protected override void AddAppliedAbilities()
        {
            if (this._abilityIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(0, 0, 0, 0xff, out context);
                Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(context.builder, AbilityControlType.AddAppliedAbilities);
                context.Finish<MetaArg_AbilityControl>(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
                base.AddAppliedAbilities();
            }
        }

        protected override ActorModifier AddModifierOnIndex(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig, int index)
        {
            if (this._abilityIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(instancedAbility.instancedAbilityID, index + 1, instancedAbility.caster.runtimeID, 0xff, out context);
                Offset<MetaArg_ModifierChange> offset = MetaArg_ModifierChange.CreateMetaArg_ModifierChange(context.builder, ModifierAction.Added, (byte) modifierConfig.localID);
                context.Finish<MetaArg_ModifierChange>(offset, AbilityInvokeArgument.MetaArg_ModifierChange);
                return base.AddModifierOnIndex(instancedAbility, modifierConfig, index);
            }
            return null;
        }

        public void ApplyLevelBuff_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            LevelBuffSide overrideCurSide;
            ApplyLevelBuff config = (ApplyLevelBuff) actionConfig;
            float duration = base._Internal_CalculateApplyLevelBuffDuration(config, instancedAbility, evt);
            uint runtimeID = instancedAbility.caster.runtimeID;
            if (config.UseOverrideCurSide)
            {
                overrideCurSide = config.OverrideCurSide;
            }
            else
            {
                overrideCurSide = base.CalculateLevelBuffSide(instancedAbility.caster.runtimeID);
            }
            MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Level_RequestLevelBuff>();
            Packet_Level_RequestLevelBuff.StartPacket_Level_RequestLevelBuff(pc.builder);
            Packet_Level_RequestLevelBuff.AddLevelBuffType(pc.builder, (byte) config.LevelBuff);
            Packet_Level_RequestLevelBuff.AddEnteringSlow(pc.builder, config.EnteringTimeSlow);
            Packet_Level_RequestLevelBuff.AddAllowRefresh(pc.builder, config.LevelBuffAllowRefresh);
            Packet_Level_RequestLevelBuff.AddSide(pc.builder, (byte) overrideCurSide);
            Packet_Level_RequestLevelBuff.AddOwnerRuntimeID(pc.builder, runtimeID);
            Packet_Level_RequestLevelBuff.AddNotStartEffect(pc.builder, config.NotStartEffect);
            Packet_Level_RequestLevelBuff.AddDuration(pc.builder, duration);
            Packet_Level_RequestLevelBuff.AddInstancedAbilityID(pc.builder, (byte) instancedAbility.instancedAbilityID);
            Packet_Level_RequestLevelBuff.AddActionLocalID(pc.builder, (byte) config.localID);
            pc.Finish<Packet_Level_RequestLevelBuff>(Packet_Level_RequestLevelBuff.EndPacket_Level_RequestLevelBuff(pc.builder));
            Singleton<MPManager>.Instance.SendReliableToPeer(0x21800001, 1, pc);
            Singleton<MPLevelManager>.Instance.levelActor.GetPlugin<MPLevelAbilityHelperPlugin>().AttachPendingModifiersToNextLevelBuff(config, base._owner.runtimeID, instancedAbility.instancedAbilityID, (target != null) ? target.runtimeID : 0);
        }

        public void ApplyLevelBuff_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
        }

        protected override ActorModifier ApplyModifier(ActorAbility instancedAbility, ConfigAbilityModifier modifierConfig)
        {
            return base.ApplyModifier(instancedAbility, modifierConfig);
        }

        public void ApplyModifier_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            MoleMole.Config.ApplyModifier modifier = (MoleMole.Config.ApplyModifier) actionConfig;
            if (target == base._owner)
            {
                base._owner.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
            }
            else if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
            {
                target.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
            }
            else
            {
                context.Finish(true);
            }
        }

        public void ApplyModifier_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            MoleMole.Config.ApplyModifier modifier = (MoleMole.Config.ApplyModifier) actionConfig;
            if ((target != base._owner) && Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
            {
                target.abilityPlugin.ApplyModifier(instancedAbility, modifier.ModifierName);
            }
        }

        public void AttachEffect_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.AttachEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void AttachEffect_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.AttachEffectHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        public void AvatarSkillStart_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.AvatarSkillStartHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void AvatarSKillStart_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.AvatarSkillStartHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        public override BaseAbilityMixin CreateInstancedAbilityMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config)
        {
            BaseAbilityMixin mixin = config.MPCreateInstancedMixin(instancedAbility, instancedModifier);
            if (mixin == null)
            {
                return null;
            }
            mixin.selfIdentity = Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(mixin.actor.runtimeID);
            return mixin;
        }

        public void DamageByAttackValue_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.DamageByAttackValueHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        public void ExternalResolveTarget(AbilityTargetting targetting, TargettingOption option, ActorAbility instancedAbility, BaseAbilityActor other, out BaseAbilityActor outTarget, out BaseAbilityActor[] outTargetLs, out bool needHandleTargetOnNull)
        {
            base.ResolveTarget(targetting, option, instancedAbility, other, out outTarget, out outTargetLs, out needHandleTargetOnNull);
        }

        public void FireEffect_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.FireEffectHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void FireEffect_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.FireEffectHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        private void FlushRecordInvokeEntriesAndSend()
        {
            if (this._invokeTableOffsets.Count > 0)
            {
                MPSendPacketContainer pc = Singleton<MPManager>.Instance.CreateSendPacket(typeof(Packet_Ability_InvocationTable), this._invokeTableBuilder);
                VectorOffset invokesOffset = Packet_Ability_InvocationTable.CreateInvokesVector(pc.builder, this._invokeTableOffsets.ToArray());
                Offset<Packet_Ability_InvocationTable> offset = Packet_Ability_InvocationTable.CreatePacket_Ability_InvocationTable(pc.builder, invokesOffset);
                pc.Finish<Packet_Ability_InvocationTable>(offset);
                if (this._abilityIdentity.isAuthority)
                {
                    Singleton<MPManager>.Instance.SendReliableToOthers(base._owner.runtimeID, pc);
                }
                else
                {
                    Singleton<MPManager>.Instance.SendReliableToPeer(base._owner.runtimeID, this._abilityIdentity.GetPeerID(), pc);
                }
                this._invokeTableBuilder.Clear();
                this._invokeTableOffsets.Clear();
            }
        }

        public ActorAbility GetInstancedAbilityByID(int appliedAbilityID)
        {
            return base._appliedAbilities[appliedAbilityID - 1];
        }

        public ActorModifier GetInstancedModifierByID(int appliedModifierID)
        {
            return base._appliedModifiers[appliedModifierID - 1];
        }

        protected override void HandleAction(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt)
        {
            if (this._abilityIdentity.isAuthority && base.EvaluateAbilityPredicate(actionConfig.Predicates, instancedAbility, instancedModifier, target, evt))
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(instancedAbility.instancedAbilityID, (instancedModifier == null) ? 0 : instancedModifier.instancedModifierID, (target != null) ? target.runtimeID : base._owner.runtimeID, actionConfig.localID, out context);
                actionConfig.MPGetAuthorityHandler(this)(actionConfig, instancedAbility, instancedModifier, target, evt, ref context);
            }
        }

        protected override void HandleActionTargetDispatch(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor other, BaseEvent evt, Func<BaseAbilityActor, bool> targetPredicate)
        {
            if (this._abilityIdentity.isAuthority)
            {
                base.HandleActionTargetDispatch(actionConfig, instancedAbility, instancedModifier, other, evt, targetPredicate);
            }
        }

        public void HandleInvokes(Packet_Ability_InvocationTable table, int fromPeerID)
        {
            int invokesLength = table.InvokesLength;
            for (int i = 0; i < invokesLength; i++)
            {
                AbilityInvokeEntry invokes = table.GetInvokes(i);
                if (invokes.LocalID == 0xff)
                {
                    if (this._abilityIdentity.isAuthority)
                    {
                        this.MetaAuthorityInvokeHandler(invokes);
                    }
                    else
                    {
                        this.MetaRemoteInvokeHandler(invokes);
                    }
                }
                else
                {
                    ActorAbility parentAbility;
                    ActorModifier instancedModifierByID;
                    uint target = invokes.Target;
                    BaseAbilityActor actor = (target != 0) ? Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(target) : base._owner;
                    if (actor == null)
                    {
                        return;
                    }
                    int instancedModifierID = invokes.InstancedModifierID;
                    if (instancedModifierID != 0)
                    {
                        instancedModifierByID = this.GetInstancedModifierByID(instancedModifierID);
                        parentAbility = instancedModifierByID.parentAbility;
                    }
                    else
                    {
                        instancedModifierByID = null;
                        parentAbility = this.GetInstancedAbilityByID(invokes.InstancedAbilityID);
                    }
                    BaseActionContainer container = parentAbility.config.InvokeSites[invokes.LocalID];
                    if (container is ConfigAbilityAction)
                    {
                        ConfigAbilityAction actionConfig = (ConfigAbilityAction) container;
                        actionConfig.MPGetRemoteHandler(this)(actionConfig, parentAbility, instancedModifierByID, actor, invokes);
                    }
                    else
                    {
                        BaseAbilityMixin instancedMixin = null;
                        if (instancedModifierByID != null)
                        {
                            instancedMixin = instancedModifierByID.GetInstancedMixin(invokes.LocalID);
                        }
                        if (instancedMixin == null)
                        {
                            instancedMixin = parentAbility.GetInstancedMixin(invokes.LocalID);
                        }
                        instancedMixin.HandleMixinInvokeEntry(invokes, fromPeerID);
                    }
                }
            }
        }

        public void HealSP_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            this.HealSP_Common((HealSP) actionConfig, instancedAbility, target);
            context.Finish(true);
        }

        private void HealSP_Common(HealSP config, ActorAbility instancedAbility, BaseAbilityActor target)
        {
            float amount = instancedAbility.Evaluate(config.Amount);
            if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
            {
                target.HealSP(amount);
            }
            if (((target.isAlive != 0) && !config.MuteHealEffect) && (amount > 0f))
            {
                target.entity.FireEffect("Ability_HealSP");
            }
        }

        public void HealSP_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            this.HealSP_Common((HealSP) actionConfig, instancedAbility, target);
        }

        private void MetaAuthorityCommand_ModifierChangeRequestHandler(AbilityInvokeEntry invokeEntry)
        {
            _metaModifierReq = invokeEntry.GetArgument<MetaArg_Command_ModifierChangeRequest>(_metaModifierReq);
            uint target = invokeEntry.Target;
            uint casterID = (target != 0) ? target : base._owner.runtimeID;
            if (((int) _metaModifierReq.Action) == 0)
            {
                this.MPTryApplyModifierByID(casterID, invokeEntry.InstancedAbilityID, _metaModifierReq.ModifierLocalID);
            }
            else if (((int) _metaModifierReq.Action) == 1)
            {
                this.MPTryRemoveModifierByID(casterID, invokeEntry.InstancedAbilityID, _metaModifierReq.ModifierLocalID);
            }
        }

        private void MetaAuthorityInvokeHandler(AbilityInvokeEntry invokeEntry)
        {
            if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_Command_ModifierChangeRequest)
            {
                this.MetaAuthorityCommand_ModifierChangeRequestHandler(invokeEntry);
            }
        }

        private void MetaHandleModifierChange(AbilityInvokeEntry table)
        {
            _metaModifierChange = table.GetArgument<MetaArg_ModifierChange>(_metaModifierChange);
            if (((int) _metaModifierChange.Action) == 0)
            {
                ActorAbility instancedAbility = null;
                ConfigAbilityModifier modifierConfig = null;
                int index = table.InstancedModifierID - 1;
                BaseAbilityActor actor = (table.Target != 0) ? Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(table.Target) : base._owner;
                instancedAbility = ((MPActorAbilityPlugin) actor.abilityPlugin).GetInstancedAbilityByID(table.InstancedAbilityID);
                modifierConfig = instancedAbility.config.ModifierIDMap[_metaModifierChange.ModifierLocalID];
                base.AddModifierOnIndex(instancedAbility, modifierConfig, index);
            }
            else
            {
                ActorModifier instancedModifierByID = this.GetInstancedModifierByID(table.InstancedModifierID);
                int num2 = table.InstancedModifierID - 1;
                base.RemoveModifierOnIndex(instancedModifierByID, num2);
            }
        }

        private void MetaHandlerAbilityControl(AbilityInvokeEntry invokeEntry)
        {
            _metaAbilityControl = invokeEntry.GetArgument<MetaArg_AbilityControl>(_metaAbilityControl);
            switch (_metaAbilityControl.Type)
            {
                case AbilityControlType.AddAppliedAbilities:
                    base.AddAppliedAbilities();
                    break;

                case AbilityControlType.RemoveAllAbilities:
                    base.RemoveAllAbilities();
                    break;

                case AbilityControlType.RemoveAllNonDestroyAbilities:
                    base.RemoveAllNonOnDestroyAbilities();
                    break;
            }
        }

        private void MetaRemoteInvokeHandler(AbilityInvokeEntry invokeEntry)
        {
            if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_ModifierChange)
            {
                this.MetaHandleModifierChange(invokeEntry);
            }
            else if (invokeEntry.ArgumentType == AbilityInvokeArgument.MetaArg_AbilityControl)
            {
                this.MetaHandlerAbilityControl(invokeEntry);
            }
        }

        public bool MPTryApplyModifierByID(uint casterID, int instancedAbilityID, int modifierLocalID)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(casterID);
            if (actor == null)
            {
                return false;
            }
            ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(instancedAbilityID);
            if (instancedAbilityByID == null)
            {
                return false;
            }
            ConfigAbilityModifier modifierConfig = instancedAbilityByID.config.ModifierIDMap[modifierLocalID];
            return (this.ApplyModifier(instancedAbilityByID, modifierConfig) != null);
        }

        public bool MPTryRemoveModifierByID(uint casterID, int instancedAbilityID, int modifierLocalID)
        {
            for (int i = 0; i < base._appliedModifiers.Count; i++)
            {
                if (base._appliedModifiers[i] != null)
                {
                    ActorModifier modifier = base._appliedModifiers[i];
                    if (((modifier.parentAbility.caster.runtimeID == casterID) && (modifier.parentAbility.instancedAbilityID == instancedAbilityID)) && (modifier.config.localID == modifierLocalID))
                    {
                        this.RemoveModifier(modifier, i);
                        return true;
                    }
                }
            }
            return false;
        }

        public override void OnAdded()
        {
            MPManager instance = Singleton<MPManager>.Instance;
            instance.OnFrameEnd = (Action) Delegate.Combine(instance.OnFrameEnd, new Action(this.FlushRecordInvokeEntriesAndSend));
        }

        public override void OnRemoved()
        {
            MPManager instance = Singleton<MPManager>.Instance;
            instance.OnFrameEnd = (Action) Delegate.Remove(instance.OnFrameEnd, new Action(this.FlushRecordInvokeEntriesAndSend));
            base.OnRemoved();
        }

        public void Predicated_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.PredicatedHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
        }

        protected override void RemoveAllAbilities()
        {
            if (this._abilityIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(0, 0, 0, 0xff, out context);
                Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(context.builder, AbilityControlType.RemoveAllAbilities);
                context.Finish<MetaArg_AbilityControl>(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
                base.RemoveAllAbilities();
            }
        }

        protected override void RemoveAllModifies()
        {
            if (this._abilityIdentity.isAuthority)
            {
                base.RemoveAllModifies();
            }
        }

        protected override void RemoveAllNonOnDestroyAbilities()
        {
            if (this._abilityIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(0, 0, 0, 0xff, out context);
                Offset<MetaArg_AbilityControl> offset = MetaArg_AbilityControl.CreateMetaArg_AbilityControl(context.builder, AbilityControlType.RemoveAllNonDestroyAbilities);
                context.Finish<MetaArg_AbilityControl>(offset, AbilityInvokeArgument.MetaArg_AbilityControl);
                base.RemoveAllNonOnDestroyAbilities();
            }
        }

        protected override void RemoveModifier(ActorModifier modifier, int index)
        {
            base.RemoveModifier(modifier, index);
        }

        public void RemoveModifier_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            MoleMole.Config.RemoveModifier modifier = (MoleMole.Config.RemoveModifier) actionConfig;
            if (target == base._owner)
            {
                base._owner.abilityPlugin.TryRemoveModifier(instancedAbility, modifier.ModifierName);
            }
            else if (Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
            {
                target.abilityPlugin.TryRemoveModifier(instancedAbility, modifier.ModifierName);
            }
            else
            {
                context.Finish(true);
            }
        }

        public void RemoveModifier_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            MoleMole.Config.RemoveModifier modifier = (MoleMole.Config.RemoveModifier) actionConfig;
            if ((target != base._owner) && Singleton<MPEventManager>.Instance.IsIdentityAuthority(target.runtimeID))
            {
                target.abilityPlugin.TryRemoveModifier(instancedAbility, modifier.ModifierName);
            }
        }

        protected override void RemoveModifierOnIndex(ActorModifier modifier, int index)
        {
            if (this._abilityIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                this.StartRecordInvokeEntry(0, modifier.instancedModifierID, 0, 0xff, out context);
                Offset<MetaArg_ModifierChange> offset = MetaArg_ModifierChange.CreateMetaArg_ModifierChange(context.builder, ModifierAction.Removed, (byte) modifier.config.localID);
                context.Finish<MetaArg_ModifierChange>(offset, AbilityInvokeArgument.MetaArg_ModifierChange);
                base.RemoveModifierOnIndex(modifier, index);
            }
        }

        public void SetAnimatorBool_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.SetAnimatorBoolHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void SetAnimatorBool_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.SetAnimatorBoolHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        public void SetupIdentity(BaseAbilityEntityIdentiy identity)
        {
            this._abilityIdentity = identity;
            base.OnAdded();
        }

        public void StartRecordInvokeEntry(int instancedAbilityID, int instancedModifierID, uint targetRuntimeID, int localID, out RecordInvokeEntryContext entry)
        {
            entry = new RecordInvokeEntryContext();
            entry.instancedAbilityID = (byte) instancedAbilityID;
            entry.instanceModifierID = (byte) instancedModifierID;
            entry.targetID = (targetRuntimeID != base._owner.runtimeID) ? targetRuntimeID : 0;
            entry.localID = (byte) localID;
            entry.builder = this._invokeTableBuilder;
            entry.outOffsetLs = this._invokeTableOffsets;
        }

        public static void STUB_AuthorityMute(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            context.Finish(false);
        }

        public static void STUB_RemoteMute(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, Table argument)
        {
        }

        public void TriggerAbility_AuthorityHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context)
        {
            base.TriggerAbilityHandler(actionConfig, instancedAbility, instancedModifier, target, evt);
            context.Finish(true);
        }

        public void TriggerAbility_RemoteHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry)
        {
            base.TriggerAbilityHandler(actionConfig, instancedAbility, instancedModifier, target, null);
        }

        public delegate void MPAuthorityActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, BaseEvent evt, ref RecordInvokeEntryContext context);

        public delegate void MPRemoteActionHandler(ConfigAbilityAction actionConfig, ActorAbility instancedAbility, ActorModifier instancedModifier, BaseAbilityActor target, AbilityInvokeEntry invokeEntry);
    }
}

