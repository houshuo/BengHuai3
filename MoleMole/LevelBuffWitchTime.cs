namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class LevelBuffWitchTime : BaseLevelBuff
    {
        private float _duration;
        private bool _enteringTimeSlow;
        protected bool _notStartEffect;
        private float _timer;
        private int _witchTimeStageEffectStackIx;
        public const float WITCH_TIMESCALE = 0.1f;

        public LevelBuffWitchTime(LevelActor levelActor)
        {
            base.levelActor = levelActor;
            base.levelBuffType = LevelBuffType.WitchTime;
        }

        protected void ActBlueCloseEffect()
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Witch_Close", Singleton<CameraManager>.Instance.GetMainCamera(), true);
        }

        protected void ActBlueOpenEffect()
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Avatar_Witch_Open", Singleton<CameraManager>.Instance.GetMainCamera(), true);
        }

        protected void ActRedCloseEffect()
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_Witch_Close", Singleton<CameraManager>.Instance.GetMainCamera(), true);
        }

        protected void ActRedOpenEffect()
        {
            Singleton<EffectManager>.Instance.TriggerEntityEffectPattern("Monster_Witch_Open", Singleton<CameraManager>.Instance.GetMainCamera(), true);
        }

        protected virtual void ActStartParticleEffect()
        {
            if (!this._notStartEffect)
            {
                if (base.levelBuffSide == LevelBuffSide.FromAvatar)
                {
                    this.ActBlueOpenEffect();
                }
                else if (base.levelBuffSide == LevelBuffSide.FromMonster)
                {
                    this.ActRedOpenEffect();
                }
            }
        }

        protected virtual void ActStopParticleEffect()
        {
            if (base.levelBuffSide == LevelBuffSide.FromAvatar)
            {
                this.ActBlueCloseEffect();
            }
            if (base.levelBuffSide == LevelBuffSide.FromMonster)
            {
                this.ActRedCloseEffect();
            }
        }

        protected void ApplyWitchTimeEffect(uint actorID)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(actorID);
            if (((actor.abilityState & AbilityState.WitchTimeSlowed) == AbilityState.None) && !actor.IsImmuneAbilityState(AbilityState.WitchTimeSlowed))
            {
                actor.AddAbilityState(AbilityState.WitchTimeSlowed, false);
            }
        }

        protected virtual void ApplyWitchTimeSlowedBySide()
        {
            if ((base.levelBuffSide == LevelBuffSide.FromMonster) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
                for (int j = 0; j < allAvatars.Count; j++)
                {
                    this.ApplyWitchTimeEffect(allAvatars[j].GetRuntimeID());
                }
            }
            else if ((base.levelBuffSide == LevelBuffSide.FromAvatar) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                for (int k = 0; k < allMonsters.Count; k++)
                {
                    this.ApplyWitchTimeEffect(allMonsters[k].GetRuntimeID());
                }
            }
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            for (int i = 0; i < allPropObjects.Count; i++)
            {
                this.ApplyWitchTimeEffect(allPropObjects[i].GetRuntimeID());
            }
        }

        public virtual void ApplyWitchTimeSlowedBySideWithRuntimeID(uint runtimeID)
        {
            if ((base.levelBuffSide == LevelBuffSide.FromMonster) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                this.ApplyWitchTimeEffect(runtimeID);
            }
            else if ((base.levelBuffSide == LevelBuffSide.FromAvatar) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                this.ApplyWitchTimeEffect(runtimeID);
            }
        }

        public override void Core()
        {
            if (!base.muteUpdateDuration && (this._timer > 0f))
            {
                this._timer -= Singleton<LevelManager>.Instance.levelEntity.TimeScale * Time.deltaTime;
                if (this._timer <= 0f)
                {
                    base.levelActor.StopLevelBuff(this);
                }
            }
        }

        private void DoWitchTimeStart()
        {
            this.ActStartParticleEffect();
            this.PushRenderingDataBySide();
            this.ApplyWitchTimeSlowedBySide();
            Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.Push(1, 0.1f, false);
            this._timer = this._duration;
            this._enteringTimeSlow = false;
        }

        public void ExtendDuration(float duration, bool enteringTimeSlow, bool useMaxDuration)
        {
            if (enteringTimeSlow)
            {
                Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.5f);
            }
            if (useMaxDuration)
            {
                this._duration = Mathf.Max(duration, this._timer);
            }
            else
            {
                this._duration = duration;
            }
            this._timer = this._duration;
            this.ActStartParticleEffect();
        }

        public override void OnAdded()
        {
            this.WitchTimeStart();
        }

        public override bool OnPostEvent(BaseEvent evt)
        {
            if (evt is EvtMonsterCreated)
            {
                return this.OnPostMonsterCreated((EvtMonsterCreated) evt);
            }
            return ((evt is EvtPropObjectCreated) && this.OnPostPropCreated((EvtPropObjectCreated) evt));
        }

        private bool OnPostMonsterCreated(EvtMonsterCreated evt)
        {
            if ((base.levelBuffSide == LevelBuffSide.FromAvatar) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                this.ApplyWitchTimeEffect(evt.monsterID);
            }
            return true;
        }

        private bool OnPostPropCreated(EvtPropObjectCreated evt)
        {
            if ((base.levelBuffSide == LevelBuffSide.FromMonster) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                this.ApplyWitchTimeEffect(evt.objectID);
            }
            return true;
        }

        public override void OnRemoved()
        {
            this._timer = 0f;
            this.WitchTimeStop();
        }

        protected void PushBlueRenderingData()
        {
            this._witchTimeStageEffectStackIx = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData("Effect_WitchTime", 0.2f);
        }

        protected void PushRedRenderingData()
        {
            this._witchTimeStageEffectStackIx = Singleton<StageManager>.Instance.GetPerpStage().PushRenderingData("Effect_WitchTime_Monster", 0.2f);
        }

        protected virtual void PushRenderingDataBySide()
        {
            if ((base.levelBuffSide == LevelBuffSide.FromMonster) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                this.PushRedRenderingData();
            }
            else
            {
                this.PushBlueRenderingData();
            }
        }

        public virtual bool Refresh(float duration, LevelBuffSide side, uint ownerID, bool enteringTimeSlow, bool useMaxDuration, bool notStartEffect)
        {
            if (side == base.levelBuffSide)
            {
                this.ExtendDuration(duration, enteringTimeSlow, useMaxDuration);
                return false;
            }
            this.SwitchSide(enteringTimeSlow, duration, side, ownerID, notStartEffect);
            return true;
        }

        protected void RemoveWitchTimeEffect(uint actorID)
        {
            BaseAbilityActor actor = Singleton<EventManager>.Instance.GetActor<BaseAbilityActor>(actorID);
            if (actor.abilityState.ContainsState(AbilityState.WitchTimeSlowed))
            {
                actor.RemoveAbilityState(AbilityState.WitchTimeSlowed);
            }
        }

        protected virtual void RemoveWitchTimeSlowedBySide()
        {
            if ((base.levelBuffSide == LevelBuffSide.FromMonster) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                List<BaseMonoAvatar> allAvatars = Singleton<AvatarManager>.Instance.GetAllAvatars();
                for (int j = 0; j < allAvatars.Count; j++)
                {
                    this.RemoveWitchTimeEffect(allAvatars[j].GetRuntimeID());
                }
            }
            else if ((base.levelBuffSide == LevelBuffSide.FromAvatar) || (base.levelBuffSide == LevelBuffSide.FromLevel))
            {
                List<BaseMonoMonster> allMonsters = Singleton<MonsterManager>.Instance.GetAllMonsters();
                for (int k = 0; k < allMonsters.Count; k++)
                {
                    this.RemoveWitchTimeEffect(allMonsters[k].GetRuntimeID());
                }
            }
            List<BaseMonoPropObject> allPropObjects = Singleton<PropObjectManager>.Instance.GetAllPropObjects();
            for (int i = 0; i < allPropObjects.Count; i++)
            {
                this.RemoveWitchTimeEffect(allPropObjects[i].GetRuntimeID());
            }
        }

        public void Setup(bool enteringTimeSlow, float duration, LevelBuffSide side, bool notStartEffect)
        {
            this._enteringTimeSlow = enteringTimeSlow;
            this._duration = duration;
            base.levelBuffSide = side;
            this._notStartEffect = notStartEffect;
        }

        public virtual void SwitchSide(bool enteringTimeSlow, float duration, LevelBuffSide side, uint newOwnerID, bool notStartEffect)
        {
            if (this._enteringTimeSlow)
            {
                this.Setup(false, duration, side, notStartEffect);
                this.DoWitchTimeStart();
                this._enteringTimeSlow = false;
            }
            else
            {
                this._enteringTimeSlow = false;
                this._duration = duration;
                this._timer = this._duration;
                this.RemoveWitchTimeSlowedBySide();
                base.levelBuffSide = side;
                base.ownerID = newOwnerID;
                this.ActStartParticleEffect();
                this.ApplyWitchTimeSlowedBySide();
                Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(this._witchTimeStageEffectStackIx);
                this.PushRenderingDataBySide();
            }
        }

        private void WitchTimeSFX()
        {
            Singleton<WwiseAudioManager>.Instance.Post("Avatar_TimeSkill_Boomoff", null, null, null);
        }

        private void WitchTimeStart()
        {
            if (this._enteringTimeSlow)
            {
                Singleton<LevelManager>.Instance.levelActor.TimeSlow(0.5f, 0.05f, delegate {
                    if (this._enteringTimeSlow && base.isActive)
                    {
                        this.DoWitchTimeStart();
                    }
                });
            }
            else
            {
                this.DoWitchTimeStart();
            }
            RainController rainController = Singleton<StageManager>.Instance.GetPerpStage().rainController;
            if (rainController != null)
            {
                rainController.EnterSlowMode(1f);
            }
            this.WitchTimeSFX();
        }

        private void WitchTimeStop()
        {
            this.ActStopParticleEffect();
            Singleton<StageManager>.Instance.GetPerpStage().PopRenderingData(this._witchTimeStageEffectStackIx);
            this.RemoveWitchTimeSlowedBySide();
            RainController rainController = Singleton<StageManager>.Instance.GetPerpStage().rainController;
            if (rainController != null)
            {
                rainController.LeaveSlowMode();
            }
            Singleton<LevelManager>.Instance.levelEntity.auxTimeScaleStack.TryPop(1);
        }
    }
}

