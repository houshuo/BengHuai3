namespace MoleMole
{
    using FullInspector;
    using MoleMole.Config;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class LevelTutorialSwapAndRestrain : BaseLevelTutorial
    {
        [ShowInInspector]
        private bool _finished;
        private bool _isTutorialAvailable;
        [ShowInInspector]
        private int _killAmount;
        [ShowInInspector]
        private int _monsterCreatedAmount;
        [CompilerGenerated]
        private static Action <>f__am$cache7;
        public bool isFirstDead;
        public uint sourceSwapAvatarId;
        public uint targetSwapAvatarId;

        public LevelTutorialSwapAndRestrain(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this.isFirstDead = false;
            this.targetSwapAvatarId = 0;
            this._killAmount = 0;
            this._monsterCreatedAmount = 0;
            this._isTutorialAvailable = false;
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

        private bool ListenAvatarCreated(EvtAvatarCreated evt)
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            localAvatar.onAttackTargetChanged = (Action<BaseMonoEntity>) Delegate.Combine(localAvatar.onAttackTargetChanged, new Action<BaseMonoEntity>(this.OnUpdateAttackTarget));
            return false;
        }

        public override bool ListenEvent(BaseEvent evt)
        {
            if (evt is EvtTutorialState)
            {
                return this.ListenTutorialState((EvtTutorialState) evt);
            }
            if (evt is EvtAvatarSwapInEnd)
            {
                return this.ListenSwapInEnd((EvtAvatarSwapInEnd) evt);
            }
            if (evt is EvtAvatarCreated)
            {
                return this.ListenAvatarCreated((EvtAvatarCreated) evt);
            }
            if (evt is EvtKilled)
            {
                return this.ListenKilled((EvtKilled) evt);
            }
            return ((evt is EvtMonsterCreated) && this.ListenMonsterCreated((EvtMonsterCreated) evt));
        }

        private bool ListenKilled(EvtKilled evt)
        {
            bool flag = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 4;
            bool flag2 = Singleton<RuntimeIDManager>.Instance.ParseCategory(evt.targetID) == 3;
            if (!flag || !flag2)
            {
                if (flag)
                {
                    this._killAmount++;
                    if (((this._killAmount == 4) && (base.step == 13)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
                    {
                        this.ActiveCurrentStep();
                        this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep13));
                    }
                }
                if (flag2)
                {
                    if ((base.step == 2) && !this.IsAllStepDone())
                    {
                        this.isFirstDead = true;
                        this.ActiveCurrentStep();
                        this.WaitShowTutorialStep(0f, new Action(this.ShowTutorialStep2));
                    }
                    if ((base.step == 13) && !this.IsAllStepDone())
                    {
                        this.Finish();
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
                    }
                }
            }
            return false;
        }

        private bool ListenMonsterCreated(EvtMonsterCreated evt)
        {
            this._monsterCreatedAmount++;
            return false;
        }

        private bool ListenSwapInEnd(EvtAvatarSwapInEnd evt)
        {
            this.targetSwapAvatarId = this.sourceSwapAvatarId;
            if ((this.IsInStep(10) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep10));
            }
            return false;
        }

        private bool ListenTutorialState(EvtTutorialState evt)
        {
            if ((evt != null) && (evt.state == EvtTutorialState.State.Start))
            {
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                if (((allPlayerAvatars.Count == 2) && (allPlayerAvatars[0].AvatarTypeName == "Kiana_C2_PT")) && (allPlayerAvatars[1].AvatarTypeName == "Mei_C2_CK"))
                {
                    this._isTutorialAvailable = true;
                }
                if (this._isTutorialAvailable)
                {
                    this.SetupAvatarId();
                }
                else
                {
                    this.Finish();
                }
            }
            return false;
        }

        public override void OnAdded()
        {
            Singleton<EventManager>.Instance.RegisterEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtAvatarSwapInEnd>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RegisterEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<EventManager>.Instance.RemoveEventListener<EvtTutorialState>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtStageReady>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtAvatarSwapInEnd>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtKilled>(base._helper.levelActor.runtimeID);
            Singleton<EventManager>.Instance.RemoveEventListener<EvtMonsterCreated>(base._helper.levelActor.runtimeID);
        }

        public void OnTutorialStep0Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(1) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
        }

        public void OnTutorialStep10Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(11) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep11));
            }
        }

        public void OnTutorialStep11Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(12) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep12));
            }
        }

        public void OnTutorialStep12Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep13Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep1Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        public void OnTutorialStep2Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(3) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep3));
            }
        }

        public void OnTutorialStep3Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(4) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep4));
            }
        }

        public void OnTutorialStep4Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(5) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep5));
            }
        }

        public void OnTutorialStep5Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(6) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep6));
            }
        }

        public void OnTutorialStep6Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(7) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep7));
            }
        }

        public void OnTutorialStep7Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(8) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep8));
            }
        }

        public void OnTutorialStep8Done()
        {
            this.MoveToNextStep();
            if ((this.IsInStep(9) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)) && !this.IsAllStepDone())
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep9));
            }
        }

        public void OnTutorialStep9Done()
        {
            this.ResumeGame();
            this.MoveToNextStep();
        }

        private void OnUpdateAttackTarget(BaseMonoEntity entity)
        {
            if ((entity != null) && this._isTutorialAvailable)
            {
                MonsterActor attackee = Singleton<EventManager>.Instance.GetActor<MonsterActor>(entity.GetRuntimeID());
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(localAvatar.GetRuntimeID());
                EntityNature attackerNature = (EntityNature) attackee.metaConfig.nature;
                EntityNature attribute = (EntityNature) actor.avatarDataItem.Attribute;
                float num = DamageModelLogic.GetNatureDamageBonusRatio(attackerNature, attribute, attackee);
                if (((((attackee != null) && (actor != null)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep))) && ((!this.IsAllStepDone() && (num > 1f)) && ((base.step == 0) && (base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep)))) && !this.IsAllStepDone())
                {
                    this.ActiveCurrentStep();
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep0));
                }
                if ((((attackee != null) && (actor != null)) && ((this._killAmount >= 2) && (this._monsterCreatedAmount > 2))) && (((num > 1f) && (base.step == 2)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone())))
                {
                    this.ActiveCurrentStep();
                    this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep2));
                }
            }
        }

        private void SetupAvatarId()
        {
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            foreach (BaseMonoAvatar avatar2 in Singleton<AvatarManager>.Instance.GetAllPlayerAvatars())
            {
                AvatarActor actor = Singleton<EventManager>.Instance.GetActor<AvatarActor>(avatar2.GetRuntimeID());
                if ((actor != null) && !Singleton<AvatarManager>.Instance.IsLocalAvatar(avatar2.GetRuntimeID()))
                {
                    this.targetSwapAvatarId = actor.runtimeID;
                }
            }
            this.sourceSwapAvatarId = localAvatar.GetRuntimeID();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
        }

        private void ShowTutorialStep0()
        {
            this.PauseGame();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep1()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep10()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
            if (<>f__am$cache7 == null)
            {
                <>f__am$cache7 = () => Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, false));
            }
            this.WaitShowTutorialStep(0.5f, <>f__am$cache7);
            this.PauseGame();
        }

        private void ShowTutorialStep11()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep12()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep13()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
            this.PauseGame();
        }

        private void ShowTutorialStep2()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
            this.PauseGame();
        }

        private void ShowTutorialStep3()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep4()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep5()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep6()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SwapBtnVisible, true));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep7()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep8()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }

        private void ShowTutorialStep9()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialSwapAndRestrain, this));
        }
    }
}

