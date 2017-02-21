namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.Config;
    using MoleMole.MPProtocol;
    using System;

    public class MPAbilityHitExplodeBulletMixin_Old : AbilityHitExplodeBulletMixin
    {
        private MixinArg_HitExplodeMixin _mixinArg;

        public MPAbilityHitExplodeBulletMixin_Old(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this._mixinArg = new MixinArg_HitExplodeMixin();
        }

        public override void HandleMixinInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
        {
            if (!base.selfIdentity.isAuthority)
            {
                this.RemoteHandleInvokeEntry(invokeEntry, fromPeerID);
            }
        }

        protected override bool ListenBulletHit(EvtBulletHit evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                Singleton<MPEventManager>.Instance.MarkEventReplicate(evt);
                return base.ListenBulletHit(evt);
            }
            return false;
        }

        public override void OnAbilityTriggered(EvtAbilityStart evt)
        {
            if (base.selfIdentity.isAuthority)
            {
                RecordInvokeEntryContext context;
                HitExplodeTracingBulletMixinArgument abilityArgument = evt.abilityArgument as HitExplodeTracingBulletMixinArgument;
                uint nextRuntimeID = Singleton<RuntimeIDManager>.Instance.GetNextRuntimeID(6);
                base.StartRecordMixinInvokeEntry(out context, 0);
                Offset<MixinArg_HitExplodeMixin> offset = MixinArg_HitExplodeMixin.CreateMixinArg_HitExplodeMixin(context.builder, HitExplodeBulletAction.Trigger, (abilityArgument == null) ? ((ushort) 0) : ((ushort) IndexedConfig<HitExplodeTracingBulletMixinArgument>.Mapping.Get(abilityArgument)), nextRuntimeID, evt.otherID);
                context.Finish<MixinArg_HitExplodeMixin>(offset, AbilityInvokeArgument.MixinArg_HitExplodeMixin);
                this.CreateBullet(abilityArgument, nextRuntimeID, evt.otherID);
            }
        }

        private void RemoteHandleInvokeEntry(AbilityInvokeEntry invokeEntry, int fromPeerID)
        {
            this._mixinArg = invokeEntry.GetArgument<MixinArg_HitExplodeMixin>(this._mixinArg);
            if (this._mixinArg.Action == HitExplodeBulletAction.Trigger)
            {
                HitExplodeTracingBulletMixinArgument arg = null;
                if (this._mixinArg.ArgMappingID != 0)
                {
                    arg = IndexedConfig<HitExplodeTracingBulletMixinArgument>.Mapping.Get(this._mixinArg.ArgMappingID);
                }
                this.CreateBullet(arg, this._mixinArg.BulletID, this._mixinArg.OtherID);
            }
        }
    }
}

