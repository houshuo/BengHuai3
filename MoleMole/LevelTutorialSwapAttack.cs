namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialSwapAttack : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;
        public uint sourceSwapAvatarId;
        public uint targetSwapAvatarId;

        public LevelTutorialSwapAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.targetSwapAvatarId = 0;
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
            Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(base.tutorialId);
            this.OnDecided();
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtStageReady)
            {
                return this.ListenStageReady((EvtStageReady) evt);
            }
            return ((evt is EvtAvatarSwapInEnd) && this.ListenSwapInEnd((EvtAvatarSwapInEnd) evt));
        }

        private bool ListenStageReady(EvtStageReady evt)
        {
            bool flag = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars().Count > 1;
            this.SetupAvatarId();
            if ((flag && (base.step == 0)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
            return false;
        }

        private bool ListenSwapInEnd(EvtAvatarSwapInEnd evt)
        {
            if (((base.step == 4) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && (!this.IsAllStepDone() && (this.targetSwapAvatarId != 0)))
            {
                this.ActiveCurrentStep();
                this.targetSwapAvatarId = this.sourceSwapAvatarId;
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep5));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
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

        public void OnTutorialStep4Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        public void OnTutorialStep5Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        private void SetupAvatarId()
        {
            foreach (BaseMonoAvatar avatar in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar.GetRuntimeID());
                if ((actor != null) && !Singleton<AvatarManager>.Instance.IsLocalAvatar(avatar.GetRuntimeID()))
                {
                    this.targetSwapAvatarId = actor.runtimeID;
                }
            }
            this.sourceSwapAvatarId = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
        }

        private void ShowTutorialStep1()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
            this.PauseGame();
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
            this.ActiveCurrentStep();
        }

        private void ShowTutorialStep3()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
            this.ActiveCurrentStep();
        }

        private void ShowTutorialStep4()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAttack, this));
            this.ActiveCurrentStep();
        }

        public void ShowTutorialStep5()
        {
            this.MoveToNextStep();
        }
    }
}

