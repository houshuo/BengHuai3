namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class AvatarSubSkillDialogContext : BaseDialogContext
    {
        private List<NeedItemData> _showItemList;
        public readonly AvatarDataItem avatarData;
        public readonly AvatarSkillDataItem skillData;
        public readonly AvatarSubSkillDataItem subSkillData;

        public AvatarSubSkillDialogContext(AvatarDataItem avatarData, AvatarSkillDataItem skillData, AvatarSubSkillDataItem subSkillData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AvatarSubSkillDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/AvatarSubSkillDialog"
            };
            base.config = pattern;
            this.avatarData = avatarData;
            this.skillData = skillData;
            this.subSkillData = subSkillData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/LeftBtn").GetComponent<Button>(), new UnityAction(this.OnLeftBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn").GetComponent<Button>(), new UnityAction(this.OnRightBtnClick));
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
        }

        private bool OnAvatarSubSkillLevelUpRsp(AvatarSubSkillLevelUpRsp rsp)
        {
            if (rsp.get_retcode() != null)
            {
                string desc = string.Empty;
                if (rsp.get_retcode() == 5)
                {
                    int num = !this.subSkillData.UnLocked ? this.subSkillData.UnlockLv : this.subSkillData.LvUpNeedAvatarLevel;
                    object[] replaceParams = new object[] { num };
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), replaceParams);
                }
                else if (rsp.get_retcode() == 6)
                {
                    int num2 = !this.subSkillData.UnLocked ? this.subSkillData.UnlockStar : this.subSkillData.GetUpLevelStarNeed();
                    string str2 = MiscData.Config.AvatarStarName[num2];
                    object[] objArray2 = new object[] { str2 };
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), objArray2);
                }
                else
                {
                    desc = LocalizationGeneralLogic.GetNetworkErrCodeOutput(rsp.get_retcode(), new object[0]);
                }
                Singleton<MainUIManager>.Instance.ShowDialog(new GeneralHintDialogContext(desc, 2f), UIType.Any);
            }
            else
            {
                Dictionary<int, SubSkillStatus> subSkillStatusDict = Singleton<MiHoYoGameData>.Instance.LocalData.SubSkillStatusDict;
                switch (this.subSkillData.Status)
                {
                    case SubSkillStatus.CanUnlock:
                    case SubSkillStatus.CanUpLevel:
                        subSkillStatusDict[this.subSkillData.subSkillID] = this.subSkillData.Status;
                        break;

                    default:
                        subSkillStatusDict.Remove(this.subSkillData.subSkillID);
                        break;
                }
                Singleton<MiHoYoGameData>.Instance.Save();
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SubSkillStatusCacheUpdate, null));
                this.Close();
            }
            return false;
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Close();
        }

        private void OnChange(Transform trans, int index)
        {
            MonoItemIconButton component = trans.GetComponent<MonoItemIconButton>();
            component.SetupView(this._showItemList[index].itemData, MonoItemIconButton.SelectMode.None, false, false, false);
            component.transform.Find("NotEnough").gameObject.SetActive(!this._showItemList[index].enough);
            component.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemButonClick));
        }

        private void OnItemButonClick(StorageDataItemBase item, bool selected)
        {
            if ((this.subSkillData.UnLocked && this.subSkillData.GetLvUpNeedItemDataByID(item.ID).enough) || (!this.subSkillData.UnLocked && this.subSkillData.GetUnlockNeedItemDataByID(item.ID).enough))
            {
                UIUtil.ShowItemDetail(item, true, true);
            }
            else
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new DropLinkDialogContext(item as MaterialDataItem, null), UIType.Any);
            }
        }

        public void OnLeftBtnClick()
        {
            if (this.subSkillData.CanTry && !this.subSkillData.UnLocked)
            {
                Singleton<LevelScoreManager>.Create();
                Singleton<LevelScoreManager>.Instance.SetTryLevelBeginIntent(this.avatarData.avatarID, "Lua/Levels/Common/LevelInfinityTest.lua", this.skillData.skillID, this.subSkillData.subSkillID);
                Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true, true, null, true);
            }
            else
            {
                this.Close();
            }
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return ((pkt.getCmdId() == 0x33) && this.OnAvatarSubSkillLevelUpRsp(pkt.getData<AvatarSubSkillLevelUpRsp>()));
        }

        public void OnRightBtnClick()
        {
            Singleton<NetworkManager>.Instance.RequestAvatarSubSkillLevelUp(this.avatarData.avatarID, this.skillData.skillID, this.subSkillData.subSkillID);
        }

        private void SetupConsume()
        {
            bool flag = !this.subSkillData.UnLocked ? (this.avatarData.level < this.subSkillData.UnlockLv) : (this.avatarData.level < this.subSkillData.LvUpNeedAvatarLevel);
            bool flag2 = !this.subSkillData.UnLocked ? (this.avatarData.star < this.subSkillData.UnlockStar) : (this.avatarData.star < this.subSkillData.GetUpLevelStarNeed());
            if (flag || flag2)
            {
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume").gameObject.SetActive(false);
            }
            else
            {
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/Label/Unlock").gameObject.SetActive(!this.subSkillData.UnLocked);
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/Label/LvUp").gameObject.SetActive(this.subSkillData.UnLocked);
                int num = !this.subSkillData.UnLocked ? this.subSkillData.UnlockPoint : this.subSkillData.LvUpPoint;
                int num2 = !this.subSkillData.UnLocked ? this.subSkillData.UnlockSCoin : this.subSkillData.LvUpSCoin;
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().text = num.ToString();
                if (num > Singleton<PlayerModule>.Instance.playerData.skillPoint)
                {
                    base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                }
                else
                {
                    base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SkillPointNum").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
                }
                base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().text = num2.ToString();
                if (num2 > Singleton<PlayerModule>.Instance.playerData.scoin)
                {
                    base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().color = MiscData.GetColor("WarningRed");
                }
                else
                {
                    base.view.transform.Find("Dialog/Content/VerticalLayout/Consume/SCoinNum").GetComponent<Text>().color = MiscData.GetColor("TotalWhite");
                }
            }
        }

        private void SetupDesc()
        {
            base.view.transform.Find("Dialog/Content/VerticalLayout/DescText").GetComponent<Text>().text = !this.subSkillData.UnLocked ? this.subSkillData.Info : (LocalizationGeneralLogic.GetText("Menu_Desc_AfterLvUp", new object[0]) + this.subSkillData.NextLevelInfo);
        }

        private void SetupLackInfo()
        {
            bool flag = !this.subSkillData.UnLocked ? (this.avatarData.level < this.subSkillData.UnlockLv) : (this.avatarData.level < this.subSkillData.LvUpNeedAvatarLevel);
            bool flag2 = !this.subSkillData.UnLocked ? (this.avatarData.star < this.subSkillData.UnlockStar) : (this.avatarData.star < this.subSkillData.GetUpLevelStarNeed());
            Transform transform = base.view.transform.Find("Dialog/Content/VerticalLayout/LevelLack");
            Transform transform2 = base.view.transform.Find("Dialog/Content/VerticalLayout/StarLack");
            base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn").GetComponent<Button>().interactable = !flag && !flag2;
            if (flag2)
            {
                transform2.gameObject.SetActive(true);
                transform.gameObject.SetActive(false);
                transform2.Find("UnLockStar").GetComponent<MonoAvatarStar>().SetupView(!this.subSkillData.UnLocked ? this.subSkillData.UnlockStar : this.subSkillData.GetUpLevelStarNeed());
            }
            else if (flag)
            {
                transform2.gameObject.SetActive(false);
                transform.gameObject.SetActive(true);
                transform.Find("LvNeed").GetComponent<Text>().text = (!this.subSkillData.UnLocked ? this.subSkillData.UnlockLv : this.subSkillData.LvUpNeedAvatarLevel).ToString();
            }
            else
            {
                transform2.gameObject.SetActive(false);
                transform.gameObject.SetActive(false);
            }
        }

        private void SetupMaterials()
        {
            this._showItemList = !this.subSkillData.UnLocked ? this.subSkillData.UnlockNeedItemList : this.subSkillData.LvUpNeedItemList;
            if ((this._showItemList == null) || (this._showItemList.Count <= 0))
            {
                base.view.transform.Find("Dialog/Content/VerticalLayout/Materials").gameObject.SetActive(false);
            }
            else
            {
                foreach (NeedItemData data in this._showItemList)
                {
                    data.enough = Singleton<StorageModule>.Instance.TryGetStorageDataItemByMetaId(data.itemMetaID, data.itemNum).Count > 0;
                }
                base.view.transform.Find("Dialog/Content/VerticalLayout/Materials/Materials").GetComponent<MonoGridScroller>().Init(new MoleMole.MonoGridScroller.OnChange(this.OnChange), this._showItemList.Count, null);
            }
        }

        protected override bool SetupView()
        {
            Transform transform = base.view.transform.Find("Dialog/Content/DoubleButton/LeftBtn");
            bool flag = !this.subSkillData.UnLocked && this.subSkillData.CanTry;
            string textID = !flag ? "Menu_Cancel" : "Menu_TrySkill";
            transform.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            Transform transform2 = base.view.transform.Find("Dialog/Content/DoubleButton/RightBtn");
            string str2 = !this.subSkillData.UnLocked ? "Menu_Action_Unlock" : "Menu_LevelUp";
            transform2.Find("Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(str2, new object[0]);
            if (!flag)
            {
                transform.gameObject.SetActive(false);
                transform2.GetComponent<RectTransform>().SetLocalPositionX(0f);
            }
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/NameText").GetComponent<Text>().text = this.subSkillData.Name;
            Sprite spriteByPrefab = Miscs.GetSpriteByPrefab(this.subSkillData.IconPath);
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/Icon/Image").GetComponent<Image>().sprite = spriteByPrefab;
            Text component = base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/NameRow/AddPtText").GetComponent<Text>();
            component.gameObject.SetActive(this.subSkillData.UnLocked);
            if (this.subSkillData.level == this.subSkillData.MaxLv)
            {
                component.text = "MAX";
            }
            else
            {
                component.text = (this.subSkillData.level <= 0) ? string.Empty : string.Format("+{0}", this.subSkillData.level);
            }
            base.view.transform.Find("Dialog/Content/VerticalLayout/TopLine/RemainSkillPoint/Num").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.skillPoint.ToString();
            this.SetupDesc();
            this.SetupConsume();
            this.SetupMaterials();
            this.SetupLackInfo();
            return false;
        }
    }
}

