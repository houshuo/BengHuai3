namespace MoleMole
{
    using proto;
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoAppliedEndlessItem : MonoBehaviour
    {
        private bool _fillDirectionReverse;
        private bool _isUpdating;
        private EndlessPlayerData _selfEndlessData;
        private DateTime _targetTime;
        private Action<int> _timeEndCallBack;
        private int _timeSpan;
        private EndlessToolDataItem _toolData;
        public Image timerMask;

        private void OnEndlessToolTimeOut(int itemID)
        {
            <OnEndlessToolTimeOut>c__AnonStorey10B storeyb = new <OnEndlessToolTimeOut>c__AnonStorey10B {
                itemID = itemID
            };
            this._selfEndlessData.get_wait_effect_item_list().RemoveAll(new Predicate<EndlessWaitEffectItem>(storeyb.<>m__1BF));
            this._selfEndlessData.get_wait_burst_bomb_list().RemoveAll(new Predicate<EndlessWaitBurstBomb>(storeyb.<>m__1C0));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessAppliedToolChange, null));
        }

        public void SetupView(EndlessToolDataItem itemData)
        {
            this._toolData = itemData;
            base.transform.Find("VerticalLayout/Icon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._toolData.GetSmallIconPath());
            base.transform.Find("VerticalLayout/TopLine/NameText").GetComponent<Text>().text = this._toolData.GetDisplayTitle();
            base.transform.Find("VerticalLayout/AbstractText").GetComponent<Text>().text = this._toolData.GetDescription();
            bool flag = false;
            this._selfEndlessData = Singleton<EndlessModule>.Instance.GetSelfEndlessData();
            if ((itemData.ToolType == 5) && this._selfEndlessData.get_hidden_expire_timeSpecified())
            {
                this._targetTime = Miscs.GetDateTimeFromTimeStamp(this._selfEndlessData.get_hidden_expire_time());
                this._timeSpan = itemData.GetTimeSpanInSeconds();
                this._fillDirectionReverse = true;
                this._isUpdating = true;
                this._timeEndCallBack = new Action<int>(this.OnEndlessToolTimeOut);
                flag = true;
            }
            if (!flag)
            {
                foreach (EndlessWaitBurstBomb bomb in this._selfEndlessData.get_wait_burst_bomb_list())
                {
                    if (bomb.get_item_id() == itemData.ID)
                    {
                        this._targetTime = Miscs.GetDateTimeFromTimeStamp(bomb.get_burst_time());
                        this._timeSpan = itemData.GetTimeSpanInSeconds();
                        this._fillDirectionReverse = false;
                        this._isUpdating = true;
                        this._timeEndCallBack = null;
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                foreach (EndlessWaitEffectItem item in this._selfEndlessData.get_wait_effect_item_list())
                {
                    if (item.get_item_id() == itemData.ID)
                    {
                        this._targetTime = Miscs.GetDateTimeFromTimeStamp(item.get_expire_time());
                        this._timeSpan = itemData.GetTimeSpanInSeconds();
                        this._fillDirectionReverse = true;
                        this._isUpdating = item.get_expire_timeSpecified();
                        this._timeEndCallBack = new Action<int>(this.OnEndlessToolTimeOut);
                        flag = true;
                        break;
                    }
                }
            }
            this.timerMask.gameObject.SetActive(this._isUpdating);
            base.transform.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(itemData.GetSmallIconPath());
            this.timerMask.gameObject.SetActive(this._timeSpan > 0);
            this.timerMask.fillAmount = 0f;
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

        [CompilerGenerated]
        private sealed class <OnEndlessToolTimeOut>c__AnonStorey10B
        {
            internal int itemID;

            internal bool <>m__1BF(EndlessWaitEffectItem item)
            {
                return (item.get_item_id() == this.itemID);
            }

            internal bool <>m__1C0(EndlessWaitBurstBomb item)
            {
                return (item.get_item_id() == this.itemID);
            }
        }
    }
}

