namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class SequenceDialogManager
    {
        private bool _bIsPlaying;
        private List<BaseSequenceDialogContext> _dialogList = new List<BaseSequenceDialogContext>();
        private int _index = 0;
        private Action _onAllDialogDestroy;
        private CanvasTimer _startDelayTimer;

        public SequenceDialogManager(Action onAllDialogDestroyCallBack = null)
        {
            this._onAllDialogDestroy = onAllDialogDestroyCallBack;
            this._bIsPlaying = false;
        }

        public void AddAsFirstDialog(BaseSequenceDialogContext dialogContext)
        {
            dialogContext.SetDestroyCallBack(new Action(this.OnDialogDestroy));
            this._dialogList.Insert(0, dialogContext);
        }

        public void AddDialog(BaseSequenceDialogContext dialogContext)
        {
            dialogContext.SetDestroyCallBack(new Action(this.OnDialogDestroy));
            this._dialogList.Add(dialogContext);
        }

        public void ClearDialogs()
        {
            this._dialogList.Clear();
        }

        private void DoStartShow()
        {
            this._index = 0;
            this._bIsPlaying = true;
            this.ShowNext();
        }

        public BaseSequenceDialogContext GetDialog(int index)
        {
            if ((index >= 0) && (index < this._dialogList.Count))
            {
                return this._dialogList[index];
            }
            return null;
        }

        public int GetDialogNum()
        {
            return this._dialogList.Count;
        }

        public bool IsPlaying()
        {
            return this._bIsPlaying;
        }

        private void OnDialogDestroy()
        {
            this._index++;
            this.ShowNext();
        }

        private void ShowNext()
        {
            if (this._index < this._dialogList.Count)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(this._dialogList[this._index], UIType.Any);
            }
            else
            {
                this._bIsPlaying = false;
                if (this._onAllDialogDestroy != null)
                {
                    this._onAllDialogDestroy();
                }
            }
        }

        public void StartShow(float startDelay = 0f)
        {
            if (Mathf.Approximately(startDelay, 0f))
            {
                this.DoStartShow();
            }
            else if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
            {
                this._startDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(startDelay, 0f);
                this._startDelayTimer.timeUpCallback = new Action(this.DoStartShow);
            }
        }
    }
}

