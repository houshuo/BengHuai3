namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialBranchAttack : BaseLevelTutorial
    {
        private BaseMonoAvatar _avatar;
        private float _delayTime;
        [ShowInInspector]
        private bool _finished;
        private bool _isBranchAttackDone;
        private bool _isInBranchAttack;
        private TutorialBranchAttackMode _mode;

        public LevelTutorialBranchAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._isBranchAttackDone = false;
            this._isInBranchAttack = false;
            this._delayTime = this.GetDelayTime(base.step);
            this._mode = TutorialBranchAttackMode.Start;
        }

        public override void Core()
        {
            if (base.active)
            {
                if (this._mode == TutorialBranchAttackMode.ReadyForTiming)
                {
                    if (this._avatar.GetCurrentNormalizedTime() > this._delayTime)
                    {
                        this._mode = TutorialBranchAttackMode.Attacking;
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, false));
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UltraBtnVisible, false));
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
                        this.ShowStep5Tutorial();
                    }
                }
                else if (this._mode == TutorialBranchAttackMode.Done)
                {
                    this.Finish();
                }
                if ((this._isInBranchAttack && this.IsInStep(4)) && (!this.IsAllStepDone() && (this._avatar.CurrentSkillID == "ATK04")))
                {
                    this._isBranchAttackDone = true;
                    this._isInBranchAttack = false;
                }
            }
        }

        private void EnterReadyForBranchAttack()
        {
            this._mode = TutorialBranchAttackMode.ReadyForTiming;
            this.ActiveCurrentStep();
            this.SetControllerEnable(false);
        }

        private void Fail()
        {
            this._finished = false;
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

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            if (this._mode == TutorialBranchAttackMode.Start)
            {
                this._avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (this._avatar == null)
                {
                    return false;
                }
                if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
                {
                    bool flag = this._avatar.CurrentSkillID == "ATK02_New";
                    bool locomotionBool = this._avatar.GetLocomotionBool("AbilityUnlockBranchAttack");
                    if ((flag && locomotionBool) && this.IsInStep(4))
                    {
                        this._delayTime = this.GetDelayTime(base.step);
                        this.EnterReadyForBranchAttack();
                    }
                }
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtAttackStart)
            {
                return this.ListenAttackStart((EvtAttackStart) evt);
            }
            return ((evt is EvtTutorialState) && this.ListenTutorialState((EvtTutorialState) evt));
        }

        private bool ListenTutorialState(EvtTutorialState evt)
        {
            if ((evt != null) && (evt.state == EvtTutorialState.State.Start))
            {
                this._avatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                if (this._avatar == null)
                {
                    return false;
                }
                this.WaitShowTutorialStep(0f, new Action(this.StartCheckStageReady));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
        }

        public void OnStep1Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep2Tutorial));
            }
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
            if ((this.IsInStep(3) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep2Tutorial));
            }
        }

        public void OnStep4Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        public void OnStep5Done()
        {
            this._mode = TutorialBranchAttackMode.AfterAttack;
            this.MoveToNextStep();
            this.ResumeGame();
            if (((base.step == 5) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep6Tutorial));
            }
        }

        public void OnStep5PointerDown()
        {
            this.ResumeGame();
        }

        public bool OnStep5PoointerUp()
        {
            if (!this._isBranchAttackDone)
            {
                this.PauseGame();
            }
            else
            {
                this._mode = TutorialBranchAttackMode.AfterAttack;
                this.MoveToNextStep();
                if (((base.step == 5) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
                {
                    this.ActiveCurrentStep();
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep6Tutorial));
                }
            }
            return this._isBranchAttackDone;
        }

        public void OnStep6Done()
        {
            this.MoveToNextStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, true));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.UltraBtnVisible, true));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
            this.ResumeGame();
            this._mode = TutorialBranchAttackMode.Done;
        }

        private void ShowStep1Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
            this.PauseGame();
        }

        private void ShowStep2Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
        }

        private void ShowStep3Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
        }

        private void ShowStep4Tutorial()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
        }

        private void ShowStep5Tutorial()
        {
            this.SetControllerEnable(true);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
            this._isInBranchAttack = true;
            this.PauseGame();
        }

        private void ShowStep6Tutorial()
        {
            this.PauseGame();
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialBranchAttack, this));
        }

        private void StartCheckStageReady()
        {
            bool locomotionBool = this._avatar.GetLocomotionBool("AbilityUnlockBranchAttack");
            bool flag2 = this._avatar.AvatarTypeName == "Kiana_C2_PT";
            if ((locomotionBool && flag2) && ((this.IsInStep(0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowStep1Tutorial));
            }
        }

        public enum TutorialBranchAttackMode
        {
            Start,
            ReadyForTiming,
            Attacking,
            AfterAttack,
            Done
        }
    }
}

