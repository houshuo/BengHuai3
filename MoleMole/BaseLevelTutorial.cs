namespace MoleMole
{
    using FullInspector;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [fiInspectorOnly]
    public abstract class BaseLevelTutorial
    {
        protected List<string> _displayList;
        protected LevelTutorialHelperPlugin _helper;
        protected int _maxStepCount;
        protected LevelTutorialMetaData _metaData;
        protected List<StepState> _stepStateList;
        public bool active;
        public readonly int tutorialId;

        public BaseLevelTutorial(LevelTutorialHelperPlugin helper, LevelTutorialMetaData metaData)
        {
            this._helper = helper;
            this._metaData = metaData;
            this.tutorialId = this._metaData.tutorialId;
            this._stepStateList = new List<StepState>();
            this._displayList = new List<string>();
            this.step = 0;
            this._maxStepCount = this._metaData.paramList.Count;
            for (int i = 0; i < this._maxStepCount; i++)
            {
                this._stepStateList.Add(StepState.Sleep);
                this._displayList.Add(this._metaData.diaplayTarget[i]);
            }
            this.active = true;
        }

        public virtual void ActiveCurrentStep()
        {
            if ((this.step < this._maxStepCount) && (((StepState) this._stepStateList[this.step]) == StepState.Sleep))
            {
                this._stepStateList[this.step] = StepState.Active;
            }
        }

        public virtual void Core()
        {
        }

        public virtual void DoneCurrentStep()
        {
            if (this.step < this._maxStepCount)
            {
                this._stepStateList[this.step] = StepState.Done;
            }
        }

        public StepState GetCurrentStepState()
        {
            if (this.step < this._maxStepCount)
            {
                return this._stepStateList[this.step];
            }
            return StepState.Sleep;
        }

        public virtual float GetDelayTime(int stepIndex)
        {
            if ((stepIndex >= 0) && (stepIndex < this._maxStepCount))
            {
                return this._metaData.paramList[stepIndex];
            }
            return 0f;
        }

        public virtual string GetDisplayTarget(int stepIndex)
        {
            if ((stepIndex >= 0) && (stepIndex < this._maxStepCount))
            {
                return LocalizationGeneralLogic.GetText(this._displayList[stepIndex], new object[0]);
            }
            return string.Empty;
        }

        public virtual StepState GetStepState(int stepIndex)
        {
            if (stepIndex < this._maxStepCount)
            {
                return this._stepStateList[stepIndex];
            }
            return StepState.Sleep;
        }

        public virtual bool IsAllStepDone()
        {
            foreach (StepState state in this._stepStateList)
            {
                if (state != StepState.Done)
                {
                    return false;
                }
            }
            return true;
        }

        public abstract bool IsFinished();
        public virtual bool IsInStep(int stepIndex)
        {
            return (this.step == stepIndex);
        }

        public virtual bool ListenEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void MoveToNextStep()
        {
            if (this.step < this._maxStepCount)
            {
                this._stepStateList[this.step] = StepState.Done;
                this.step++;
            }
        }

        public virtual void NotifyStep(NotifyTypes notifyType)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(notifyType, this));
        }

        public virtual void OnAdded()
        {
        }

        public virtual void OnDecided()
        {
            this.active = false;
        }

        public virtual bool OnEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual bool OnPostEvent(BaseEvent evt)
        {
            return false;
        }

        public virtual void PauseGame()
        {
            Singleton<LevelManager>.Instance.SetTutorialTimeScale(0f);
        }

        public virtual void ResetToStep(int stepIndex)
        {
            if ((stepIndex < this._maxStepCount) && (stepIndex >= 0))
            {
                this.step = stepIndex;
                for (int i = 0; i < this._maxStepCount; i++)
                {
                    this._stepStateList[i] = (i >= stepIndex) ? StepState.Sleep : StepState.Done;
                }
            }
        }

        public virtual void ResumeGame()
        {
            Singleton<LevelManager>.Instance.SetTutorialTimeScale(1f);
        }

        public virtual void SetControllerEnable(bool enable)
        {
            Singleton<AvatarManager>.Instance.SetMuteAllAvatarControl(!enable);
        }

        [DebuggerHidden]
        private IEnumerator WaitShowStep(float delay, Action callback)
        {
            return new <WaitShowStep>c__Iterator1 { delay = delay, callback = callback, <$>delay = delay, <$>callback = callback };
        }

        public virtual void WaitShowTutorialStep(float delay, Action callback)
        {
            if (delay == 0f)
            {
                callback();
            }
            else
            {
                Singleton<LevelManager>.Instance.levelEntity.StartCoroutine(this.WaitShowStep(delay, callback));
            }
        }

        [ShowInInspector]
        public int step { get; set; }

        [CompilerGenerated]
        private sealed class <WaitShowStep>c__Iterator1 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action <$>callback;
            internal float <$>delay;
            internal Action callback;
            internal float delay;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForSeconds(this.delay);
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.callback();
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }

        public enum StepState
        {
            Sleep,
            Active,
            Done
        }
    }
}

