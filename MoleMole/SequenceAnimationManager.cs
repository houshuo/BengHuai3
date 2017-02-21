namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SequenceAnimationManager
    {
        private List<MonoAnimationinSequence> _animationList = new List<MonoAnimationinSequence>();
        private int _index = 0;
        private int _lastIndex = -1;
        private bool _lockUI;
        private Action _onAllAnimationEnd;
        private Action<Transform> _onEveryAnimation;
        private CanvasTimer _startDelayTimer;
        public bool IsPlaying;

        public SequenceAnimationManager(Action allAnimationEndCallBack = null, Action<Transform> onEveryAnimation = null)
        {
            this._onAllAnimationEnd = allAnimationEndCallBack;
            this._onEveryAnimation = onEveryAnimation;
        }

        public void AddAllChildrenInTransform(Transform trans)
        {
            IEnumerator enumerator = trans.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MonoAnimationinSequence component = ((Transform) enumerator.Current).GetComponent<MonoAnimationinSequence>();
                    if (component != null)
                    {
                        this.AddAnimation(component, null);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        public void AddAnimation(MonoAnimationinSequence animation, Action<Transform> endCallBack = null)
        {
            animation.SetAnimationEndCallBack(new MoleMole.OnAnimationEnd(this.OnAnimationEnd), endCallBack);
            animation.TryResetToAnimationFirstFrame();
            this._animationList.Add(animation);
        }

        public void ClearAnimations()
        {
            this._animationList.Clear();
        }

        private void DoStarPlay()
        {
            this.IsPlaying = true;
            Singleton<MainUIManager>.Instance.LockUI(this._lockUI, 3f);
            this._index = 0;
            this._lastIndex = -1;
            this.PlayNext();
        }

        private void OnAnimationEnd()
        {
            this._lastIndex = this._index;
            this._index++;
            this.PlayNext();
        }

        private void PlayNext()
        {
            if ((this._lastIndex > 0) && (this._onEveryAnimation != null))
            {
                this._onEveryAnimation(this._animationList[this._lastIndex].transform);
            }
            while ((this._index < this._animationList.Count) && ((this._animationList[this._index] == null) || !this._animationList[this._index].gameObject.activeSelf))
            {
                this._index++;
            }
            if (((this._index < this._animationList.Count) && (this._animationList[this._index] != null)) && this._animationList[this._index].gameObject.activeSelf)
            {
                this._animationList[this._index].Play();
            }
            else
            {
                Singleton<MainUIManager>.Instance.LockUI(false, 3f);
                if (this._onAllAnimationEnd != null)
                {
                    this._onAllAnimationEnd();
                }
                this.IsPlaying = false;
            }
        }

        public void StartPlay(float startDelay = 0f, bool lockUI = true)
        {
            this._lockUI = lockUI;
            if (Mathf.Approximately(startDelay, 0f))
            {
                this.DoStarPlay();
            }
            else
            {
                this._startDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(startDelay, 0f);
                this._startDelayTimer.timeUpCallback = new Action(this.DoStarPlay);
            }
        }
    }
}

