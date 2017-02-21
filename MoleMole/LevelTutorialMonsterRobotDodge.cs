namespace MoleMole
{
    using FullInspector;
    using System;
    using UnityEngine;

    public class LevelTutorialMonsterRobotDodge : BaseLevelTutorial
    {
        private string _aniamtorDodgeName;
        [ShowInInspector]
        private bool _finished;

        public LevelTutorialMonsterRobotDodge(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData) : base(helper, metaData)
        {
            this._finished = false;
            this._aniamtorDodgeName = "SwordReflect";
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
            localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.OnLocalAvatarAnimatorStateChnage));
            return false;
        }

        private bool ListenAvatarSwapIn(EvtAvatarSwapInEnd evt)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Combine(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.OnLocalAvatarAnimatorStateChnage));
            }
            return false;
        }

        private bool ListenAvatarSwapOut(EvtAvatarSwapOutStart evt)
        {
            if (Singleton<AvatarManager>.Instance.IsLocalAvatar(evt.targetID))
            {
                BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
                localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.OnLocalAvatarAnimatorStateChnage));
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
                return this.ListenAvatarSwapIn((EvtAvatarSwapInEnd) evt);
            }
            return ((evt is EvtAvatarSwapOutStart) && this.ListenAvatarSwapOut((EvtAvatarSwapOutStart) evt));
        }

        public override void OnAdded()
        {
        }

        public override void OnDecided()
        {
            base.OnDecided();
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterRobotDodge, this));
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            localAvatar.onAnimatorStateChanged = (Action<AnimatorStateInfo, AnimatorStateInfo>) Delegate.Remove(localAvatar.onAnimatorStateChanged, new Action<AnimatorStateInfo, AnimatorStateInfo>(this.OnLocalAvatarAnimatorStateChnage));
        }

        private void OnLocalAvatarAnimatorStateChnage(AnimatorStateInfo fromState, AnimatorStateInfo toState)
        {
            if ((toState.IsName(this._aniamtorDodgeName) && (base.step == 0)) && ((base.GetCurrentStepState() == BaseLevelTutorial.StepState.Sleep) && !this.IsAllStepDone()))
            {
                this.ActiveCurrentStep();
                this.WaitShowTutorialStep(this.GetDelayTime(base.step), new Action(this.ShowTutorialStep1));
            }
        }

        public void OnTutoriaStep1Done()
        {
            this.MoveToNextStep();
            this.ResumeGame();
        }

        private void ShowTutorialStep1()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.TutorialMonsterRobotDodge, this));
            this.PauseGame();
        }
    }
}

