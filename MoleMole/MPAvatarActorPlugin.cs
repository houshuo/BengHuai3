namespace MoleMole
{
    using System;

    public class MPAvatarActorPlugin : BaseMPAbilityActorPlugin
    {
        protected AvatarActor _actor;
        protected AvatarIdentity _identity;

        public MPAvatarActorPlugin(BaseActor actor)
        {
            this._actor = (AvatarActor) actor;
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
            this._actor.BeingHit(evt.attackData, evt.beHitEffect, evt.sourceID);
            return true;
        }

        protected override bool OnRemoteReplicatedEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.OnRemoteBeingHit((EvtBeingHit) evt));
        }

        public void SetupIdentity(AvatarIdentity identity)
        {
            this._identity = identity;
            base.Setup(this._actor, identity);
        }
    }
}

