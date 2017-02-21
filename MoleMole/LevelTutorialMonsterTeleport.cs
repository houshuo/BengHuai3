namespace MoleMole
{
    using FullInspector;
    using System;

    public class LevelTutorialMonsterTeleport : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;
        private string _monsterConfigTypeName;
        private string _monsterSubTypeName;

        public LevelTutorialMonsterTeleport(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._monsterSubTypeName = "DG_031";
            this._monsterConfigTypeName = "CS_Default";
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

        private bool IsMonsterDeadKillerWithTeleportSkill(MonsterActor monsterActor)
        {
            bool flag = false;
            if (((monsterActor != null) && (monsterActor.metaConfig.subTypeName == this._monsterSubTypeName)) && (monsterActor.metaConfig.configType == this._monsterConfigTypeName))
            {
                flag = true;
            }
            return flag;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtTeleport)
            {
                return this.ListenTeleport((EvtTeleport) evt);
            }
            return ((evt is EvtMonsterCreated) && this.ListenMonsterCreated((EvtMonsterCreated) evt));
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.monsterID);
            if ((((monsterActor != null) && this.IsMonsterDeadKillerWithTeleportSkill(monsterActor)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
            return false;
        }

        private bool ListenTeleport(EvtTeleport evt)
        {
            MonsterActor monsterActor = Singleton<EventManager>.Instance.GetActor<MonsterActor>(evt.targetID);
            if ((((monsterActor != null) && this.IsMonsterDeadKillerWithTeleportSkill(monsterActor)) && ((base.step == 1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep2));
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtTeleport>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtTeleport>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
        }

        public void OnTutorialStep1Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep2Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        private void ShowTutorialStep1()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
            this.ActiveCurrentStep();
            this.PauseGame();
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterTeleport, this));
            this.ActiveCurrentStep();
            this.PauseGame();
        }
    }
}

