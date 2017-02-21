namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;

    public class AbilityAvatarSaveAlliedMixin : BaseAbilityMixin
    {
        private List<uint> _alliedIDs;
        private bool _saveActive;
        private int _saveCountRemains;
        private AvatarSaveAlliedMixin config;

        public AbilityAvatarSaveAlliedMixin(ActorAbility instancedAbility, ActorModifier instancedModifier, ConfigAbilityMixin config) : base(instancedAbility, instancedModifier, config)
        {
            this.config = (AvatarSaveAlliedMixin) config;
            this._alliedIDs = new List<uint>();
        }

        private bool ListenBeingHit(EvtBeingHit evt)
        {
            if (!this._saveActive)
            {
                return false;
            }
            if (evt.attackData.rejected)
            {
                return false;
            }
            if (this._alliedIDs.Contains(evt.targetID))
            {
                AvatarActor other = Singleton<EventManager>.Instance.GetActor<AvatarActor>(evt.targetID);
                if ((other.HP == 0f) && other.IsOnStage())
                {
                    Singleton<LevelManager>.Instance.levelActor.TriggerSwapLocalAvatar(other.runtimeID, base.actor.runtimeID, true);
                    base.actor.abilityPlugin.HandleActionTargetDispatch(this.config.AdditionalActions, base.instancedAbility, base.instancedModifier, other, null);
                    this._saveCountRemains--;
                }
                if (this._saveCountRemains == 0)
                {
                    this.StopSaving();
                }
            }
            return true;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            return ((evt is EvtBeingHit) && this.ListenBeingHit((EvtBeingHit) evt));
        }

        public override void OnAdded()
        {
            if ((Singleton<AvatarManager>.Instance.IsHelperAvatar(base.actor.runtimeID) || !Singleton<AvatarManager>.Instance.IsPlayerAvatar(base.actor.runtimeID)) || (Singleton<LevelManager>.Instance.levelActor.levelMode != LevelActor.Mode.Single))
            {
                this._saveCountRemains = 0;
                this._saveActive = false;
            }
            else
            {
                this._saveCountRemains = base.instancedAbility.Evaluate(this.config.SaveCountLimit);
                this.StartSaving();
            }
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtKilled)
            {
                return this.OnPostKilled((EvtKilled) evt);
            }
            return ((evt is EvtReviveAvatar) && this.OnPostReviveAvatar((EvtReviveAvatar) evt));
        }

        private bool OnPostKilled(EvtKilled evt)
        {
            if (this._saveActive)
            {
                this.StopSaving();
            }
            return true;
        }

        private bool OnPostReviveAvatar(EvtReviveAvatar evt)
        {
            if (this._saveCountRemains > 0)
            {
                this.StartSaving();
            }
            return true;
        }

        public override void OnRemoved()
        {
            if (this._saveCountRemains > 0)
            {
                this.StopSaving();
            }
        }

        private void StartSaving()
        {
            this._saveActive = true;
            AvatarActor[] alliedActorsOf = Singleton<EventManager>.Instance.GetAlliedActorsOf<AvatarActor>(base.actor);
            for (int i = 0; i < alliedActorsOf.Length; i++)
            {
                if (Singleton<AvatarManager>.Instance.IsPlayerAvatar(base.actor.runtimeID) && (alliedActorsOf[i] != base.actor))
                {
                    alliedActorsOf[i].AddAbilityState(AbilityState.Limbo, true);
                    this._alliedIDs.Add(alliedActorsOf[i].runtimeID);
                }
            }
            Singleton<EventManager>.Instance.RegisterEventListener<EvtBeingHit>(base.actor.runtimeID);
        }

        private void StopSaving()
        {
            this._saveActive = false;
            for (int i = 0; i < this._alliedIDs.Count; i++)
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this._alliedIDs[i]);
                if ((actor != null) && ((actor.abilityState & AbilityState.Limbo) != AbilityState.None))
                {
                    actor.RemoveAbilityState(AbilityState.Limbo);
                }
            }
            this._alliedIDs.Clear();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtBeingHit>(base.actor.runtimeID);
        }
    }
}

