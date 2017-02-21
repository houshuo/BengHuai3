namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MailDetailDialogContext : BaseDialogContext
    {
        private MailDataItem _mailData;
        private const string MAIL_REWARD_HC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarHCoin";
        private const string MAIL_REWARD_SC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarSCoin";

        public MailDetailDialogContext(MailDataItem mailData)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MailDetailDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/MailDetailDialog"
            };
            base.config = pattern;
            this._mailData = mailData;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/OK").GetComponent<Button>(), new UnityAction(this.OnOKBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Destroy));
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.Destroy));
        }

        private void OnItemClick(StorageDataItemBase itemData, bool selected)
        {
            UIUtil.ShowItemDetail(itemData, false, true);
        }

        private void OnOKBtnClick()
        {
            if (this._mailData.hasAttachment)
            {
                Singleton<NetworkManager>.Instance.RequestGetOneMailAttachment(this._mailData);
            }
            this.Destroy();
        }

        private void OnScrollChange(Transform trans, int index)
        {
            RewardUIData data = this._mailData.attachment.itemList[index];
            Image component = trans.Find("ItemIcon").GetComponent<Image>();
            Image image2 = trans.Find("ItemIcon/Icon").GetComponent<Image>();
            component.gameObject.SetActive(true);
            trans.Find("SelectedMark").gameObject.SetActive(false);
            trans.Find("ProtectedMark").gameObject.SetActive(false);
            trans.Find("InteractiveMask").gameObject.SetActive(false);
            trans.Find("NotEnough").gameObject.SetActive(false);
            trans.Find("Star").gameObject.SetActive(false);
            trans.Find("StigmataType").gameObject.SetActive(false);
            trans.Find("UnidentifyText").gameObject.SetActive(false);
            trans.Find("QuestionMark").gameObject.SetActive(false);
            trans.Find("ItemIcon").GetComponent<Image>().color = Color.white;
            trans.Find("ItemIcon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
            if (data.rewardType == ResourceType.Item)
            {
                StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, data.level);
                dummyStorageDataItem.number = data.value;
                MonoItemIconButton button = trans.GetComponent<MonoItemIconButton>();
                button.SetupView(dummyStorageDataItem, MonoItemIconButton.SelectMode.None, false, false, false);
                button.SetClickCallback(new MonoItemIconButton.ClickCallBack(this.OnItemClick));
            }
            else
            {
                image2.sprite = data.GetIconSprite();
                trans.Find("Text").GetComponent<Text>().text = "\x00d7" + data.value;
            }
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = this._mailData.title;
            base.view.transform.Find("Dialog/Content/Sender/Sender").GetComponent<Text>().text = this._mailData.sender;
            base.view.transform.Find("Dialog/Content/Time/Time").GetComponent<Text>().text = Miscs.GetTimeString(this._mailData.time);
            base.view.transform.Find("Dialog/Content/MailContentScrollView/Content/Text").GetComponent<Text>().text = UIUtil.ProcessStrWithNewLine(this._mailData.content);
            Transform transform = base.view.transform.Find("Dialog/Content/Items");
            transform.gameObject.SetActive(this._mailData.hasAttachment);
            if (this._mailData.hasAttachment)
            {
                int count = this._mailData.attachment.itemList.Count;
                transform.Find("ScrollView").GetComponent<MonoGridScroller>().Init(new MonoGridScroller.OnChange(this.OnScrollChange), count, null);
                base.view.transform.Find("Dialog/Content/MailContentScrollView").GetComponent<LayoutElement>().preferredHeight = 150f;
            }
            else
            {
                base.view.transform.Find("Dialog/Content/MailContentScrollView").GetComponent<LayoutElement>().preferredHeight = 350f;
            }
            string textID = !this._mailData.hasAttachment ? "Menu_Close" : "Menu_Action_Get";
            base.view.transform.Find("Dialog/Content/ActionBtns/OK/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(textID, new object[0]);
            return false;
        }
    }
}

