namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialPlayerTeaching : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;
        private bool _lastMonsterBorn;
        private MonsterActor _witchTimeActor;
        private bool _witchTimeAttacking;

        public LevelTutorialPlayerTeaching(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._witchTimeAttacking = false;
            this._lastMonsterBorn = false;
            this._witchTimeActor = null;
        }

        public override void Core()
        {
            if (base.active && this._witchTimeAttacking)
            {
                if ((((this._witchTimeActor != null) && (base.step == 10)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Active) && !this.IsAllStepDone())) && (this._witchTimeActor.entity.GetCurrentNormalizedTime() > this.GetDelayTime(base.step)))
                {
                    this.DoneCurrentStep();
                    this._witchTimeAttacking = false;
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep10));
                }
                if ((((this._witchTimeActor != null) && (base.step == 15)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Active) && !this.IsAllStepDone())) && (this._witchTimeActor.entity.GetCurrentNormalizedTime() > this.GetDelayTime(base.step)))
                {
                    this.DoneCurrentStep();
                    this._witchTimeAttacking = false;
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep15));
                }
                if (this.IsAllStepDone())
                {
                    this.Finish();
                }
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
            if (evt is EvtStageCreated)
            {
                return this.ListenStageCreated((EvtStageCreated) evt);
            }
            if (evt is EvtFieldEnter)
            {
                return this.ListenFieldEnter((EvtFieldEnter) evt);
            }
            if (evt is EvtMonsterCreated)
            {
                return this.ListenMonsterCreated((EvtMonsterCreated) evt);
            }
            if (evt is EvtKilled)
            {
                return this.ListenMonsterKilled((EvtKilled) evt);
            }
            if (evt is EvtAttackStart)
            {
                return this.ListenMonsterAttackStart((EvtAttackStart) evt);
            }
            if (evt is EvtAvatarCreated)
            {
                return this.OnAvatarCreated((EvtAvatarCreated) evt);
            }
            return ((evt is EvtBeingHit) && this.OnBeingHit((EvtBeingHit) evt));
        }

        private bool ListenFieldEnter(EvtFieldEnter evt)
        {
            return false;
        }

        private bool ListenMonsterAttackStart(EvtAttackStart evt)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if (((actor != null) && (base.step == 10)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this._witchTimeActor = actor;
                this._witchTimeAttacking = true;
            }
            if ((((actor != null) && (base.step == 15)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone())) && this._lastMonsterBorn)
            {
                this.ActiveCurrentStep();
                this._witchTimeActor = actor;
                this._witchTimeAttacking = true;
            }
            return false;
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            if (((base.step == 4) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep5));
            }
            else if (((base.step == 9) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep9));
            }
            else if (((base.step == 15) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this._lastMonsterBorn = true;
            }
            return false;
        }

        private bool ListenMonsterKilled(EvtKilled evt)
        {
            if (((Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID) != null) && (base.step == 7)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep8));
            }
            return false;
        }

        private bool ListenStageCreated(EvtStageCreated evt)
        {
            this.WaitShowTutorialStep(0.09f, new Action(this.SetupController));
            return false;
        }

        private bool ListenTutorialState(EvtTutorialState evt)
        {
            if ((((evt != null) && (evt.state == EvtTutorialState.State.Start)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageCreated>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtFieldEnter>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAttackStart>(base._helper.levelActor.runtimeID);
        }

        private bool OnAvatarCreated(EvtAvatarCreated evt)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
            if (actor != null)
            {
                actor.AddAbilityState(AbilityState.Undamagable, true);
            }
            return false;
        }

        private bool OnBeingHit(EvtBeingHit evt)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
            {
            }
            return false;
        }

        public override void OnDecided()
        {
            base.OnDecided();
        }

        public void OnTutorialStep13Done()
        {
            this.MoveToNextStep();
            if (((base.step == 14) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep14));
            }
        }

        public void OnTutorialStep14Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep15Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep16Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep1Done()
        {
            this.MoveToNextStep();
            if (((base.step == 1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep2));
            }
        }

        public void OnTutorialStep2Done()
        {
            this.MoveToNextStep();
            if (((base.step == 2) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep3));
            }
        }

        public void OnTutorialStep3Done()
        {
            this.MoveToNextStep();
            if (((base.step == 3) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep4));
            }
        }

        public void OnTutorialStep5Done()
        {
            this.MoveToNextStep();
            if (((base.step == 5) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep6));
            }
        }

        public void OnTutorialStep6Done()
        {
            this.MoveToNextStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackBtnVisible, true));
            if (((base.step == 6) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep7));
            }
        }

        public void OnTutorialStep8_1Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStpe4Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStpe7Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
            if (((base.step == 7) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep8));
            }
        }

        public void OnTutoriaStep10Done()
        {
            this.SetControllerEnable(true);
            this.MoveToNextStep();
            if (((base.step == 11) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep11));
            }
        }

        public void OnTutoriaStep11Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
            if (((base.step == 12) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep12));
            }
        }

        public void OnTutoriaStep12Done()
        {
            this.MoveToNextStep();
            if (((base.step == 13) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep13));
            }
        }

        public void OnTutoriaStep8Done()
        {
            this.MoveToNextStep();
            if (((base.step == 8) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep8));
            }
        }

        public void OnTutoriaStep9Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        private void SetupController()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, false));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.AttackBtnVisible, false));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, false));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.PauseBtnVisible, false));
        }

        private void ShowTutorialStep1()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        private void ShowTutorialStep10()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EvadeBtnVisible, true));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        private void ShowTutorialStep11()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep12()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        public void ShowTutorialStep13()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep14()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep15()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        public void ShowTutorialStep16()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        private void ShowTutorialStep2()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep3()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.JoystickVisible, true));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep4()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep5()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        private void ShowTutorialStep6()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep7()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep8()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
            this.PauseGame();
        }

        private void ShowTutorialStep8_1()
        {
            this.ActiveCurrentStep();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialPlayerTeaching, this));
        }

        private void ShowTutorialStep9()
        {
            this.ActiveCurrentStep();
            this.MoveToNextStep();
        }
    }
}

