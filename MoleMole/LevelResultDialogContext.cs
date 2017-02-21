namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class LevelResultDialogContext : BaseDialogContext
    {
        private AddFriendDialogContext _addFriendDialog;
        private bool _avatarCanUnlockSkillDialogShowAlready;
        private int _dropItemAniCount;
        private SequenceAnimationManager _dropItemAnimationManager;
        private List<DropItem> _dropItemList;
        private SequenceDialogManager _dropNewItemDialogManager;
        private bool _dropPanelBGAniamtionEnd;
        private SequenceAnimationManager _dropPanelBGAnimationManager;
        private MonoGridScroller _dropScroller;
        private DropItem _fastDropItem;
        private FriendDetailDataItem _friendDetailData;
        private FriendDetailDataItem _helperInfo;
        private SequenceAnimationManager _leftPanelAnimationManager;
        private bool _leftPanelAnimationsEnd;
        private LevelDataItem _levelData;
        private bool _levelSuccess;
        private DropItem _normalDropItem;
        private bool _playerAvatarDialogsEnd;
        private SequenceDialogManager _playerLevelUpAndAvatarNewSkillDialogManager;
        private PlayerLevelUpDialogContext _playerLevelUpDialogContext;
        private DropItem _sonicDropItem;
        private StageEndRsp _stageEndRsp;
        private const string AVATAR_NULL_BG_PATH = "SpriteOutput/AvatarTachie/BgType4";
        private const string DROP_ANI_CALL_BACK_STR = "Play_drop";
        private const int DROP_ITEM_ANI_MAX_COUNT = 9;
        private const string DROP_ITEM_SCALE_07 = "DropItemScale07";
        private const int MAX_TEAM_MEMBER_NUM = 3;
        public Action onDestory;

        public LevelResultDialogContext(StageEndRsp rsp = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "LevelResultDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/LevelResultDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
            base.findViewSavedInScene = true;
            this._stageEndRsp = rsp;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("RewardPanel/OKBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("RewardPanel/ExpPanel/Helper/BattleFriendInfoRow/DetailButton").GetComponent<Button>(), new UnityAction(this.OnHelperClick));
            base.BindViewCallback(base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn").GetComponent<Button>(), new UnityAction(this.OnAddFriendBtnClick));
        }

        private void Close()
        {
            this.Destroy();
        }

        public override void Destroy()
        {
            if (Singleton<LevelScoreManager>.Instance == null)
            {
                base.Destroy();
            }
            else
            {
                bool hasNuclearActivityBefore = Singleton<LevelScoreManager>.Instance.hasNuclearActivityBefore;
                bool flag2 = (this._levelData != null) && Singleton<LevelScoreManager>.Instance.HasNuclearActivity(this._levelData.levelId);
                bool body = !hasNuclearActivityBefore && flag2;
                Singleton<LevelScoreManager>.Destroy();
                Singleton<NetworkManager>.Instance.RequestHasGotItemIdList();
                base.Destroy();
                if (this.onDestory != null)
                {
                    this.onDestory();
                }
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.StageEnd, body));
            }
        }

        private bool HasFriendToAdd()
        {
            if (this._friendDetailData == null)
            {
                return false;
            }
            return !Singleton<FriendModule>.Instance.IsMyFriend(this._friendDetailData.uid);
        }

        private void InitAnimationAndDialogManager()
        {
            this._leftPanelAnimationManager = new SequenceAnimationManager(new Action(this.OnLeftPanelAnimationsEnd), null);
            this._playerLevelUpAndAvatarNewSkillDialogManager = new SequenceDialogManager(new Action(this.OnPlayerAvatarDialogsEnd));
            this._dropPanelBGAnimationManager = new SequenceAnimationManager(new Action(this.OnDropPanelBGAniamtionEnd), null);
            this._dropNewItemDialogManager = new SequenceDialogManager(new Action(this.OnDropNewItemDialogsEnd));
            this._dropItemAnimationManager = new SequenceAnimationManager(new Action(this.OnItemPanelAnimationEnd), null);
            this._dropScroller = base.view.transform.Find("RewardPanel/DropPanel/Drops/ScrollView").GetComponent<MonoGridScroller>();
            this._leftPanelAnimationsEnd = false;
            this._playerAvatarDialogsEnd = false;
            this._dropPanelBGAniamtionEnd = false;
        }

        private void OnAddFriendBtnClick()
        {
            if (this._helperInfo != null)
            {
                Singleton<NetworkManager>.Instance.RequestAddFriend(this._helperInfo.uid);
                base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_RequestSend", new object[0]);
                base.view.transform.Find("RewardPanel/ExpPanel/Helper/Btn").GetComponent<Button>().interactable = false;
            }
        }

        private bool OnAvatarCanUnlockSkillNotify(Notify ntf)
        {
            if (!this._avatarCanUnlockSkillDialogShowAlready)
            {
                this._avatarCanUnlockSkillDialogShowAlready = true;
                if (this._playerLevelUpAndAvatarNewSkillDialogManager != null)
                {
                    if (this._playerLevelUpDialogContext != null)
                    {
                        this._playerLevelUpAndAvatarNewSkillDialogManager.AddAsFirstDialog(this._playerLevelUpDialogContext);
                    }
                    else
                    {
                        this._playerLevelUpAndAvatarNewSkillDialogManager.StartShow(0f);
                    }
                }
            }
            return false;
        }

        private bool OnAvatarLevelUpNotify(Notify ntf)
        {
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            Transform transform = base.view.transform.Find("RewardPanel/ExpPanel");
            for (int i = 0; i < 3; i++)
            {
                Transform child = transform.Find("AvatarExp/Team").GetChild(i);
                AvatarDataItem item = (i >= instance.memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(instance.memberList[i].avatarID);
                if (item != null)
                {
                    child.Find("Content/LVNum").GetComponent<Text>().text = item.level.ToString();
                }
            }
            return false;
        }

        private bool OnDropAniNofity(Notify ntf)
        {
            if ((this._dropItemAniCount < 9) && (this._dropItemAniCount < this._dropItemList.Count))
            {
                this._dropScroller.GetItemTransByIndex(this._dropItemAniCount).GetComponent<Animation>().Play();
                this._dropItemAniCount++;
            }
            return false;
        }

        private void OnDropItemBtnClick(StorageDataItemBase itemData)
        {
            UIUtil.ShowItemDetail(itemData, true, true);
        }

        private void OnDropNewItemDialogsEnd()
        {
            foreach (MonoLevelDropIconButtonBox box in base.view.transform.Find("RewardPanel/DropPanel").Find("Drops/ScrollView/Content").GetComponentsInChildren<MonoLevelDropIconButtonBox>())
            {
                box.SetOpenStatusView(true);
            }
            base.view.transform.Find("BlockPanel").gameObject.SetActive(false);
            if (this._addFriendDialog != null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(this._addFriendDialog, UIType.Any);
            }
        }

        private void OnDropPanelBGAniamtionEnd()
        {
            Transform transform = base.view.transform.Find("RewardPanel/DropPanel");
            IEnumerator enumerator = transform.Find("Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    MonoAnimationinSequence component = current.GetComponent<MonoAnimationinSequence>();
                    if (component != null)
                    {
                        component.animationName = current.GetComponent<MonoLevelDropIconButtonBox>().GetOpenAnimationName();
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this._dropItemAnimationManager.AddAllChildrenInTransform(transform.Find("Drops/ScrollView/Content"));
            this._dropItemAnimationManager.StartPlay(0f, true);
        }

        private void OnHelperClick()
        {
            if (this._helperInfo != null)
            {
                base.view.transform.gameObject.SetActive(false);
                UIUtil.ShowFriendDetailInfo(this._helperInfo, true, base.view.transform);
            }
        }

        private void OnItemPanelAnimationEnd()
        {
            IEnumerator enumerator = base.view.transform.Find("RewardPanel/DropPanel").Find("Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.GetComponent<MonoLevelDropIconButtonBox>().SetItemAfterAnimation();
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this._dropNewItemDialogManager.StartShow(0.6f);
        }

        private void OnLeftPanelAnimationsEnd()
        {
            this._leftPanelAnimationsEnd = true;
            if (!this._dropPanelBGAniamtionEnd && this._playerAvatarDialogsEnd)
            {
                this._dropPanelBGAniamtionEnd = true;
                this._dropPanelBGAnimationManager.StartPlay(0f, true);
            }
        }

        private void OnLoseNextBtnClick()
        {
            this.Close();
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.PlayerLevelUp)
            {
                return this.OnPlayerLevelUpNotify(ntf);
            }
            return ((ntf.type == NotifyTypes.AvatarLevelUp) && this.OnAvatarLevelUpNotify(ntf));
        }

        private void OnPlayerAvatarDialogsEnd()
        {
            this._playerAvatarDialogsEnd = true;
            if (!this._dropPanelBGAniamtionEnd && this._leftPanelAnimationsEnd)
            {
                this._dropPanelBGAniamtionEnd = true;
                this._dropPanelBGAnimationManager.StartPlay(0f, true);
            }
        }

        private bool OnPlayerLevelUpNotify(Notify ntf)
        {
            base.view.transform.Find("RewardPanel/ExpPanel").Find("PlayerExp/InfoRowLv").Find("LevelLabel").GetComponent<Text>().text = "LV." + Singleton<PlayerModule>.Instance.playerData.teamLevel;
            this._playerLevelUpAndAvatarNewSkillDialogManager.StartShow(0f);
            return false;
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            MonoLevelDropIconButtonBox component = trans.GetComponent<MonoLevelDropIconButtonBox>();
            MonoLevelDropIconButton button = trans.Find("Item").GetComponent<MonoLevelDropIconButton>();
            Vector2 cellSize = this._dropScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
            trans.SetLocalScaleX(cellSize.x / button.width);
            trans.SetLocalScaleY(cellSize.y / button.height);
            DropItem item = this._dropItemList[index];
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), 1);
            dummyStorageDataItem.level = (int) item.get_level();
            dummyStorageDataItem.number = (int) item.get_num();
            button.SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnDropItemBtnClick), true, true, false, false);
            if (item == this._normalDropItem)
            {
                component.SetupTypeView(MonoLevelDropIconButtonBox.Type.NormalFinishChallengeReward, this._dropPanelBGAniamtionEnd);
            }
            else if (item == this._fastDropItem)
            {
                component.SetupTypeView(MonoLevelDropIconButtonBox.Type.FastFinishChallengeReward, this._dropPanelBGAniamtionEnd);
            }
            else if (item == this._sonicDropItem)
            {
                component.SetupTypeView(MonoLevelDropIconButtonBox.Type.SonicFinishChallengeReward, this._dropPanelBGAniamtionEnd);
            }
            else
            {
                component.SetupTypeView(MonoLevelDropIconButtonBox.Type.DefaultDrop, this._dropPanelBGAniamtionEnd);
            }
        }

        private bool OnStageEndRsp(StageEndRsp rsp)
        {
            this._stageEndRsp = rsp;
            if (rsp.get_retcode() == null)
            {
                if (this._levelSuccess)
                {
                    LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
                    if (rsp.get_stage_idSpecified())
                    {
                    }
                    this._leftPanelAnimationManager.AddAnimation(base.view.transform.Find("Title").GetComponent<MonoAnimationinSequence>(), null);
                    this.SetupRewardPanel(this._stageEndRsp);
                    this.ShowRewardPanel();
                }
            }
            else
            {
                GeneralDialogContext dialogContext = new GeneralDialogContext {
                    type = GeneralDialogContext.ButtonType.SingleButton,
                    title = LocalizationGeneralLogic.GetText("Menu_Title_Tips", new object[0]),
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0])
                };
                Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
            }
            return false;
        }

        private void SetTotalDropList(StageEndRsp rsp, out List<DropItem> totalList, out DropItem normalDropItem, out DropItem fastDropItem, out DropItem sonicDropItem)
        {
            totalList = Singleton<LevelScoreManager>.Instance.GetTotalDropList();
            normalDropItem = null;
            fastDropItem = null;
            sonicDropItem = null;
            List<int> configChallengeIds = Singleton<LevelScoreManager>.Instance.configChallengeIds;
            LevelMetaData levelMeta = LevelMetaDataReader.TryGetLevelMetaDataByKey((int) rsp.get_stage_id());
            foreach (StageSpecialChallengeData data2 in rsp.get_special_challenge_list())
            {
                int num = (int) data2.get_challenge_index();
                if (num < configChallengeIds.Count)
                {
                    LevelChallengeDataItem item = new LevelChallengeDataItem(configChallengeIds[num], levelMeta, 0);
                    if (item.IsFinishStageNomalChallenge())
                    {
                        normalDropItem = data2.get_drop_item();
                        totalList.Add(normalDropItem);
                    }
                    else if (item.IsFinishStageFastChallenge())
                    {
                        fastDropItem = data2.get_drop_item();
                        totalList.Add(fastDropItem);
                    }
                    else if (item.IsFinishStageVeryFastChallenge())
                    {
                        sonicDropItem = data2.get_drop_item();
                        totalList.Add(sonicDropItem);
                    }
                }
            }
        }

        private void SetupFriendDialog()
        {
            if (this.HasFriendToAdd())
            {
                this._addFriendDialog = new AddFriendDialogContext(this._friendDetailData);
            }
        }

        private void SetupRewardPanel(StageEndRsp rsp)
        {
            string str;
            LevelScoreManager instance = Singleton<LevelScoreManager>.Instance;
            PlayerModule module = Singleton<PlayerModule>.Instance;
            Transform transform = base.view.transform.Find("RewardPanel/ExpPanel");
            Transform transform2 = transform.Find("PlayerExp/InfoRowLv");
            transform2.Find("LevelLabel").GetComponent<Text>().text = "LV." + instance.playerLevelBefore;
            transform2.Find("Exp/AddExp").GetComponent<Text>().text = rsp.get_player_exp_reward().ToString();
            transform2.Find("Exp/TiltSlider/").GetComponent<MonoMaskSlider>().UpdateValue((float) instance.playerExpBefore, (float) module.playerData.TeamMaxExp, 0f);
            transform2.Find("Exp/MaxNumText").GetComponent<Text>().text = module.playerData.TeamMaxExp.ToString();
            transform2.Find("Exp/NumText").GetComponent<Text>().text = module.playerData.teamExp.ToString();
            if (instance.playerLevelBefore < module.playerData.teamLevel)
            {
                this._playerLevelUpDialogContext = new PlayerLevelUpDialogContext();
                this._playerLevelUpAndAvatarNewSkillDialogManager.AddDialog(this._playerLevelUpDialogContext);
            }
            this._leftPanelAnimationManager.AddAnimation(transform.Find("PlayerExp").GetComponent<MonoAnimationinSequence>(), null);
            for (int i = 0; i < 3; i++)
            {
                Transform child = transform.Find("AvatarExp/Team").GetChild(i);
                Transform transform4 = transform.Find("AvatarExp/Exps").GetChild(i);
                AvatarDataItem avatarData = (i >= instance.memberList.Count) ? null : Singleton<AvatarModule>.Instance.GetAvatarByID(instance.memberList[i].avatarID);
                if (avatarData == null)
                {
                    child.gameObject.SetActive(false);
                    transform4.gameObject.SetActive(false);
                    child.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab("SpriteOutput/AvatarTachie/BgType4");
                }
                else
                {
                    AvatarDataItem item2 = instance.memberList[i];
                    child.Find("Content").gameObject.SetActive(true);
                    child.Find("BG/BGColor").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.AvatarAttributeBGSpriteList[avatarData.Attribute]);
                    child.Find("Content/Avatar").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(avatarData.AvatarTachie);
                    child.Find("Content/LVNum").GetComponent<Text>().text = item2.level.ToString();
                    child.Find("Content/StarPanel/AvatarStar").GetComponent<MonoAvatarStar>().SetupView(avatarData.star);
                    transform4.Find("AddExp").gameObject.SetActive(false);
                    if (rsp.get_avatar_exp_rewardSpecified())
                    {
                        transform4.Find("AddExp").gameObject.SetActive(true);
                        transform4.Find("AddExp").GetComponent<Text>().text = rsp.get_avatar_exp_reward().ToString();
                    }
                    transform4.Find("TiltSlider").GetComponent<MonoMaskSlider>().UpdateValue((float) avatarData.exp, (float) avatarData.MaxExp, 0f);
                    if ((avatarData.level != item2.level) || (avatarData.star != item2.star))
                    {
                        UIUtil.UpdateAvatarSkillStatusInLocalData(avatarData);
                    }
                    foreach (KeyValuePair<string, bool> pair in Singleton<AvatarModule>.Instance.GetCanUnlockSkillNameList(avatarData.avatarID, item2.level, item2.star, avatarData.level, avatarData.star))
                    {
                        this._playerLevelUpAndAvatarNewSkillDialogManager.AddDialog(new AvatarNewSkillCanUnlockDialogContext(avatarData.FullName, pair.Key, pair.Value));
                    }
                }
            }
            this._leftPanelAnimationManager.AddAnimation(transform.Find("AvatarExp").GetComponent<MonoAnimationinSequence>(), null);
            this._helperInfo = instance.friendDetailItem;
            this.SetTotalDropList(rsp, out this._dropItemList, out this._normalDropItem, out this._fastDropItem, out this._sonicDropItem);
            foreach (DropItem item3 in this._dropItemList)
            {
                if (Singleton<StorageModule>.Instance.IsItemNew((int) item3.get_item_id()))
                {
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item3.get_item_id(), 1);
                    dummyStorageDataItem.level = (int) item3.get_level();
                    dummyStorageDataItem.number = (int) item3.get_num();
                    this._dropNewItemDialogManager.AddDialog(new DropNewItemDialogContext(dummyStorageDataItem, true, false));
                }
            }
            Transform transform5 = base.view.transform.Find("RewardPanel/DropPanel");
            this._dropPanelBGAnimationManager.AddAnimation(transform5.GetComponent<MonoAnimationinSequence>(), null);
            transform5.Find("Drops/ScrollView").GetComponent<MonoGridScroller>().Init((trans, index) => this.OnScrollerChange(trans, index), this._dropItemList.Count, null);
            IEnumerator enumerator = transform5.Find("Drops/ScrollView/Content").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    MonoAnimationinSequence component = ((Transform) enumerator.Current).GetComponent<MonoAnimationinSequence>();
                    if (component != null)
                    {
                        component.animationName = "DropItemScale07";
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            this._dropPanelBGAnimationManager.AddAllChildrenInTransform(transform5.Find("Drops/ScrollView/Content"));
            transform5.Find("Reward").gameObject.SetActive(true);
            if (rsp.get_scoin_rewardSpecified())
            {
                str = rsp.get_scoin_reward().ToString();
            }
            else
            {
                str = "0";
            }
            transform5.Find("Reward/Num/Num").GetComponent<Text>().text = str;
        }

        private void SetupTitle()
        {
            this._levelSuccess = Singleton<LevelScoreManager>.Instance.endStatus == 1;
            this._levelData = Singleton<LevelModule>.Instance.GetLevelById(Singleton<LevelScoreManager>.Instance.LevelId);
            if (this._levelData == null)
            {
                base.view.transform.Find("Title/LevelInfo").gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("Title/LevelInfo").gameObject.SetActive(true);
                switch (this._levelData.LevelType)
                {
                    case 1:
                    {
                        ActDataItem item = new ActDataItem(this._levelData.ActID);
                        base.view.transform.Find("Title/LevelInfo/ActName").GetComponent<Text>().text = item.actTitle + " " + item.actName;
                        break;
                    }
                    case 2:
                    case 3:
                        base.view.transform.Find("Title/LevelInfo/ActName").GetComponent<Text>().text = Singleton<LevelModule>.Instance.GetWeekDayActivityByID(this._levelData.ActID).GetActitityTitle();
                        break;
                }
                base.view.transform.Find("Title/LevelInfo/LevelName").GetComponent<Text>().text = this._levelData.Title;
            }
        }

        protected override bool SetupView()
        {
            if (Singleton<LevelScoreManager>.Instance == null)
            {
                base.Destroy();
                return false;
            }
            base.view.transform.Find("BlockPanel").gameObject.SetActive(true);
            this._friendDetailData = Singleton<LevelScoreManager>.Instance.friendDetailItem;
            this.InitAnimationAndDialogManager();
            this.SetupTitle();
            this.OnStageEndRsp(Singleton<LevelScoreManager>.Instance.stageEndRsp);
            this.SetupFriendDialog();
            return false;
        }

        private void ShowRewardPanel()
        {
            base.view.transform.Find("RewardPanel").gameObject.SetActive(true);
            this._leftPanelAnimationManager.StartPlay(0f, true);
        }
    }
}

