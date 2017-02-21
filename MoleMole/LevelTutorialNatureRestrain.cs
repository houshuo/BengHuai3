namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;

    public class LevelTutorialNatureRestrain : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;

        public LevelTutorialNatureRestrain(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
        }

        public override void Core()
        {
            if (base.active && this.IsAllStepDone())
            {
                this.Finish();
            }
        }

        private void Fail()
        {
            this._finished = false;
            this.OnDecided();
        }

        private void Finish()
        {
            this._finished = true;
            this.OnDecided();
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            return false;
        }

        private bool ListenAvatarCreated(EvtAvatarCreated evt)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            localAvatar.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Combine(localAvatar.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
            return false;
        }

        private bool ListenAvatarSwapInEnd(EvtAvatarSwapInEnd evt)
        {
            BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.targetID);
            if (avatarByRuntimeID != null)
            {
                avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Combine(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            }
            return false;
        }

        private bool ListenAvatarSwapOutStart(EvtAvatarSwapOutStart evt)
        {
            BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.targetID);
            if (avatarByRuntimeID != null)
            {
                avatarByRuntimeID.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Remove(avatarByRuntimeID.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtAvatarCreated)
            {
                return this.ListenAvatarCreated((EvtAvatarCreated) evt);
            }
            if (evt is EvtAvatarSwapInEnd)
            {
                return this.ListenAvatarSwapInEnd((EvtAvatarSwapInEnd) evt);
            }
            return ((evt is EvtAvatarSwapOutStart) && this.ListenAvatarSwapOutStart((EvtAvatarSwapOutStart) evt));
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarCreated>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarCreated>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
        }

        public void OnTutorialStep1Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep2));
            }
        }

        public void OnTutorialStep2Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(2) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep3));
            }
        }

        public void OnTutorialStep3Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(3) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep4));
            }
        }

        public void OnTutorialStep4Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(4) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep4));
            }
        }

        public void OnTutorialStep5Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        private void OnUpdateAttackTarget(BaseMonoEntity entity)
        {
            if (entity != null)
            {
                MonsterActor attackee = Singleton<EventManager>.Instance.GetActor<MonsterActor>(entity.GetRuntimeID());
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
                if ((((attackee != null) && (actor != null)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
                {
                    EntityNature attackerNature = (EntityNature) attackee.metaConfig.nature;
                    EntityNature attribute = (EntityNature) actor.avatarDataItem.Attribute;
                    if (DamageModelLogic.GetNatureDamageBonusRatio(attackerNature, attribute, attackee) > 1f)
                    {
                        this.ActiveCurrentStep();
                        this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
                    }
                }
            }
        }

        private void ShowTutorialStep1()
        {
            this.PauseGame();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
        }

        private void ShowTutorialStep3()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
        }

        private void ShowTutorialStep4()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
            this.ActiveCurrentStep();
        }

        private void ShowTutorialStep5()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialNatureRestrain, this));
            this.ActiveCurrentStep();
        }
    }
}

