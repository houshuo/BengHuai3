namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoRankButton : MonoBehaviour
    {
        private float _checkTimer = 60f;
        private Action<EndlessPlayerData> _clickCallBack;
        private EndlessPlayerData _endlessPlayerData;
        private DateTime _frozenEndTime;
        private bool _interactable;
        private EndlessToolDataItem _selectToolData;
        private const float CHECK_INTERVAL = 60f;
        private const int MAX_NUM_TOOLS_TO_SHOW = 5;

        private void CheckInteractable()
        {
            if (!this._interactable || (this._endlessPlayerData == null))
            {
                base.transform.Find("BlockPanel").gameObject.SetActive(true);
            }
            else
            {
                bool flag = true;
                DateTime dateTimeFromTimeStamp = TimeUtil.Now.AddDays(-1.0);
                if (this._endlessPlayerData.get_hidden_expire_timeSpecified())
                {
                    dateTimeFromTimeStamp = Miscs.GetDateTimeFromTimeStamp(this._endlessPlayerData.get_hidden_expire_time());
                }
                if (this._endlessPlayerData.get_uid() == Singleton<PlayerModule>.Instance.playerData.userId)
                {
                    flag = false;
                    base.transform.Find("FrozenInfo").gameObject.SetActive(false);
                }
                else if ((this._frozenEndTime > TimeUtil.Now) || (this._endlessPlayerData.get_hidden_expire_timeSpecified() && (dateTimeFromTimeStamp > TimeUtil.Now)))
                {
                    flag = false;
                }
                if (((this._endlessPlayerData.get_progress() < 2) && (this._selectToolData != null)) && (this._selectToolData.ToolType == 4))
                {
                    flag = false;
                }
                if ((this._selectToolData != null) && (this._selectToolData.ToolType == 3))
                {
                    flag = false;
                }
                base.transform.Find("BlockPanel").gameObject.SetActive(!flag);
            }
        }

        public void OnClick()
        {
            if (this._clickCallBack != null)
            {
                this._clickCallBack(this._endlessPlayerData);
            }
        }

        private void SetFrozenTime()
        {
            this._frozenEndTime = Singleton<EndlessModule>.Instance.GetFrozenEndTime((int) this._endlessPlayerData.get_uid());
            base.transform.Find("FrozenInfo").gameObject.SetActive(this._frozenEndTime > TimeUtil.Now);
            if (this._frozenEndTime > TimeUtil.Now)
            {
                this.SetTheRemainTime((TimeSpan) (this._frozenEndTime - TimeUtil.Now));
            }
        }

        public void SetInteractable(bool interactable)
        {
            this._interactable = interactable;
            this.CheckInteractable();
        }

        private void SetTheRemainTime(TimeSpan timeSpan)
        {
            base.transform.Find("FrozenInfo/RemainTime/Time/Hrs").gameObject.SetActive(true);
            base.transform.Find("FrozenInfo/RemainTime/Time/HrsText").gameObject.SetActive(true);
            base.transform.Find("FrozenInfo/RemainTime/Time/Hrs").GetComponent<Text>().text = timeSpan.Hours.ToString();
            base.transform.Find("FrozenInfo/RemainTime/Time/Min").gameObject.SetActive(true);
            base.transform.Find("FrozenInfo/RemainTime/Time/MinText").gameObject.SetActive(true);
            base.transform.Find("FrozenInfo/RemainTime/Time/Min").GetComponent<Text>().text = ((timeSpan.Minutes > 0) || (timeSpan.Seconds <= 0)) ? string.Format("{0:D2}", timeSpan.Minutes) : "01";
            base.transform.Find("FrozenInfo/Slider").GetComponent<Slider>().value = ((float) timeSpan.TotalSeconds) / ((float) Singleton<PlayerModule>.Instance.playerData.endlessUseItemCDTime);
        }

        public void SetupView(int rank, EndlessPlayerData endlessData, string playerName, bool isSelect = false, Action<EndlessPlayerData> buttonClickCallback = null, EndlessToolDataItem selectToolData = null)
        {
            this._endlessPlayerData = endlessData;
            this._clickCallBack = buttonClickCallback;
            this._selectToolData = selectToolData;
            this._interactable = (selectToolData != null) && !selectToolData.ApplyToSelf;
            base.transform.Find("Me").gameObject.SetActive(rank == Singleton<EndlessModule>.Instance.CurrentRank);
            base.transform.Find("Rank").GetComponent<Text>().text = rank.ToString();
            base.transform.Find("PlayerName").GetComponent<Text>().text = playerName;
            base.transform.Find("FloorNum").GetComponent<Text>().text = (endlessData.get_progress() >= 1) ? endlessData.get_progress().ToString() : "-";
            base.transform.Find("FloorLabel").gameObject.SetActive(endlessData.get_progress() >= 1);
            List<EndlessToolDataItem> range = new List<EndlessToolDataItem>();
            foreach (EndlessWaitEffectItem item in this._endlessPlayerData.get_wait_effect_item_list())
            {
                EndlessToolDataItem item2 = new EndlessToolDataItem((int) item.get_item_id(), 1);
                if (item2.ShowIcon)
                {
                    range.Add(item2);
                }
            }
            if (range.Count > 5)
            {
                range = range.GetRange(range.Count - 5, 5);
            }
            Transform transform = base.transform.Find("ApplyedToolsList");
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                if (i >= range.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                    child.GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(range[(range.Count - i) - 1].GetSmallIconPath());
                }
            }
            this.SetFrozenTime();
            base.transform.Find("SelectedMask").gameObject.SetActive(isSelect);
            this.CheckInteractable();
        }

        private void Update()
        {
            if (this._endlessPlayerData != null)
            {
                this._checkTimer += Time.deltaTime;
                if (this._checkTimer >= 60f)
                {
                    this._checkTimer = 0f;
                    this.SetFrozenTime();
                    this.CheckInteractable();
                }
            }
        }
    }
}

