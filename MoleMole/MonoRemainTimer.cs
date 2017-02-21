namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoRemainTimer : MonoBehaviour
    {
        private Transform _Day;
        private Transform _DayText;
        private bool _hideTime;
        private Transform _Hrs;
        private Transform _HrsText;
        private bool _isUpdating;
        private Transform _Min;
        private Transform _MinText;
        private Transform _Sec;
        private Transform _SecText;
        private DateTime _targetTime;
        private Action _timeoutCallBack;
        private int _timeSpan;
        private Action _updateAction;

        private void Awake()
        {
            this.InitTransforms();
        }

        private void InitTransforms()
        {
            if (this._Day == null)
            {
                this._Day = base.transform.Find("Day");
            }
            if (this._Hrs == null)
            {
                this._Hrs = base.transform.Find("Hrs");
            }
            if (this._Min == null)
            {
                this._Min = base.transform.Find("Min");
            }
            if (this._Sec == null)
            {
                this._Sec = base.transform.Find("Sec");
            }
            if (this._DayText == null)
            {
                this._DayText = base.transform.Find("DayText");
            }
            if (this._HrsText == null)
            {
                this._HrsText = base.transform.Find("HrsText");
            }
            if (this._MinText == null)
            {
                this._MinText = base.transform.Find("MinText");
            }
            if (this._SecText == null)
            {
                this._SecText = base.transform.Find("SecText");
            }
            if (this._hideTime)
            {
                this._Day.gameObject.SetActive(false);
                this._DayText.gameObject.SetActive(false);
                this._Hrs.gameObject.SetActive(false);
                this._HrsText.gameObject.SetActive(false);
                this._Min.gameObject.SetActive(false);
                this._MinText.gameObject.SetActive(false);
                this._Sec.gameObject.SetActive(false);
                this._SecText.gameObject.SetActive(false);
            }
        }

        private void SetRemainTime()
        {
            if (!this._hideTime)
            {
                TimeSpan span;
                if (this._isUpdating)
                {
                    span = (TimeSpan) (this._targetTime - TimeUtil.Now);
                }
                else
                {
                    span = new TimeSpan(0, 0, this._timeSpan);
                }
                this._Day.GetComponent<Text>().text = string.Format("{0:D2}", span.Days);
                this._Hrs.GetComponent<Text>().text = string.Format("{0:D2}", span.Hours);
                this._Min.GetComponent<Text>().text = string.Format("{0:D2}", span.Minutes);
                this._Sec.GetComponent<Text>().text = string.Format("{0:D2}", span.Seconds);
                this._Day.gameObject.SetActive(true);
                this._DayText.gameObject.SetActive(true);
                this._Hrs.gameObject.SetActive(true);
                this._HrsText.gameObject.SetActive(true);
                this._Min.gameObject.SetActive(true);
                this._MinText.gameObject.SetActive(true);
                this._Sec.gameObject.SetActive(true);
                this._SecText.gameObject.SetActive(true);
                if (span.TotalDays >= 1.0)
                {
                    this._Min.gameObject.SetActive(false);
                    this._MinText.gameObject.SetActive(false);
                    this._Sec.gameObject.SetActive(false);
                    this._SecText.gameObject.SetActive(false);
                }
                else if (span.TotalHours >= 1.0)
                {
                    this._Day.gameObject.SetActive(false);
                    this._DayText.gameObject.SetActive(false);
                    this._Sec.gameObject.SetActive(false);
                    this._SecText.gameObject.SetActive(false);
                }
                else
                {
                    this._Day.gameObject.SetActive(false);
                    this._DayText.gameObject.SetActive(false);
                    this._Hrs.gameObject.SetActive(false);
                    this._HrsText.gameObject.SetActive(false);
                }
            }
        }

        public void SetTargetTime(int timeSpan)
        {
            this.InitTransforms();
            this._timeSpan = timeSpan;
            this._isUpdating = false;
            this.SetRemainTime();
        }

        public void SetTargetTime(DateTime targetTime, Action updateAction = null, Action timeoutCallBack = null, bool hideTime = false)
        {
            this.InitTransforms();
            this._targetTime = targetTime;
            this._timeoutCallBack = timeoutCallBack;
            this._updateAction = updateAction;
            this._isUpdating = true;
            this._hideTime = hideTime;
            this.SetRemainTime();
        }

        private void Update()
        {
            if (this._isUpdating)
            {
                if (TimeUtil.Now > this._targetTime)
                {
                    this._isUpdating = false;
                    if (this._timeoutCallBack != null)
                    {
                        this._timeoutCallBack();
                    }
                }
                this.SetRemainTime();
                if (this._updateAction != null)
                {
                    this._updateAction();
                }
            }
        }
    }
}

