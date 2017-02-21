namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class LevelTutorialUltraAttack : BaseLevelTutorial
    {
        [ShowInInspector]
        private AvatarActor _actor;
        [ShowInInspector]
        private bool _finished;
        private bool _isHealingSP;
        private const float DEFAULT_SP_PERCENT = 0.3f;

        public LevelTutorialUltraAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._actor = null;
            this._isHealingSP = false;
        }

        private void ChargeSPToFull()
        {
            if (this._actor != null)
            {
                this._isHealingSP = true;
            }
        }

        public override void Core()
        {
            base.Core();
            if (base.active && (this._actor != null))
            {
                if (this._isHealingSP)
                {
                    this._actor.HealSP(Time.smoothDeltaTime * 100f);
                    if (this._actor.SP >= this._actor.maxSP)
                    {
                        this._isHealingSP = false;
                        if ((this.IsInStep(3) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
                        {
                            this.ActiveCurrentStep();
                            this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep4Tutorial));
                        }
                    }
                }
                if (this.IsAllStepDone())
                {
                    this.Finish();
                }
            }
        }

        private void Finish()
        {
            this._finished = true;
            Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(base.tutorialId);
            this.OnDecided();
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtTutorialState)
            {
                return this.ListenTutorialState((EvtTutorialState) evt);
            }
            return ((evt is EvtMonsterCreated) && this.ListenMonsterCreated((EvtMonsterCreated) evt));
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            if ((this.IsInStep(1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep2Tutorial));
            }
            return false;
        }

        private bool ListenTutorialState(EvtTutorialState evt)
        {
            if ((evt != null) && (evt.state == EvtTutorialState.State.Start))
            {
                this._actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID());
                if (this._actor == null)
                {
                    return false;
                }
                this.WaitShowTutorialStep(0.5f, new Action(this.StartCheckStageReady));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
        }

        public void OnStep1Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        public void OnStep2Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(2) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep3Tutorial));
            }
        }

        public void OnStep3Done()
        {
            this.MoveToNextStep();
            this.ChargeSPToFull();
        }

        public void OnStep4Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
            if ((this.IsInStep(4) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep5Tutorial));
            }
        }

        public void OnStep5Done()
        {
            this.MoveToNextStep();
        }

        private void ShowStep1Tutorial()
        {
            this.MoveToNextStep();
        }

        private void ShowStep2Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
            this.PauseGame();
            this.ActiveCurrentStep();
        }

        private void ShowStep3Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
            this.ActiveCurrentStep();
        }

        private void ShowStep4Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
            this.PauseGame();
            this.ActiveCurrentStep();
        }

        private void ShowStep5Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialUltraAttack, this));
            this.ActiveCurrentStep();
            this.WaitShowTutorialStep(2.5f, new Action(this.OnStep5Done));
        }

        private void StartCheckStageReady()
        {
            bool flag = !this._actor.IsSkillLocked("SKL02");
            if (((this.IsInStep(0) && (this.GetStepState(0) == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone()) && flag)
            {
                if (this._actor != null)
                {
                    this._actor.HealSP(this._actor.maxSP * 0.3f);
                }
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep1Tutorial));
            }
        }
    }
}

