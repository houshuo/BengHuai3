namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    public class MPLevelAbilityHelperPlugin : BaseActorPlugin
    {
        private EffectiveAttachModifier _curAttachedEntry;
        private List<PendingApplyModifierEntry> _pendingApplyLevelBuffs = new List<PendingApplyModifierEntry>();
        private State _state;

        public MPLevelAbilityHelperPlugin(MPLevelActor mpLevelActor)
        {
        }

        private void AttachCurrent(uint sourceID)
        {
            for (int i = 0; i < this._pendingApplyLevelBuffs.Count; i++)
            {
                PendingApplyModifierEntry entry = this._pendingApplyLevelBuffs[i];
                if (entry.ownerID == sourceID)
                {
                    this._curAttachedEntry = this.AttachLevelBuffModifier(this._pendingApplyLevelBuffs[i]);
                    break;
                }
            }
            this._pendingApplyLevelBuffs.Clear();
        }

        private EffectiveAttachModifier AttachLevelBuffModifier(PendingApplyModifierEntry entry)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entry.ownerID);
            if (actor == null)
            {
                return null;
            }
            ActorAbility instancedAbilityByID = actor.mpAbilityPlugin.GetInstancedAbilityByID(entry.instancedAbilityID);
            if (instancedAbilityByID == null)
            {
                return null;
            }
            BaseAbilityActor other = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(entry.otherTargetID);
            EffectiveAttachModifier modifier = new EffectiveAttachModifier {
                ownerID = entry.ownerID,
                config = entry.config,
                instancedAbilityID = entry.instancedAbilityID
            };
            foreach (AttachModifier modifier2 in entry.config.AttachModifiers)
            {
                BaseAbilityActor actor3;
                BaseAbilityActor[] actorArray;
                bool flag;
                actor.mpAbilityPlugin.ExternalResolveTarget(modifier2.Target, modifier2.TargetOption, instancedAbilityByID, other, out actor3, out actorArray, out flag);
                ConfigAbilityModifier modifier3 = instancedAbilityByID.config.Modifiers[modifier2.ModifierName];
                int localID = modifier3.localID;
                if (actor3 != null)
                {
                    Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(actor3.runtimeID).Command_TryApplyModifier(entry.ownerID, entry.instancedAbilityID, localID);
                    SubModifierLocater item = new SubModifierLocater {
                        modifierOwnerID = actor3.runtimeID,
                        modifierLocalID = localID
                    };
                    modifier.attachedModifiers.Add(item);
                }
                else if (actorArray != null)
                {
                    for (int i = 0; i < actorArray.Length; i++)
                    {
                        if (actorArray[i] != null)
                        {
                            Singleton<MPManager>.Instance.GetIdentity<BaseAbilityEntityIdentiy>(actorArray[i].runtimeID).Command_TryApplyModifier(entry.ownerID, entry.instancedAbilityID, localID);
                            SubModifierLocater locater2 = new SubModifierLocater {
                                modifierOwnerID = actorArray[i].runtimeID,
                                modifierLocalID = localID
                            };
                            modifier.attachedModifiers.Add(locater2);
                        }
                    }
                }
            }
            return modifier;
        }

        public void AttachPendingModifiersToNextLevelBuff(ApplyLevelBuff config, uint ownerID, int instancedAbilityID, uint otherTargetID)
        {
            PendingApplyModifierEntry item = new PendingApplyModifierEntry {
                config = config,
                ownerID = ownerID,
                instancedAbilityID = instancedAbilityID,
                otherTargetID = otherTargetID
            };
            this._pendingApplyLevelBuffs.Add(item);
        }

        private void DetachCurrent()
        {
            if (this._curAttachedEntry != null)
            {
                this.DetachLevelBuffModifiers(this._curAttachedEntry);
                this._curAttachedEntry = null;
            }
        }

        private void DetachLevelBuffModifiers(EffectiveAttachModifier entry)
        {
            for (int i = 0; i < entry.attachedModifiers.Count; i++)
            {
                SubModifierLocater locater = entry.attachedModifiers[i];
                BaseAbilityEntityIdentiy identiy = Singleton<MPManager>.Instance.TryGetIdentity<BaseAbilityEntityIdentiy>(locater.modifierOwnerID);
                if (identiy != null)
                {
                    SubModifierLocater locater2 = entry.attachedModifiers[i];
                    identiy.Command_TryRemoveModifier(entry.ownerID, entry.instancedAbilityID, locater2.modifierLocalID);
                }
            }
        }

        public override bool OnEvent(BaseEvent evt)
        {
            return ((evt is EvtLevelBuffState) && this.OnLevelBuffStateChange((EvtLevelBuffState) evt));
        }

        private bool OnLevelBuffStateChange(EvtLevelBuffState evt)
        {
            if (evt.state == LevelBuffState.Start)
            {
                this.AttachCurrent(evt.sourceId);
                this._state = State.WaitingForEnd;
            }
            else if (evt.state == LevelBuffState.Stop)
            {
                this.DetachCurrent();
                this._state = State.WaitingForStart;
            }
            else if (evt.state == LevelBuffState.Switch)
            {
                this.DetachCurrent();
                this.AttachCurrent(evt.sourceId);
            }
            return false;
        }

        private class EffectiveAttachModifier
        {
            public List<MPLevelAbilityHelperPlugin.SubModifierLocater> attachedModifiers = new List<MPLevelAbilityHelperPlugin.SubModifierLocater>();
            public ApplyLevelBuff config;
            public int instancedAbilityID;
            public uint ownerID;
        }

        private class PendingApplyModifierEntry
        {
            public ApplyLevelBuff config;
            public int instancedAbilityID;
            public uint otherTargetID;
            public uint ownerID;
        }

        private enum State
        {
            WaitingForStart,
            WaitingForEnd
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SubModifierLocater
        {
            public uint modifierOwnerID;
            public int modifierLocalID;
        }
    }
}

