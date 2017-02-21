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

    public class TechTreeNodeDialogContext : BaseSequenceDialogContext
    {
        private CabinTechTreeNode _data;

        public TechTreeNodeDialogContext(CabinTechTreeNode data)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "TechTreeNodeDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/TechTreeNodeDialog"
            };
            base.config = pattern;
            this._data = data;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>(), new UnityAction(this.OnActivate));
        }

        private void Close()
        {
            this.Destroy();
        }

        private void InitView()
        {
            base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/AvatarIcon").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/LabelActive").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/ActivateBtn").gameObject.SetActive(true);
            base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>().interactable = false;
            base.view.transform.Find("Dialog/Content/Info/Error/BG/Line1").gameObject.SetActive(false);
            base.view.transform.Find("Dialog/Content/Info/Error/BG/Line2").gameObject.SetActive(false);
        }

        private void OnActivate()
        {
            Singleton<NetworkManager>.Instance.RequestAddCabinTech((CabinType) this._data._metaData.Cabin, this._data._metaData.X, this._data._metaData.Y);
            this.Destroy();
        }

        private void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void SetupDesc()
        {
            Text component = base.view.transform.Find("Dialog/Content/Info/Desc/Text").GetComponent<Text>();
            if (this._data._metaData.AbilityType == 1)
            {
                object[] replaceParams = new object[] { ((float) this._data._metaData.Argument2) / 100f };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, replaceParams);
            }
            else if (this._data._metaData.AbilityType == 2)
            {
                object[] objArray2 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray2);
            }
            else if (this._data._metaData.AbilityType == 3)
            {
                object[] objArray3 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray3);
            }
            else if (this._data._metaData.AbilityType == 4)
            {
                object[] objArray4 = new object[] { this._data._metaData.Argument1, MiscData.Config.VentureDifficultyDesc[this._data._metaData.Argument2] };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray4);
            }
            else if (this._data._metaData.AbilityType == 5)
            {
                object[] objArray5 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray5);
            }
            else if (this._data._metaData.AbilityType == 6)
            {
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, new object[0]);
            }
            else if (this._data._metaData.AbilityType == 7)
            {
                object[] objArray6 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray6);
            }
            else if (this._data._metaData.AbilityType == 8)
            {
                object[] objArray7 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray7);
            }
            else if (this._data._metaData.AbilityType == 9)
            {
                object[] objArray8 = new object[] { ((float) this._data._metaData.Argument1) / 100f };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray8);
            }
            else if (this._data._metaData.AbilityType == 10)
            {
                object[] objArray9 = new object[] { ((float) this._data._metaData.Argument1) / 100f };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray9);
            }
            else if (this._data._metaData.AbilityType == 11)
            {
                object[] objArray10 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray10);
            }
            else if (this._data._metaData.AbilityType == 14)
            {
                object[] objArray11 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray11);
            }
            else if (this._data._metaData.AbilityType == 13)
            {
                object[] objArray12 = new object[] { this._data._metaData.Argument1 };
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, objArray12);
            }
            else
            {
                component.text = LocalizationGeneralLogic.GetText(this._data._metaData.Desc, new object[0]);
            }
        }

        protected override bool SetupView()
        {
            this.InitView();
            base.view.transform.Find("Dialog/Content/Info/Title").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(this._data._metaData.Title, new object[0]);
            this.SetupDesc();
            base.view.transform.Find("Dialog/Content/NodeIcon/Image").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._data._metaData.Icon);
            Transform transform = base.view.transform.Find("Dialog/Content/Info/Error/BG/Line1");
            Transform transform2 = base.view.transform.Find("Dialog/Content/Info/Error/BG/Line2");
            if (this._data._status == TechTreeNodeStatus.Lock)
            {
                base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
                List<TechTreeNodeLockInfo> lockInfo = this._data.GetLockInfo();
                for (int i = 0; i < lockInfo.Count; i++)
                {
                    Transform transform3 = (i != 0) ? transform2 : transform;
                    transform3.gameObject.SetActive(true);
                    TechTreeNodeLockInfo info = lockInfo[i];
                    if (info._lockType == TechTreeNodeLock.CabinLevel)
                    {
                        string cabinName = Singleton<IslandModule>.Instance.GetCabinDataByType((CabinType) this._data._metaData.Cabin).GetCabinName();
                        object[] replaceParams = new object[] { cabinName, info._needLevel.ToString() };
                        transform3.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_TechTreeNodeLevelLack", replaceParams);
                    }
                    else if ((info._lockType == TechTreeNodeLock.AvatarLevel) || (info._lockType == TechTreeNodeLock.AvatarUnlock))
                    {
                        AvatarDataItem avatarByID = Singleton<AvatarModule>.Instance.GetAvatarByID(this._data._metaData.UnlockAvatarID);
                        object[] objArray2 = new object[] { avatarByID.ShortName, info._needLevel.ToString() };
                        transform3.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_TechTreeNodeLevelLack", objArray2);
                        base.view.transform.Find("Dialog/Content/AvatarIcon").gameObject.SetActive(true);
                        base.view.transform.Find("Dialog/Content/AvatarIcon/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_AvatarLevelLack", new object[0]);
                        base.view.transform.Find("Dialog/Content/AvatarIcon/Image").GetComponent<Image>().sprite = UIUtil.GetAvatarCardIcon(this._data._metaData.UnlockAvatarID);
                    }
                }
            }
            else if (this._data._status == TechTreeNodeStatus.Unlock_Ban_Active)
            {
                base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
                transform.gameObject.SetActive(true);
                transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_NeedActiveNeibour", new object[0]);
            }
            else if (this._data._status == TechTreeNodeStatus.Unlock_Ready_Active)
            {
                int leftPowerCost = Singleton<IslandModule>.Instance.GetLeftPowerCost();
                if (this._data._metaData.PowerCost > leftPowerCost)
                {
                    base.view.transform.Find("Dialog/Content/Info/Error").gameObject.SetActive(true);
                    transform.gameObject.SetActive(true);
                    transform.GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_PowerLack", new object[0]);
                }
                else
                {
                    base.view.transform.Find("Dialog/Content/ActivateBtn").GetComponent<Button>().interactable = true;
                }
            }
            else if (this._data._status == TechTreeNodeStatus.Active)
            {
                base.view.transform.Find("Dialog/Content/LabelActive").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/ActivateBtn").gameObject.SetActive(false);
            }
            base.view.transform.Find("Dialog/Content/PowerInfo/Num").GetComponent<Text>().text = this._data._metaData.PowerCost.ToString();
            return false;
        }
    }
}

