namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialEliteAttack : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;

        public LevelTutorialEliteAttack(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
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
            Singleton<LevelTutorialModule>.Instance.MarkTutorialIDFinish(base.tutorialId);
            this.OnDecided();
        }

        public override bool IsFinished()
        {
            return this._finished;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtBeingHit)
            {
                return this.ListenMonsterBeingHit((EvtBeingHit) evt);
            }
            return ((evt is EvtShieldBroken) && this.ListenShieldBroken((EvtShieldBroken) evt));
        }

        private bool ListenMonsterBeingHit(EvtBeingHit evt)
        {
            bool flag = Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.sourceID);
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if (flag && (actor != null))
            {
                BaseMonoAvatar avatarByRuntimeID = Singleton<AvatarManager>.Instance.GetAvatarByRuntimeID(evt.sourceID);
                if ((((avatarByRuntimeID != null) && (avatarByRuntimeID.GetAttackTarget() != null)) && (actor.isElite && (base.step == 0))) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
                {
                    this.ActiveCurrentStep();
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
                }
            }
            return false;
        }

        private bool ListenShieldBroken(EvtShieldBroken evt)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if ((((actor != null) && actor.isElite) && ((base.step == 1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(0.5f, new Action(this.ShowTutorialStep2));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtShieldBroken>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtShieldBroken>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialEliteAttack, this));
        }

        public void OnTutorialStep1Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        public void OnTutorialStep2Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        private void ShowTutorialStep1()
        {
            this.NotifyStep(NotifyTypes.TutorialEliteAttack);
            this.PauseGame();
            this.ActiveCurrentStep();
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialEliteAttack, this));
            this.PauseGame();
        }
    }
}

