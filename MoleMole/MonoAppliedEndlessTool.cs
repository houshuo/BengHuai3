namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAppliedEndlessTool : MonoBehaviour
    {
        private bool _fillDirectionReverse;
        private bool _isUpdating;
        private DateTime _targetTime;
        private Action<int> _timeEndCallBack;
        private int _timeSpan;
        private EndlessToolDataItem _toolData;
        public Image timerMask;

        public void SetupView(EndlessToolDataItem toolData, uint timestamp, Action<int> endCallBack)
        {
            this._toolData = toolData;
            if (timestamp == 0)
            {
                this.timerMask.gameObject.SetActive(false);
                this._isUpdating = false;
            }
            else
            {
                this._targetTime = Miscs.GetDateTimeFromTimeStamp(timestamp);
                this._timeEndCallBack = endCallBack;
                this._timeSpan = this._toolData.GetTimeSpanInSeconds();
                this._isUpdating = this._timeSpan > 0;
                this._fillDirectionReverse = toolData.ToolType != 4;
                base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(toolData.GetSmallIconPath());
                this.timerMask.gameObject.SetActive(this._timeSpan > 0);
                this.timerMask.fillAmount = 0f;
            }
        }

        private void Update()
        {
            if (this._isUpdating)
            {
                if (TimeUtil.Now > this._targetTime)
                {
                    if (this._timeEndCallBack != null)
                    {
                        this._timeEndCallBack(this._toolData.ID);
                    }
                    this._isUpdating = false;
                }
                TimeSpan span = (TimeSpan) (this._targetTime - TimeUtil.Now);
                float num = Mathf.Clamp01(((float) span.TotalSeconds) / ((float) this._timeSpan));
                if (this.timerMask != null)
                {
                    this.timerMask.fillAmount = !this._fillDirectionReverse ? num : (1f - num);
                }
            }
        }
    }
}

