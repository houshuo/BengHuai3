namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class SignInRewardGotDialogContext : BaseDialogContext
    {
        private SequenceAnimationManager _animationManager;
        private List<RewardUIData> _missionRewardList = new List<RewardUIData>();
        private OnDialogDestroy _onDestroy;
        private GetSignInRewardRsp _signInRewardRsp;
        private int _signInTimes;

        public SignInRewardGotDialogContext(GetSignInRewardRsp rsp, int times = 1)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "SignInRewardGotDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/SignInRewardGotDialog"
            };
            base.config = pattern;
            this._signInRewardRsp = rsp;
            this._signInTimes = times;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/OKBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
        }

        private string GetDesc(string textID, int id)
        {
            if (textID == RewardUIData.ITEM_ICON_TEXT_ID)
            {
                return Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id, 1).GetDisplayTitle();
            }
            return LocalizationGeneralLogic.GetText(textID, new object[0]);
        }

        private void InitRewardList()
        {
            RewardData data;
            if (this._signInRewardRsp != null)
            {
                data = this._signInRewardRsp.get_reward_list()[0];
            }
            else
            {
                return;
            }
            this._missionRewardList.Clear();
            if (data.get_exp() > 0)
            {
                RewardUIData playerExpData = RewardUIData.GetPlayerExpData((int) data.get_exp());
                this._missionRewardList.Add(playerExpData);
            }
            if (data.get_scoin() > 0)
            {
                RewardUIData scoinData = RewardUIData.GetScoinData((int) data.get_scoin());
                this._missionRewardList.Add(scoinData);
            }
            if (data.get_hcoin() > 0)
            {
                RewardUIData hcoinData = RewardUIData.GetHcoinData((int) data.get_hcoin());
                this._missionRewardList.Add(hcoinData);
            }
            if (data.get_stamina() > 0)
            {
                RewardUIData staminaData = RewardUIData.GetStaminaData((int) data.get_stamina());
                this._missionRewardList.Add(staminaData);
            }
            if (data.get_skill_point() > 0)
            {
                RewardUIData skillPointData = RewardUIData.GetSkillPointData((int) data.get_skill_point());
                this._missionRewardList.Add(skillPointData);
            }
            if (data.get_friends_point() > 0)
            {
                RewardUIData friendPointData = RewardUIData.GetFriendPointData((int) data.get_friends_point());
                this._missionRewardList.Add(friendPointData);
            }
            foreach (RewardItemData data8 in data.get_item_list())
            {
                RewardUIData item = new RewardUIData(ResourceType.Item, (int) data8.get_num(), RewardUIData.ITEM_ICON_TEXT_ID, string.Empty, (int) data8.get_id(), (int) data8.get_level());
                this._missionRewardList.Add(item);
            }
            foreach (RewardUIData data10 in this._missionRewardList)
            {
            }
        }

        private void OnBGBtnClick()
        {
            this.Destroy();
            if (this._onDestroy != null)
            {
                this._onDestroy();
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.UnlockAvatar)
            {
                int body = (int) ntf.body;
                Singleton<MainUIManager>.Instance.ShowDialog(new AvatarUnlockDialogContext(body, false), UIType.Any);
            }
            return false;
        }

        public void RegisterCallBack(OnDialogDestroy callback)
        {
            this._onDestroy = callback;
        }

        private void SetupContents()
        {
            switch (this._missionRewardList.Count)
            {
                case 1:
                {
                    RewardUIData data = this._missionRewardList[0];
                    Transform line = base.view.transform.Find("Dialog/Content/RewardPanel");
                    this.SetupLine(line, data);
                    break;
                }
            }
        }

        private void SetupLine(Transform line, RewardUIData data)
        {
            line.gameObject.SetActive(true);
            line.Find("Image").GetComponent<Image>().sprite = data.GetIconSprite();
            line.Find("Desc").GetComponent<Text>().text = this.GetDesc(data.valueLabelTextID, data.itemID);
            line.Find("Number").GetComponent<Text>().text = string.Format("\x00d7{0}", data.value);
        }

        private void SetupTitle()
        {
            int month = TimeUtil.Now.Month;
            base.view.transform.Find("Dialog/Content/Title/Month").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(MiscData.Config.MonthTextIDList[month], new object[0]);
            base.view.transform.Find("Dialog/Content/Title/DayNum").GetComponent<Text>().text = this._signInTimes.ToString();
        }

        protected override bool SetupView()
        {
            this._animationManager = new SequenceAnimationManager(null, null);
            this.SetupTitle();
            this.InitRewardList();
            this.SetupContents();
            this._animationManager.AddAnimation(base.view.transform.Find("Dialog/Content/CompleteIcon").GetComponent<MonoAnimationinSequence>(), null);
            this._animationManager.StartPlay(0.5f, true);
            Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
            return false;
        }

        public delegate void OnDialogDestroy();
    }
}

