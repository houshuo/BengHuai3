namespace MoleMole
{
    using FlatBuffers;
    using MoleMole.MPProtocol;
    using System;
    using UniRx;
    using UnityEngine;

    public class MPMonsterActorPlugin : BaseMPAbilityActorPlugin
    {
        protected MonsterActor _actor;
        protected MonsterIdentity _identity;

        public MPMonsterActorPlugin(BaseActor actor)
        {
            this._actor = (MonsterActor) actor;
        }

        public override void OnAdded()
        {
            this._actor.appliedAbilities.Add(Tuple.Create<ConfigAbility, Dictionary<string, object>>(AbilityData.GetAbilityConfig("MPTest_HitToBleedAlt"), AbilityData.EMPTY_OVERRIDE_MAP));
            if (Singleton<MPManager>.Instance.isMaster)
            {
                this._actor.onPostInitialized = (Action) Delegate.Combine(this._actor.onPostInitialized, new Action(this.OnInitializeDoneReplicate));
            }
        }

        private void OnInitializeDoneReplicate()
        {
            MPSendPacketContainer initPc = Singleton<MPManager>.Instance.CreateSendPacket<Packet_Monster_MonsterCreation>();
            StringOffset monsterNameOffset = initPc.builder.CreateString(this._actor.monster.MonsterName);
            StringOffset monsterTypeOffset = initPc.builder.CreateString(this._actor.monster.TypeName);
            Packet_Monster_MonsterCreation.StartPacket_Monster_MonsterCreation(initPc.builder);
            Packet_Monster_MonsterCreation.AddMonsterName(initPc.builder, monsterNameOffset);
            Packet_Monster_MonsterCreation.AddMonsterType(initPc.builder, monsterTypeOffset);
            Packet_Monster_MonsterCreation.AddLevel(initPc.builder, (int) this._actor.level);
            Packet_Monster_MonsterCreation.AddIsElite(initPc.builder, this._actor.isElite);
            Packet_Monster_MonsterCreation.AddUniqueMonsterID(initPc.builder, this._actor.uniqueMonsterID);
            Vector3 xZPosition = this._actor.monster.XZPosition;
            Packet_Monster_MonsterCreation.AddInitPos(initPc.builder, MPVector2_XZ.CreateMPVector2_XZ(initPc.builder, xZPosition.x, xZPosition.z));
            initPc.Finish<Packet_Monster_MonsterCreation>(Packet_Monster_MonsterCreation.EndPacket_Monster_MonsterCreation(initPc.builder));
            Singleton<MPManager>.Instance.InstantiateMPIdentity<MonsterIdentity>(this._actor.runtimeID, initPc);
        }

        private bool OnRemoteBeingHit(EvtBeingHit evt)
        {
            if (evt.attackData.rejected)
            {
                return false;
            }
            if (evt.attackData.hitCollision == null)
            {
                this._actor.AmendHitCollision(evt.attackData);
            }
            evt.attackData.resolveStep = AttackData.AttackDataStep.FinalResolved;
            float newValue = this._actor.HP - evt.resolvedDamage;
            if (newValue <= 0f)
            {
                newValue = 0f;
            }
            DelegateUtils.UpdateField(ref this._actor.HP, newValue, newValue - this._actor.HP, this._actor.onHPChanged);
            this._actor.FireAttackDataEffects(evt.attackData);
            this._actor.AbilityBeingHit(evt);
            this._actor.BeingHit(evt.attackData, evt.beHitEffect);
            return true;
        }

        protected override bool OnRemoteReplicatedEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnRemoteBeingHit((EvtBeingHit) evt));
        }

        public void SetupIdentity(MonsterIdentity identity)
        {
            this._identity = identity;
            base.Setup(this._actor, identity);
        }
    }
}

