namespace MoleMole
{
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoRankInfoRow : MonoBehaviour
    {
        private EndlessPlayerData _endlessPlayerData;
        private const int INVISIBLE_ITEM_ID = 0x1117f;
        private const int MAX_NUM_TOOLS_TO_SHOW = 5;

        private void OnEndlessToolTimeOut(int itemID)
        {
            <OnEndlessToolTimeOut>c__AnonStorey10C storeyc = new <OnEndlessToolTimeOut>c__AnonStorey10C {
                itemID = itemID
            };
            this._endlessPlayerData.get_wait_effect_item_list().RemoveAll(new Predicate<EndlessWaitEffectItem>(storeyc.<>m__1C1));
            this._endlessPlayerData.get_wait_burst_bomb_list().RemoveAll(new Predicate<EndlessWaitBurstBomb>(storeyc.<>m__1C2));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.EndlessAppliedToolChange, null));
        }

        public void SetupView(int rank, EndlessPlayerData endlessData, string playerName, EndlessMainPageContext.ViewStatus viewStatus = 0)
        {
            this._endlessPlayerData = endlessData;
            base.transform.Find("Rank").GetComponent<Text>().text = rank.ToString();
            base.transform.Find("PlayerName").GetComponent<Text>().text = playerName;
            base.transform.Find("FloorNum").GetComponent<Text>().text = (endlessData.get_progress() >= 1) ? endlessData.get_progress().ToString() : "-";
            base.transform.Find("FloorLabel").gameObject.SetActive(endlessData.get_progress() >= 1);
            if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
            {
                base.transform.Find("Me").gameObject.SetActive(rank == Singleton<EndlessModule>.Instance.CurrentRank);
            }
            else
            {
                base.transform.Find("Me").gameObject.SetActive(false);
            }
            Transform transform = base.transform.Find("ApplyedToolsList");
            List<EndlessWaitBurstBomb> list = endlessData.get_wait_burst_bomb_list();
            int index = 0;
            foreach (EndlessWaitBurstBomb bomb in list)
            {
                EndlessToolDataItem toolData = new EndlessToolDataItem((int) bomb.get_item_id(), 1);
                if (index < transform.childCount)
                {
                    Transform child = transform.GetChild(index);
                    child.gameObject.SetActive(true);
                    child.GetComponent<MonoAppliedEndlessTool>().SetupView(toolData, bomb.get_burst_time(), null);
                    index++;
                }
            }
            foreach (EndlessWaitEffectItem item2 in endlessData.get_wait_effect_item_list())
            {
                EndlessToolDataItem item3 = new EndlessToolDataItem((int) item2.get_item_id(), 1);
                if (item3.ShowIcon && (index < transform.childCount))
                {
                    Transform transform3 = transform.GetChild(index);
                    transform3.gameObject.SetActive(true);
                    transform3.GetComponent<MonoAppliedEndlessTool>().SetupView(item3, item2.get_expire_time(), new Action<int>(this.OnEndlessToolTimeOut));
                    index++;
                }
            }
            if ((viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup) && Singleton<EndlessModule>.Instance.PlayerInvisible((int) endlessData.get_uid()))
            {
                EndlessToolDataItem item4 = new EndlessToolDataItem(0x1117f, 1);
                if (item4.ShowIcon && (index < transform.childCount))
                {
                    Transform transform4 = transform.GetChild(index);
                    transform4.gameObject.SetActive(true);
                    transform4.GetComponent<MonoAppliedEndlessTool>().SetupView(item4, endlessData.get_hidden_expire_time(), new Action<int>(this.OnEndlessToolTimeOut));
                    index++;
                }
            }
            while (index < transform.childCount)
            {
                transform.GetChild(index).gameObject.SetActive(false);
                index++;
            }
        }

        [CompilerGenerated]
        private sealed class <OnEndlessToolTimeOut>c__AnonStorey10C
        {
            internal int itemID;

            internal bool <>m__1C1(EndlessWaitEffectItem item)
            {
                return (item.get_item_id() == this.itemID);
            }

            internal bool <>m__1C2(EndlessWaitBurstBomb item)
            {
                return (item.get_item_id() == this.itemID);
            }
        }
    }
}

