namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class InLevelReviveDialogContext : BaseDialogContext
    {
        private MonoGridScroller _dropGridScroller;
        private List<DropItem> _dropItemList;
        private LevelScoreManager _levelScoreManager;
        public bool allTeamDown;
        public uint avatarRuntimeID;
        public Vector3 revivePosition;
        private const int UNLIMIT_REVIVE_TIMES = 0xffff;

        public InLevelReviveDialogContext(uint avatarRuntimeID, Vector3 revivePosition, bool allTeamDown = false)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelReviveDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/InLevelReviveDialogV2"
            };
            base.config = pattern;
            this.avatarRuntimeID = avatarRuntimeID;
            this.revivePosition = revivePosition;
            this.allTeamDown = allTeamDown;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Title/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/CurrentGetItems/Items/PrevBtn").GetComponent<Button>(), new UnityAction(this.OnDropItemLeftArrowClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/CurrentGetItems/Items/NextBtn").GetComponent<Button>(), new UnityAction(this.OnDropItemLeftArrowClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), new UnityAction(this.OnGiveUpBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), new UnityAction(this.OnReviveButtonClick));
        }

        private void InitTeam()
        {
            if (this.allTeamDown)
            {
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel");
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (i >= allPlayerAvatars.Count)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        BaseMonoAvatar avatar = allPlayerAvatars[i];
                        child.GetComponent<MonoAvatarButton>().InitForReviveButton(avatar);
                        child.Find("CDMask").gameObject.SetActive(avatar.GetRuntimeID() != this.avatarRuntimeID);
                    }
                }
            }
        }

        private bool OnAvatarSelectForRevive(uint selectedAvatarRuntimeID)
        {
            this.SetupTeam(selectedAvatarRuntimeID);
            this.SetupReviveInfo();
            return false;
        }

        public void OnBGBtnClick()
        {
            if (!this.allTeamDown)
            {
                Singleton<LevelManager>.Instance.SetPause(false);
                this.Destroy();
            }
        }

        private void OnDropItemLeftArrowClick()
        {
            base.view.transform.Find("Dialog/Content/CurrentGetItems/Items").Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToPreItem();
        }

        private void OnDropItemRightArrowClick()
        {
            base.view.transform.Find("Dialog/Content/CurrentGetItems/Items").Find("ScrollView").GetComponent<MonoGridScroller>().ScrollToNextItem();
        }

        private void OnGiveUpBtnClick()
        {
            if (this.allTeamDown)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new InLevelGiveUpConfirmDialogContext(this, new Action(this.OnGiveUpConfirm)), UIType.Any);
            }
            else
            {
                this.OnBGBtnClick();
            }
        }

        private void OnGiveUpConfirm()
        {
            Singleton<LevelManager>.Instance.SetPause(false);
            this.Destroy();
            Singleton<EventManager>.Instance.FireEvent(new EvtLevelState(EvtLevelState.State.EndLose, EvtLevelState.LevelEndReason.EndLoseAllDead, 0), MPEventDispatchMode.Normal);
        }

        public override bool OnNotify(Notify ntf)
        {
            return ((ntf.type == NotifyTypes.AvatarSelectForRevive) && this.OnAvatarSelectForRevive((uint) ntf.body));
        }

        private void OnReviveButtonClick()
        {
            int reviveCost = this._levelScoreManager.GetReviveCost();
            if (reviveCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new InLevelRechargeDialogContext(this), UIType.Any);
            }
            else
            {
                string fullName = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.avatarRuntimeID).avatarDataItem.FullName;
                Singleton<MainUIManager>.Instance.ShowDialog(new InLevelReviveConfirmDialogContext(this, reviveCost, fullName), UIType.Any);
            }
        }

        public void OnReviveConfirm()
        {
            Singleton<LevelManager>.Instance.SetPause(false);
            this.Destroy();
        }

        private void OnScrollerChange(Transform trans, int index)
        {
            Vector2 cellSize = this._dropGridScroller.grid.GetComponent<GridLayoutGroup>().cellSize;
            trans.SetLocalScaleX(cellSize.x / trans.GetComponent<MonoLevelDropIconButton>().width);
            trans.SetLocalScaleY(cellSize.y / trans.GetComponent<MonoLevelDropIconButton>().height);
            DropItem item = this._dropItemList[index];
            StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem((int) item.get_item_id(), 1);
            dummyStorageDataItem.level = (int) item.get_level();
            dummyStorageDataItem.number = (int) item.get_num();
            trans.GetComponent<CanvasGroup>().alpha = 1f;
            trans.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, null, true, false, false, false);
        }

        public void RefreshView()
        {
            this.SetupView();
        }

        private void SetupCurrentGetItems()
        {
            if (this.allTeamDown)
            {
                base.view.transform.Find("Dialog/Content/CurrentGetItems/UpContent/Scoin/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._levelScoreManager.scoinInside).ToString();
                Transform transform = base.view.transform.Find("Dialog/Content/CurrentGetItems/Items");
                this._dropItemList = this._levelScoreManager.GetDropListToShow();
                transform.gameObject.SetActive(this._dropItemList.Count > 0);
                this._dropGridScroller = transform.Find("ScrollView").GetComponent<MonoGridScroller>();
                this._dropGridScroller.Init(new MonoGridScroller.OnChange(this.OnScrollerChange), this._dropItemList.Count, null);
                bool flag = this._dropItemList.Count > this._dropGridScroller.GetMaxItemCountWithouScroll();
                transform.Find("PrevBtn").gameObject.SetActive(flag);
                transform.Find("NextBtn").gameObject.SetActive(flag);
            }
        }

        private void SetupLayout()
        {
            base.view.transform.Find("Dialog/Content/CurrentGetItems").gameObject.SetActive(this.allTeamDown);
            base.view.transform.Find("Dialog/Content/Padding").gameObject.SetActive(!this.allTeamDown);
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/MiddleLine").gameObject.SetActive(!this.allTeamDown);
            base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel").gameObject.SetActive(this.allTeamDown);
            base.view.transform.Find("Dialog/Title/CloseBtn").gameObject.SetActive(!this.allTeamDown);
        }

        private void SetupReviveInfo()
        {
            Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel");
            int reviveCost = this._levelScoreManager.GetReviveCost();
            if (reviveCost > Singleton<PlayerModule>.Instance.playerData.hcoin)
            {
                transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                transform.Find("InfoAvatar").Find("HcoinLabel").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconRecharge").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconOK").gameObject.SetActive(false);
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Tab_Recharge", new object[0]);
            }
            else
            {
                transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>().color = MiscData.GetColor("Blue");
                transform.Find("InfoAvatar").Find("HcoinLabel").GetComponent<Text>().color = MiscData.GetColor("Blue");
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconRecharge").gameObject.SetActive(false);
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/IconOK").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_OK", new object[0]);
            }
            transform.Find("InfoAvatar").Find("HcoinNum").GetComponent<Text>().text = reviveCost.ToString();
            transform.Find("InfoAvatar").Find("AvatarFullName").GetComponent<Text>().text = Singleton<EventManager>.Instance.GetActor<AvatarActor>(this.avatarRuntimeID).avatarDataItem.FullName;
            Transform transform2 = transform.Find("Consume");
            transform2.Find("ReviveTimes").gameObject.SetActive(this._levelScoreManager.maxReviveNum != 0xffff);
            transform2.Find("ReviveTimes/AvaiableTimes").GetComponent<Text>().text = this._levelScoreManager.avaiableReviveNum.ToString();
            transform2.Find("ReviveTimes/MaxTimes").GetComponent<Text>().text = this._levelScoreManager.maxReviveNum.ToString();
            transform2.Find("Hcoin/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.hcoin.ToString();
            base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>().interactable = this._levelScoreManager.avaiableReviveNum > 0;
        }

        private void SetupTeam(uint selectAvatarRuntimeID)
        {
            this.avatarRuntimeID = selectAvatarRuntimeID;
            if (this.allTeamDown)
            {
                List<BaseMonoAvatar> allPlayerAvatars = Singleton<AvatarManager>.Instance.GetAllPlayerAvatars();
                Transform transform = base.view.transform.Find("Dialog/Content/ReviveConsumePanel/AvatarSelectPanel");
                for (int i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (i >= allPlayerAvatars.Count)
                    {
                        child.gameObject.SetActive(false);
                    }
                    else
                    {
                        BaseMonoAvatar avatar = allPlayerAvatars[i];
                        child.Find("CDMask").gameObject.SetActive(avatar.GetRuntimeID() != this.avatarRuntimeID);
                    }
                }
            }
        }

        protected override bool SetupView()
        {
            this._levelScoreManager = Singleton<LevelScoreManager>.Instance;
            this.SetupCurrentGetItems();
            this.InitTeam();
            this.SetupReviveInfo();
            this.SetupLayout();
            return false;
        }
    }
}

