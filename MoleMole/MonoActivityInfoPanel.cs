namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoActivityInfoPanel : MonoBehaviour
    {
        private WeekDayActivityDataItem _activityData;
        private GameObject _contentGameObject;
        private GameObject _greyGameObject;
        private Text _inProgressDescText;
        private GameObject _inProgressGameObject;
        private Text _labelText;
        private Text _leftEnterTimeText;
        private GameObject _leftTimeGameObject;
        private Text _lockDescText;
        private GameObject _lockGameObject;
        private Text _milliSecondText;
        private Text _minuteText;
        private GameObject _normalGameObject;
        private GameObject _nuclearExPanelGameObject;
        private GameObject _nuclearExPanelLeftTimeGameObject;
        private GameObject _overGameObject;
        private Text _secondText;
        private Text _timeValueText;
        private Text _waitToStartDescText;
        private GameObject _waitToStartGameObject;

        private void Awake()
        {
            this._timeValueText = base.transform.Find("Content/WaitToStart/LeftTime/TimeValue").GetComponent<Text>();
            this._labelText = base.transform.Find("Content/WaitToStart/LeftTime/Label").GetComponent<Text>();
            this._inProgressDescText = base.transform.Find("Content/InProgress/Desc").GetComponent<Text>();
            this._waitToStartDescText = base.transform.Find("Content/WaitToStart/Desc").GetComponent<Text>();
            this._lockDescText = base.transform.Find("Content/Lock/Desc").GetComponent<Text>();
            this._leftEnterTimeText = base.transform.Find("Content/InProgress/LeftEnterTime/Text").GetComponent<Text>();
            this._minuteText = base.transform.Find("NuclearExPanel/LeftTime/Minute").GetComponent<Text>();
            this._secondText = base.transform.Find("NuclearExPanel/LeftTime/Second").GetComponent<Text>();
            this._milliSecondText = base.transform.Find("NuclearExPanel/LeftTime/MilliSecond").GetComponent<Text>();
            this._contentGameObject = base.transform.Find("Content").gameObject;
            this._nuclearExPanelGameObject = base.transform.Find("NuclearExPanel").gameObject;
            this._lockGameObject = base.transform.Find("Content/Lock").gameObject;
            this._waitToStartGameObject = base.transform.Find("Content/WaitToStart").gameObject;
            this._overGameObject = base.transform.Find("Content/Over").gameObject;
            this._inProgressGameObject = base.transform.Find("Content/InProgress").gameObject;
            this._leftTimeGameObject = base.transform.Find("Content/WaitToStart/LeftTime").gameObject;
            this._normalGameObject = base.transform.Find("Title/Normal").gameObject;
            this._greyGameObject = base.transform.Find("Title/Grey").gameObject;
            this._nuclearExPanelLeftTimeGameObject = base.transform.Find("NuclearExPanel/LeftTime").gameObject;
            this._inProgressGameObject.transform.Find("StaminaCost").gameObject.SetActive(false);
        }

        private void SetupDefaultActivityInfo()
        {
            this._contentGameObject.SetActive(true);
            this._nuclearExPanelGameObject.SetActive(false);
            this.SetupTitleView(this._activityData);
            this._lockGameObject.SetActive(false);
            this._waitToStartGameObject.SetActive(false);
            this._overGameObject.SetActive(false);
            this._inProgressGameObject.SetActive(false);
            switch (this._activityData.GetStatus())
            {
                case ActivityDataItemBase.Status.Unavailable:
                    this._waitToStartGameObject.SetActive(true);
                    this._waitToStartDescText.text = this._activityData.GetActivityLockDescription();
                    this._leftTimeGameObject.SetActive(false);
                    break;

                case ActivityDataItemBase.Status.Over:
                    this._overGameObject.SetActive(true);
                    break;

                case ActivityDataItemBase.Status.Locked:
                {
                    this._lockGameObject.SetActive(true);
                    object[] replaceParams = new object[] { this._activityData.GetMinPlayerLevelLimit() };
                    this._lockDescText.text = LocalizationGeneralLogic.GetText("Menu_ActivityLock", replaceParams);
                    break;
                }
                case ActivityDataItemBase.Status.WaitToStart:
                    string str;
                    this._waitToStartGameObject.SetActive(true);
                    this._waitToStartDescText.text = this._activityData.GetActivityDescription();
                    this._timeValueText.text = Miscs.GetTimeSpanToShow(this._activityData.beginTime, out str).ToString();
                    this._labelText.text = str;
                    break;

                case ActivityDataItemBase.Status.InProgress:
                    this._inProgressGameObject.SetActive(true);
                    this._inProgressDescText.text = this._activityData.GetActivityDescription();
                    this._leftEnterTimeText.text = (this._activityData.maxEnterTimes - this._activityData.enterTimes).ToString();
                    break;
            }
        }

        private void SetupNuclearActivityInfo()
        {
            this._contentGameObject.SetActive(false);
            this._nuclearExPanelGameObject.SetActive(true);
            this.SetupNuclearCountDown();
        }

        private void SetupNuclearCountDown()
        {
            TimeSpan span = this._activityData.endTime.Subtract(TimeUtil.Now);
            if (span < TimeSpan.Zero)
            {
                this._nuclearExPanelLeftTimeGameObject.SetActive(false);
            }
            else
            {
                this._nuclearExPanelLeftTimeGameObject.SetActive(true);
                this._minuteText.text = string.Format("{0:D2}", (int) span.TotalMinutes);
                this._secondText.text = string.Format("{0:D2}", span.Seconds);
                this._milliSecondText.text = string.Format("{0:D3}", span.Milliseconds);
            }
        }

        private void SetupTitleView(WeekDayActivityDataItem activityData)
        {
            bool flag = activityData.GetStatus() == ActivityDataItemBase.Status.InProgress;
            this._normalGameObject.SetActive(flag);
            this._greyGameObject.SetActive(!flag);
        }

        public void SetupView(WeekDayActivityDataItem activityData)
        {
            this._activityData = activityData;
            base.gameObject.SetActive(true);
            if (this._activityData.GetActivityType() == 3)
            {
                this.SetupNuclearActivityInfo();
            }
            else
            {
                this.SetupDefaultActivityInfo();
            }
        }

        public void Update()
        {
            if (this._activityData != null)
            {
                if (this._activityData.GetStatus() == ActivityDataItemBase.Status.WaitToStart)
                {
                    string str;
                    this._timeValueText.text = Miscs.GetTimeSpanToShow(this._activityData.beginTime, out str).ToString();
                    this._labelText.text = str;
                }
                if (this._activityData.GetActivityType() == 3)
                {
                    if (this._activityData.GetStatus() == ActivityDataItemBase.Status.InProgress)
                    {
                        this.SetupNuclearCountDown();
                    }
                    else
                    {
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RefreshChapterSelectPage, null));
                    }
                }
            }
        }
    }
}

