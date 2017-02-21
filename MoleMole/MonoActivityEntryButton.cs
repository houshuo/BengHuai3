namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonoActivityEntryButton : MonoBehaviour
    {
        private ActivityDataItemBase _activityData;

        private void Awake()
        {
            base.transform.Find("Button").GetComponent<Button>().onClick.AddListener(new UnityAction(this.OnClick));
        }

        public void OnClick()
        {
            MonoChapterScroller component = base.transform.parent.parent.GetComponent<MonoChapterScroller>();
            if (!component.IsCenter(base.transform))
            {
                component.ClickToChangeCenter(base.transform);
            }
            else
            {
                switch (this._activityData.GetStatus())
                {
                    case ActivityDataItemBase.Status.Locked:
                    {
                        object[] replaceParams = new object[] { this._activityData.GetMinPlayerLevelLimit() };
                        Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(LocalizationGeneralLogic.GetText("Menu_ActivityLock", replaceParams), 2f), UIType.Any);
                        return;
                    }
                    case ActivityDataItemBase.Status.WaitToStart:
                        return;

                    case ActivityDataItemBase.Status.InProgress:
                        if (!(this._activityData is WeekDayActivityDataItem))
                        {
                            if (this._activityData is EndlessActivityDataItem)
                            {
                                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.RequestEnterEndlessActivity, null));
                            }
                            break;
                        }
                        Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(this._activityData as WeekDayActivityDataItem), UIType.Page);
                        break;

                    default:
                        return;
                }
            }
        }

        public void SetupView(ActivityDataItemBase activityData)
        {
            this._activityData = activityData;
            base.transform.Find("Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._activityData.GetActivityEnterImgPath());
            base.transform.Find("LeftTime").gameObject.SetActive(false);
            base.transform.Find("Icon/Status/Over").gameObject.SetActive(false);
            base.transform.Find("Icon/Status/Lock").gameObject.SetActive(false);
            base.transform.Find("Icon/Status/WaitToStart").gameObject.SetActive(false);
            string str = null;
            switch (this._activityData.GetStatus())
            {
                case ActivityDataItemBase.Status.Over:
                    base.transform.Find("Icon/Status/Over").gameObject.SetActive(true);
                    break;

                case ActivityDataItemBase.Status.Locked:
                    base.transform.Find("Icon/Status/Lock").gameObject.SetActive(true);
                    str = "UI_Gen_Select_Negative";
                    break;

                case ActivityDataItemBase.Status.WaitToStart:
                    base.transform.Find("Icon/Status/WaitToStart").gameObject.SetActive(true);
                    base.transform.Find("LeftTime").gameObject.SetActive(true);
                    break;

                case ActivityDataItemBase.Status.InProgress:
                    base.transform.Find("LeftTime").gameObject.SetActive(!(activityData is EndlessActivityDataItem));
                    break;
            }
            if (!string.IsNullOrEmpty(str))
            {
                MonoButtonWwiseEvent component = base.transform.Find("Button").GetComponent<MonoButtonWwiseEvent>();
                if (component == null)
                {
                    component = base.transform.Find("Button").gameObject.AddComponent<MonoButtonWwiseEvent>();
                }
                if (component != null)
                {
                    component.eventName = str;
                }
            }
        }

        public void Update()
        {
            Transform transform = base.transform.Find("LeftTime");
            if (transform.gameObject.activeSelf)
            {
                ActivityDataItemBase.Status status = this._activityData.GetStatus();
                if (status == ActivityDataItemBase.Status.WaitToStart)
                {
                    string str;
                    transform.Find("TimeValue").GetComponent<Text>().text = Miscs.GetTimeSpanToShow(this._activityData.beginTime, out str).ToString();
                    transform.Find("Label").GetComponent<Text>().text = str;
                }
                else if (status == ActivityDataItemBase.Status.InProgress)
                {
                    string str2;
                    transform.Find("TimeValue").GetComponent<Text>().text = Miscs.GetTimeSpanToShow(this._activityData.endTime, out str2).ToString();
                    transform.Find("Label").GetComponent<Text>().text = str2;
                }
                else
                {
                    transform.gameObject.SetActive(false);
                }
            }
        }

        public void UpdateView(bool isSelected)
        {
            this.selected = isSelected;
            base.transform.Find("Icon/Mask").gameObject.SetActive(!isSelected);
            base.transform.Find("Arrow").gameObject.SetActive(isSelected);
            base.transform.Find("Frame").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Top").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Bottom").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Left").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
            base.transform.Find("Frame/Right").GetComponent<Image>().color = !isSelected ? MiscData.GetColor("Black") : MiscData.GetColor("Blue");
        }

        public bool selected { get; private set; }
    }
}

