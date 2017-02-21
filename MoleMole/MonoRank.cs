namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoRank : MonoBehaviour
    {
        private const string RANK_INFO_ROW_PREFAB_PATH = "UI/Menus/Widget/EndlessActivity/RankInfoRow";
        public MonoRankRewardRow rewardRow;

        public void SetupView(int startRank, int rewardID, List<int> playerUidList, EndlessMainPageContext.ViewStatus viewStatus = 0)
        {
            List<EndlessPlayerData> list = new List<EndlessPlayerData>();
            List<PlayerFriendBriefData> list2 = new List<PlayerFriendBriefData>();
            foreach (int num in playerUidList)
            {
                if (viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
                {
                    list2.Add(Singleton<EndlessModule>.Instance.GetPlayerBriefData(num));
                    list.Add(Singleton<EndlessModule>.Instance.GetPlayerEndlessData(num));
                }
                else
                {
                    list2.Add(Singleton<EndlessModule>.Instance.GetTopGroupPlayerBriefData(num));
                    list.Add(Singleton<EndlessModule>.Instance.GetTopGroupPlayerEndlessData(num));
                }
            }
            this.rewardRow.SetupView(rewardID);
            base.transform.Find("RankRowContainer").DestroyChildren();
            for (int i = 0; i < list.Count; i++)
            {
                Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/EndlessActivity/RankInfoRow")).transform;
                transform.SetParent(base.transform.Find("RankRowContainer"), false);
                transform.GetComponent<MonoRankInfoRow>().SetupView(i + startRank, list[i], UIUtil.GetPlayerNickname(list2[i]), viewStatus);
            }
        }
    }
}

