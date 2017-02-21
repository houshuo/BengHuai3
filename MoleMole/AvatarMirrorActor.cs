namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class AvatarMirrorActor : AvatarActor
    {
        public BaseMonoAvatar owner;
        public AvatarActor ownerActor;

        public void InitFromAvatarActor(AvatarActor avatarActor, float hpRatioOfParent)
        {
            base.ownerID = avatarActor.runtimeID;
            this.owner = avatarActor.avatar;
            this.ownerActor = avatarActor;
            base.level = this.ownerActor.level;
            base.attack = this.ownerActor.attack;
            base.critical = this.ownerActor.critical;
            base.defense = this.ownerActor.defense;
            base.HP = base.maxHP = hpRatioOfParent * avatarActor.maxHP;
            base.avatarDataItem = avatarActor.avatarDataItem.Clone();
            base._isOnStage = true;
            base.avatar.DisableShadow();
            Physics.IgnoreCollision(this.owner.transform.GetComponent<CapsuleCollider>(), base.avatar.transform.GetComponent<CapsuleCollider>());
            base.avatar.transform.position = this.owner.transform.position;
        }

        public override void Kill(uint killerID, string killerAnimEventID, KillEffect killEffect)
        {
            Singleton<EventManager>.Instance.FireEvent(new EvtKilled(base.runtimeID, killerID, killerAnimEventID), MPEventDispatchMode.Normal);
        }

        public override bool OnEventWithPlugins(BaseEvent evt)
        {
            return ((!(evt is EvtAvatarSwapInEnd) && !(evt is EvtAvatarSwapOutStart)) && base.OnEventWithPlugins(evt));
        }

        public override void PostInit()
        {
            ActorAbilityPlugin.PostInitAbilityActorPlugin(this);
            base.abilityPlugin.onKillBehavior = ActorAbilityPlugin.OnKillBehavior.RemoveAll;
        }
    }
}

