namespace MoleMole
{
    using System;

    public abstract class BaseMPAbilityActorPlugin : BaseActorPlugin
    {
        private BaseAbilityActor _actor;
        private BaseAbilityEntityIdentiy _identity;

        protected BaseMPAbilityActorPlugin()
        {
        }

        protected virtual void OnAuthorityStart()
        {
        }

        public sealed override bool OnEvent(BaseEvent evt)
        {
            return ((!this._identity.isAuthority && (evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated)) && this.OnRemoteReplicatedEvent(evt));
        }

        protected abstract bool OnRemoteReplicatedEvent(BaseEvent evt);
        protected virtual void OnRemoteStart()
        {
            this._actor.rejectBaseEventHandlingPredicate = new Func<BaseEvent, bool>(this.RejectRemoteReplicatedResults);
        }

        public override void OnRemoved()
        {
            if (this._identity.isAuthority)
            {
                Singleton<MPManager>.Instance.DestroyMPIdentity(this._identity.runtimeID);
            }
        }

        private bool RejectRemoteReplicatedResults(BaseEvent evt)
        {
            return (evt.remoteState == EventRemoteState.IsRemoteReceiveHandledReplcated);
        }

        protected void Setup(BaseAbilityActor actor, BaseAbilityEntityIdentiy identity)
        {
            this._actor = actor;
            this._identity = identity;
            if (this._identity.isAuthority)
            {
                this.OnAuthorityStart();
            }
            else
            {
                this.OnRemoteStart();
            }
        }
    }
}

