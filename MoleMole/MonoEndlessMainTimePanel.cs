namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoEndlessMainTimePanel : MonoBehaviour
    {
        private EndlessActivityStatus _endlessStatus;
        [SerializeField]
        private Transform _inProgressLabel;
        [SerializeField]
        private Transform _inProgressTime;
        private float _intervalTimer;
        [SerializeField]
        private Transform _prepareLabel;
        [SerializeField]
        private Transform _prepareTime;
        [SerializeField]
        private Transform _settleLabel;
        [SerializeField]
        private Transform _settleTime;
        private float _updateInterval = 60f;
        private const string CURRENT_STAGE_COLOR = "EndlessCurrentStageColor";
        private const string POST_STAGE_COLOR = "EndlessPostStageColor";
        private const string PRE_STAGE_COLOR = "EndlessPreStageColor";

        private void SetTheRemainTime(Transform timeTrans, Transform labelTrans, TimeSpan timeSpan, EndlessStage stage = 1)
        {
            Vector3 vector = new Vector3(1f, 1f, 1f);
            Color color = MiscData.GetColor("EndlessCurrentStageColor");
            switch (stage)
            {
                case EndlessStage.Pre:
                    vector = new Vector3(0.9f, 0.9f, 0.9f);
                    color = MiscData.GetColor("EndlessPreStageColor");
                    break;

                case EndlessStage.Post:
                    vector = new Vector3(0.9f, 0.9f, 0.9f);
                    color = MiscData.GetColor("EndlessPostStageColor");
                    break;
            }
            timeTrans.localScale = vector;
            labelTrans.localScale = vector;
            labelTrans.GetComponent<Text>().color = color;
            timeTrans.Find("Day").gameObject.SetActive(timeSpan.Days > 0);
            timeTrans.Find("DayText").gameObject.SetActive(timeSpan.Days > 0);
            timeTrans.Find("Day").GetComponent<Text>().text = timeSpan.Days.ToString();
            timeTrans.Find("Day").GetComponent<Text>().color = color;
            timeTrans.Find("DayText").GetComponent<Text>().color = color;
            timeTrans.Find("Hrs").gameObject.SetActive(true);
            timeTrans.Find("HrsText").gameObject.SetActive(true);
            timeTrans.Find("Hrs").GetComponent<Text>().text = string.Format("{0:D2}", timeSpan.Hours);
            timeTrans.Find("Hrs").GetComponent<Text>().color = color;
            timeTrans.Find("HrsText").GetComponent<Text>().color = color;
            timeTrans.Find("Min").gameObject.SetActive(true);
            timeTrans.Find("MinText").gameObject.SetActive(true);
            timeTrans.Find("Min").GetComponent<Text>().text = ((timeSpan.Minutes > 0) || (timeSpan.Seconds <= 0)) ? string.Format("{0:D2}", timeSpan.Minutes) : "01";
            timeTrans.Find("Min").GetComponent<Text>().color = color;
            timeTrans.Find("MinText").GetComponent<Text>().color = color;
            timeTrans.Find("Sec").gameObject.SetActive(false);
            timeTrans.Find("SecText").gameObject.SetActive(false);
            timeTrans.Find("Sec").GetComponent<Text>().color = color;
            timeTrans.Find("SecText").GetComponent<Text>().color = color;
        }

        private void Start()
        {
            this._endlessStatus = Singleton<EndlessModule>.Instance.GetEndlessActivityStatus();
            this._intervalTimer = this._updateInterval;
        }

        private void Update()
        {
            this._intervalTimer += Time.deltaTime;
            if (this._intervalTimer > this._updateInterval)
            {
                this.UpdateTimePanel();
                this._intervalTimer = 0f;
            }
        }

        private void UpdateTimePanel()
        {
            EndlessActivityStatus status = this._endlessStatus;
            this._endlessStatus = Singleton<EndlessModule>.Instance.GetEndlessActivityStatus();
            switch (this._endlessStatus)
            {
                case EndlessActivityStatus.WaitToStart:
                    if (this._endlessStatus != status)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivitySettlement, null));
                    }
                    this.SetTheRemainTime(this._prepareTime, this._prepareLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.BeginTime - TimeUtil.Now), EndlessStage.Current);
                    this.SetTheRemainTime(this._inProgressTime, this._inProgressLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.EndTime - Singleton<EndlessModule>.Instance.BeginTime), EndlessStage.Post);
                    this.SetTheRemainTime(this._settleTime, this._settleLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.SettlementTime - Singleton<EndlessModule>.Instance.EndTime), EndlessStage.Post);
                    break;

                case EndlessActivityStatus.InProgress:
                    if (this._endlessStatus != status)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivityBegin, null));
                    }
                    this.SetTheRemainTime(this._prepareTime, this._prepareLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
                    this.SetTheRemainTime(this._inProgressTime, this._inProgressLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.EndTime - TimeUtil.Now), EndlessStage.Current);
                    this.SetTheRemainTime(this._settleTime, this._settleLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.SettlementTime - Singleton<EndlessModule>.Instance.EndTime), EndlessStage.Post);
                    break;

                case EndlessActivityStatus.WaitToSettlement:
                    if (this._endlessStatus != status)
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessActivityEnd, null));
                    }
                    this.SetTheRemainTime(this._prepareTime, this._prepareLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
                    this.SetTheRemainTime(this._inProgressTime, this._inProgressLabel, new TimeSpan(0, 0, 0, 0), EndlessStage.Pre);
                    this.SetTheRemainTime(this._settleTime, this._settleLabel, (TimeSpan) (Singleton<EndlessModule>.Instance.SettlementTime - TimeUtil.Now), EndlessStage.Current);
                    break;
            }
        }

        public enum EndlessStage
        {
            Pre,
            Current,
            Post
        }
    }
}

