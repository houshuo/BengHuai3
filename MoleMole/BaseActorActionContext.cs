namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UniRx;

    public abstract class BaseActorActionContext
    {
        private List<Tuple<BaseMonoAbilityEntity, string>> _animEventMasked;
        private List<Tuple<AvatarActor, bool>> _attachAllowSwitchOther;
        private List<Tuple<BaseMonoAbilityEntity, bool>> _attachedAllowSelected;
        private List<Tuple<BaseMonoAbilityEntity, string>> _attachedEffectOverrides;
        private List<BaseAbilityActor> _attachedImmuneDebuff;
        private List<Tuple<BaseMonoAbilityEntity, bool>> _attachedIsGhost;
        private List<BaseMonoAbilityEntity> _attachedNoCollisions;
        private List<Tuple<BaseMonoAbilityEntity, float>> _attachedOpacity;
        private List<int> _attachedPatternIndices;
        private List<Tuple<BaseAbilityActor, ConfigBuffDebuffResistance>> _attachedResistanceBuffDebuffs;
        private List<int> _attachedStageTintsIndices;
        private List<Tuple<BaseAbilityActor, AbilityState>> _attachedStateImmunes;
        private List<Tuple<AvatarActor, bool>> _attachMuteOtherQTE;
        private List<BaseMonoAbilityEntity> _materialGroupPushed;
        private List<ActorModifier> _modifiersAttached;
        private List<Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState>> _ownedPredicates;
        [InspectorCollapsedFoldout]
        public BaseAbilityMixin[] instancedMixins;

        public void AttachAllowSelected(BaseMonoAbilityEntity target, bool allowSelected)
        {
            target.SetCountedDenySelect(!allowSelected, false);
            this.CheckInit<Tuple<BaseMonoAbilityEntity, bool>>(ref this._attachedAllowSelected);
            this._attachedAllowSelected.Add(Tuple.Create<BaseMonoAbilityEntity, bool>(target, allowSelected));
        }

        public void AttachAllowSwitchOther(BaseMonoAbilityEntity target, bool allowSwitchOther)
        {
            if (Singleton<LevelManager>.Instance.levelActor.levelMode == LevelActor.Mode.Single)
            {
                this.CheckInit<Tuple<AvatarActor, bool>>(ref this._attachAllowSwitchOther);
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(target.GetRuntimeID());
                if (actor != null)
                {
                    actor.AllowOtherSwitchIn = allowSwitchOther;
                    if (allowSwitchOther)
                    {
                        actor.ResetSwitchInTimer();
                    }
                    this._attachAllowSwitchOther.Add(Tuple.Create<AvatarActor, bool>(actor, allowSwitchOther));
                }
            }
        }

        public void AttachAnimEventPredicate(BaseMonoAbilityEntity target, string predicate)
        {
            this.CheckInit<Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState>>(ref this._ownedPredicates);
            this._ownedPredicates.Add(Tuple.Create<BaseMonoAbilityEntity, string, OwnedPredicateState>(target, predicate, OwnedPredicateState.Attach));
        }

        public void AttachBuffDebuffResistance(BaseAbilityActor target, MoleMole.Config.AttachBuffDebuffResistance resistance)
        {
            this.CheckInit<Tuple<BaseAbilityActor, ConfigBuffDebuffResistance>>(ref this._attachedResistanceBuffDebuffs);
            ConfigBuffDebuffResistance resistance2 = new ConfigBuffDebuffResistance(resistance.ResistanceBuffDebuffs, resistance.ResistanceRatio, resistance.ResistanceDurationRatio);
            this._attachedResistanceBuffDebuffs.Add(Tuple.Create<BaseAbilityActor, ConfigBuffDebuffResistance>(target, resistance2));
            target.AddBuffDebuffResistance(resistance2);
        }

        public void AttachEffectOverride(BaseMonoAbilityEntity target, string key)
        {
            this.CheckInit<Tuple<BaseMonoAbilityEntity, string>>(ref this._attachedEffectOverrides);
            this._attachedEffectOverrides.Add(Tuple.Create<BaseMonoAbilityEntity, string>(target, key));
        }

        public void AttachEffectPatternIndex(int patternIx)
        {
            this.CheckInit<int>(ref this._attachedPatternIndices);
            this._attachedPatternIndices.Add(patternIx);
        }

        public void AttachImmuneAbilityState(BaseAbilityActor target, AbilityState state)
        {
            this.CheckInit<Tuple<BaseAbilityActor, AbilityState>>(ref this._attachedStateImmunes);
            this._attachedStateImmunes.Add(Tuple.Create<BaseAbilityActor, AbilityState>(target, state));
        }

        public void AttachImmuneDebuff(BaseAbilityActor target)
        {
            this.CheckInit<BaseAbilityActor>(ref this._attachedImmuneDebuff);
            this._attachedImmuneDebuff.Add(target);
        }

        public void AttachIsGhost(BaseMonoAbilityEntity target, bool isGhost)
        {
            target.SetCountedIsGhost(isGhost);
            this.CheckInit<Tuple<BaseMonoAbilityEntity, bool>>(ref this._attachedIsGhost);
            this._attachedIsGhost.Add(Tuple.Create<BaseMonoAbilityEntity, bool>(target, isGhost));
        }

        public void AttachMaskedAnimEventID(BaseMonoAbilityEntity target, string animEventID)
        {
            this.CheckInit<Tuple<BaseMonoAbilityEntity, string>>(ref this._animEventMasked);
            this._animEventMasked.Add(Tuple.Create<BaseMonoAbilityEntity, string>(target, animEventID));
        }

        public void AttachModifier(ActorModifier modifier)
        {
            this.CheckInit<ActorModifier>(ref this._modifiersAttached);
            this._modifiersAttached.Add(modifier);
        }

        public void AttachMuteOtherQTE(BaseMonoAbilityEntity target, bool muteOtherQTE)
        {
            this.CheckInit<Tuple<AvatarActor, bool>>(ref this._attachMuteOtherQTE);
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(target.GetRuntimeID());
            if (actor != null)
            {
                actor.MuteOtherQTE = muteOtherQTE;
                this._attachMuteOtherQTE.Add(Tuple.Create<AvatarActor, bool>(actor, muteOtherQTE));
            }
        }

        public void AttachNoCollision(BaseMonoAbilityEntity target)
        {
            target.PushNoCollision();
            this.CheckInit<BaseMonoAbilityEntity>(ref this._attachedNoCollisions);
            this._attachedNoCollisions.Add(target);
        }

        public void AttachOpacity(BaseMonoAbilityEntity target, float opacity)
        {
            PropObjectActor actor = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(target.GetRuntimeID());
            if (actor != null)
            {
                actor.SetPorpObjectOpacity(opacity);
                this.CheckInit<Tuple<BaseMonoAbilityEntity, float>>(ref this._attachedOpacity);
                this._attachedOpacity.Add(Tuple.Create<BaseMonoAbilityEntity, float>(target, opacity));
            }
        }

        public void AttachPushMaterialGroup(BaseMonoAbilityEntity target)
        {
            this.CheckInit<BaseMonoAbilityEntity>(ref this._materialGroupPushed);
            this._materialGroupPushed.Add(target);
        }

        public void AttachStageTint(MoleMole.Config.AttachStageTint tintConfig)
        {
            this.CheckInit<int>(ref this._attachedStageTintsIndices);
            int item = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData(tintConfig.RenderDataName, tintConfig.TransitDuration);
            this._attachedStageTintsIndices.Add(item);
        }

        protected void AttachToActor(BaseAbilityActor actor)
        {
        }

        private void CheckInit<T>(ref List<T> ls)
        {
            if (ls == null)
            {
                ls = new List<T>();
            }
        }

        [Conditional("NG_HSOD_DEBUG"), Conditional("UNITY_EDITOR")]
        protected void DebugLogContext(string format, params object[] args)
        {
        }

        public void DetachAnimEventPredicate(BaseMonoAbilityEntity target, string predicate)
        {
            this.CheckInit<Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState>>(ref this._ownedPredicates);
            this._ownedPredicates.Add(Tuple.Create<BaseMonoAbilityEntity, string, OwnedPredicateState>(target, predicate, OwnedPredicateState.Detach));
        }

        protected void DetachFromActor(BaseAbilityActor actor)
        {
            if (this._ownedPredicates != null)
            {
                for (int i = 0; i < this._ownedPredicates.Count; i++)
                {
                    Tuple<BaseMonoAbilityEntity, string, OwnedPredicateState> tuple = this._ownedPredicates[i];
                    if (tuple.Item1 != null)
                    {
                        if (((OwnedPredicateState) tuple.Item3) == OwnedPredicateState.Attach)
                        {
                            tuple.Item1.RemoveAnimEventPredicate(tuple.Item2);
                        }
                        else
                        {
                            tuple.Item1.AddAnimEventPredicate(tuple.Item2);
                        }
                    }
                }
                this._ownedPredicates.Clear();
            }
            if (this._attachedPatternIndices != null)
            {
                for (int j = 0; j < this._attachedPatternIndices.Count; j++)
                {
                    actor.entity.DetachEffect(this._attachedPatternIndices[j]);
                }
                this._attachedPatternIndices.Clear();
            }
            if (this._materialGroupPushed != null)
            {
                for (int k = 0; k < this._materialGroupPushed.Count; k++)
                {
                    if (this._materialGroupPushed[k] != null)
                    {
                        this._materialGroupPushed[k].PopMaterialGroup();
                    }
                }
                this._materialGroupPushed.Clear();
            }
            if (this._animEventMasked != null)
            {
                for (int m = 0; m < this._animEventMasked.Count; m++)
                {
                    if (this._animEventMasked[m].Item1 != null)
                    {
                        this._animEventMasked[m].Item1.UnmaskAnimEvent(this._animEventMasked[m].Item2);
                    }
                }
                this._animEventMasked.Clear();
            }
            if (this._modifiersAttached != null)
            {
                for (int n = 0; n < this._modifiersAttached.Count; n++)
                {
                    ActorModifier modifier = this._modifiersAttached[n];
                    if (modifier.owner != null)
                    {
                        bool flag = modifier.owner.abilityPlugin.TryRemoveModifier(modifier);
                    }
                }
                this._modifiersAttached.Clear();
            }
            if (this._attachedStateImmunes != null)
            {
                for (int num6 = 0; num6 < this._attachedStateImmunes.Count; num6++)
                {
                    Tuple<BaseAbilityActor, AbilityState> tuple2 = this._attachedStateImmunes[num6];
                    if (tuple2.Item1 != null)
                    {
                        tuple2.Item1.SetAbilityStateImmune(tuple2.Item2, false);
                    }
                }
                this._attachedStateImmunes.Clear();
            }
            if (this._attachedImmuneDebuff != null)
            {
                for (int num7 = 0; num7 < this._attachedImmuneDebuff.Count; num7++)
                {
                    BaseAbilityActor actor2 = this._attachedImmuneDebuff[num7];
                    if (this._attachedImmuneDebuff[num7] != null)
                    {
                        actor2.SetImmuneDebuff(false);
                    }
                }
                this._attachedImmuneDebuff.Clear();
            }
            if (this._attachedResistanceBuffDebuffs != null)
            {
                for (int num8 = 0; num8 < this._attachedResistanceBuffDebuffs.Count; num8++)
                {
                    BaseAbilityActor actor3 = this._attachedResistanceBuffDebuffs[num8].Item1;
                    if (actor3 != null)
                    {
                        actor3.RemoveBuffDebuffResistance(this._attachedResistanceBuffDebuffs[num8].Item2);
                    }
                }
                this._attachedResistanceBuffDebuffs.Clear();
            }
            if (this._attachedIsGhost != null)
            {
                for (int num9 = 0; num9 < this._attachedIsGhost.Count; num9++)
                {
                    Tuple<BaseMonoAbilityEntity, bool> tuple3 = this._attachedIsGhost[num9];
                    if (tuple3.Item1 != null)
                    {
                        tuple3.Item1.SetCountedIsGhost(!tuple3.Item2);
                    }
                }
                this._attachedIsGhost.Clear();
            }
            if (this._attachedAllowSelected != null)
            {
                for (int num10 = 0; num10 < this._attachedAllowSelected.Count; num10++)
                {
                    Tuple<BaseMonoAbilityEntity, bool> tuple4 = this._attachedAllowSelected[num10];
                    if (tuple4.Item1 != null)
                    {
                        tuple4.Item1.SetCountedDenySelect(tuple4.Item2, false);
                    }
                }
                this._attachedAllowSelected.Clear();
            }
            if (this._attachAllowSwitchOther != null)
            {
                for (int num11 = 0; num11 < this._attachAllowSwitchOther.Count; num11++)
                {
                    Tuple<AvatarActor, bool> tuple5 = this._attachAllowSwitchOther[num11];
                    if (tuple5.Item1 != null)
                    {
                        tuple5.Item1.SetAllowOtherCanSwitchIn(false);
                    }
                }
                this._attachAllowSwitchOther.Clear();
            }
            if (this._attachMuteOtherQTE != null)
            {
                for (int num12 = 0; num12 < this._attachMuteOtherQTE.Count; num12++)
                {
                    Tuple<AvatarActor, bool> tuple6 = this._attachMuteOtherQTE[num12];
                    if (tuple6.Item1 != null)
                    {
                        tuple6.Item1.MuteOtherQTE = false;
                    }
                }
                this._attachMuteOtherQTE.Clear();
            }
            if (this._attachedOpacity != null)
            {
                for (int num13 = 0; num13 < this._attachedOpacity.Count; num13++)
                {
                    Tuple<BaseMonoAbilityEntity, float> tuple7 = this._attachedOpacity[num13];
                    if (tuple7.Item1 != null)
                    {
                        PropObjectActor actor4 = Singleton<EventManager>.Instance.GetActor<PropObjectActor>(tuple7.Item1.GetRuntimeID());
                        if (actor4 != null)
                        {
                            actor4.SetPorpObjectOpacity(actor4.Opacity);
                        }
                    }
                }
                this._attachedOpacity.Clear();
            }
            if (this._attachedEffectOverrides != null)
            {
                for (int num14 = 0; num14 < this._attachedEffectOverrides.Count; num14++)
                {
                    Tuple<BaseMonoAbilityEntity, string> tuple8 = this._attachedEffectOverrides[num14];
                    if (tuple8.Item1 != null)
                    {
                        tuple8.Item1.RemoveEffectOverride(tuple8.Item2);
                    }
                }
                this._attachedEffectOverrides.Clear();
            }
            if (this._attachedStageTintsIndices != null)
            {
                for (int num15 = 0; num15 < this._attachedStageTintsIndices.Count; num15++)
                {
                    int stackIx = this._attachedStageTintsIndices[num15];
                    Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(stackIx);
                }
                this._attachedStageTintsIndices.Clear();
            }
            if (this._attachedNoCollisions != null)
            {
                for (int num17 = 0; num17 < this._attachedNoCollisions.Count; num17++)
                {
                    if (this._attachedNoCollisions[num17] != null)
                    {
                        this._attachedNoCollisions[num17].PopNoCollision();
                    }
                }
                this._attachedNoCollisions.Clear();
            }
        }

        public abstract string GetDebugContextName();
        public abstract BaseAbilityActor GetDebugOwner();
        public BaseAbilityMixin GetInstancedMixin(int mixinLocalID)
        {
            for (int i = 0; i < this.instancedMixins.Length; i++)
            {
                if (this.instancedMixins[i].mixinLocalID == mixinLocalID)
                {
                    return this.instancedMixins[i];
                }
            }
            return null;
        }

        private enum OwnedPredicateState
        {
            Attach,
            Detach
        }
    }
}

