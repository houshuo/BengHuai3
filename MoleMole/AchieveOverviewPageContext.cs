namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class AchieveOverviewPageContext : BasePageContext
    {
        private MonoGridScroller _achieveScroller;
        private Dictionary<int, RectTransform> _dictBeforeFetch;
        private MonoScrollerFadeManager _fadeMgr;
        [CompilerGenerated]
        private static Comparison<MissionDataItem> <>f__am$cache3;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, int> <>f__am$cache4;
        [CompilerGenerated]
        private static Func<KeyValuePair<int, RectTransform>, RectTransform> <>f__am$cache5;

        public AchieveOverviewPageContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AchieveOverviewPageContext",
                viewPrefabPath = "UI/Menus/Page/Achieve/AchieveOverviewPage"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
        }

        private void FetchWidget()
        {
            this._achieveScroller = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoGridScroller>();
        }

        private bool IsMissionEqual(RectTransform missionNew, RectTransform missionOld)
        {
            if ((missionNew == null) || (missionOld == null))
            {
                return false;
            }
            MonoAchieveInfo component = missionOld.GetComponent<MonoAchieveInfo>();
            return (missionNew.GetComponent<MonoAchieveInfo>().id == component.id);
        }

        private void OnFetchRewardButtonClicked(MissionDataItem data)
        {
            if (data != null)
            {
                Singleton<NetworkManager>.Instance.RequestGetMissionReward((uint) data.id);
                Transform transform = base.view.transform.Find("MissionList/ScrollView");
                if (transform != null)
                {
                    Dictionary<int, RectTransform> itemDict = transform.GetComponent<MonoGridScroller>().GetItemDict();
                    if (itemDict != null)
                    {
                        if (<>f__am$cache4 == null)
                        {
                            <>f__am$cache4 = entry => entry.Key;
                        }
                        if (<>f__am$cache5 == null)
                        {
                            <>f__am$cache5 = entry => entry.Value;
                        }
                        this._dictBeforeFetch = Enumerable.ToDictionary<KeyValuePair<int, RectTransform>, int, RectTransform>(itemDict, <>f__am$cache4, <>f__am$cache5);
                    }
                }
            }
        }

        private void OnMissionRewardGot(GetMissionRewardRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                AchieveRewardGotContext dialogContext = new AchieveRewardGotContext(rsp.get_reward_list());
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.MissionUpdated)
            {
                this.SetupView();
            }
            else if (ntf.type == NotifyTypes.MissionRewardGot)
            {
                this.OnMissionRewardGot((GetMissionRewardRsp) ntf.body);
            }
            return false;
        }

        private List<MissionDataItem> SelectAchieveToDisplay(List<MissionDataItem> fromList)
        {
            List<MissionDataItem> list = new List<MissionDataItem>();
            int num = 0;
            int count = fromList.Count;
            while (num < count)
            {
                LinearMissionData data = LinearMissionDataReader.TryGetLinearMissionDataByKey(fromList[num].metaData.id);
                if ((data != null) && (data.IsAchievement == 1))
                {
                    list.Add(fromList[num]);
                }
                num++;
            }
            return list;
        }

        private void SetupAchieveInfoScroller()
        {
            <SetupAchieveInfoScroller>c__AnonStoreyE4 ye = new <SetupAchieveInfoScroller>c__AnonStoreyE4 {
                <>f__this = this
            };
            Dictionary<int, MissionDataItem> missionDict = Singleton<MissionModule>.Instance.GetMissionDict();
            if (missionDict != null)
            {
                List<MissionDataItem> fromList = Enumerable.ToList<MissionDataItem>(missionDict.Values);
                ye.displayList = this.SelectAchieveToDisplay(fromList);
                if (<>f__am$cache3 == null)
                {
                    <>f__am$cache3 = delegate (MissionDataItem lhs, MissionDataItem rhs) {
                        int num = 0;
                        int num2 = 0;
                        if (lhs.status == 3)
                        {
                            num = 0;
                        }
                        else if (lhs.status == 2)
                        {
                            num = 1;
                        }
                        else if (lhs.status == 5)
                        {
                            num = 2;
                        }
                        if (rhs.status == 3)
                        {
                            num2 = 0;
                        }
                        else if (rhs.status == 2)
                        {
                            num2 = 1;
                        }
                        else if (rhs.status == 5)
                        {
                            num2 = 2;
                        }
                        if (num != num2)
                        {
                            return num - num2;
                        }
                        return lhs.id - rhs.id;
                    };
                }
                ye.displayList.Sort(<>f__am$cache3);
                if (this._achieveScroller != null)
                {
                    this._achieveScroller.Init(new MonoGridScroller.OnChange(ye.<>m__11C), ye.displayList.Count, null);
                    this._fadeMgr = base.view.transform.Find("MissionList/ScrollView").GetComponent<MonoScrollerFadeManager>();
                    this._fadeMgr.Init(this._achieveScroller.GetItemDict(), this._dictBeforeFetch, new Func<RectTransform, RectTransform, bool>(this.IsMissionEqual));
                    this._fadeMgr.Play();
                    this._dictBeforeFetch = null;
                }
            }
        }

        protected override bool SetupView()
        {
            if ((base.view != null) && (base.view.transform != null))
            {
                this.FetchWidget();
                this.SetupAchieveInfoScroller();
            }
            return false;
        }

        [CompilerGenerated]
        private sealed class <SetupAchieveInfoScroller>c__AnonStoreyE4
        {
            internal AchieveOverviewPageContext <>f__this;
            internal List<MissionDataItem> displayList;

            internal void <>m__11C(Transform trans, int index)
            {
                MonoAchieveInfo component = trans.GetComponent<MonoAchieveInfo>();
                if (component != null)
                {
                    component.SetupView(this.displayList[index]);
                    component.SetupFetchRewardButtonClickCallback(new UnityAction<MissionDataItem>(this.<>f__this.OnFetchRewardButtonClicked));
                }
            }
        }
    }
}

