namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialMonsterBlock : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;
        private string _monsterSubTypeName;

        public LevelTutorialMonsterBlock(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._monsterSubTypeName = "DG_030";
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

        private bool ListenAttackStart(EvtAttackStart evt)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if ((((actor != null) && (actor.metaConfig.subTypeName == this._monsterSubTypeName)) && ((actor.entity.CurrentSkillID == "DEF_ATK") && (base.step == 1))) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep2));
            }
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtMonsterCreated)
            {
                return this.ListenMonsterCreated((EvtMonsterCreated) evt);
            }
            return ((evt is EvtAttackStart) && this.ListenAttackStart((EvtAttackStart) evt));
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            MonsterActor actor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID);
            if ((((actor != null) && (actor.metaConfig.subTypeName == this._monsterSubTypeName)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
            {
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterBlock, this));
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
            this.ResumeGame();
        }

        public void OnTutoriaStep1Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        private void ShowTutorialStep1()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterBlock, this));
            this.ActiveCurrentStep();
            this.PauseGame();
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterBlock, this));
            this.ActiveCurrentStep();
            this.PauseGame();
        }

        private void ShowTutorialStep3()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterBlock, this));
            this.ActiveCurrentStep();
        }
    }
}

