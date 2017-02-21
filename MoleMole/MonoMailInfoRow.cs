namespace MoleMole
{
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoMailInfoRow : MonoBehaviour
    {
        private Action<MailDataItem> _checkBtnCallBack;
        private Action<MailDataItem> _getBtnCallBack;
        private MailDataItem _mailData;
        private const string MAIL_OPEN_ICON_PATH = "SpriteOutput/Mail/IconMailOpen";
        private const string MAIL_READED_TEXT_ID = "Menu_Desc_MailReaded";
        private const string MAIL_REWARD_HC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarHCoin";
        private const string MAIL_REWARD_SC_PATH = "SpriteOutput/Mail/RewardIcons/IconStatusBarSCoin";
        private const string MAIL_UNREAD_ICON_PATH = "SpriteOutput/Mail/IconMailUnread";
        private const string MAIL_UNREND_TEXT_ID = "Menu_Desc_MailUnRead";

        public MailCacheKey GetMailCacheKey()
        {
            return this._mailData.GetKeyForMailCache();
        }

        private string GetMailContentAbstract()
        {
            if (string.IsNullOrEmpty(this._mailData.content))
            {
                return string.Empty;
            }
            string[] strArray = Regex.Split(UIUtil.ProcessStrWithNewLine(this._mailData.content), Environment.NewLine);
            if (strArray.Length <= 0)
            {
                return string.Empty;
            }
            string str2 = strArray[0];
            if (str2.Length > 20)
            {
                str2 = str2.Substring(0, (str2.Length <= 20) ? str2.Length : 20);
            }
            else
            {
                str2 = str2.Substring(0, str2.Length - 1);
            }
            return (str2 + "...");
        }

        public void OnCheckBtnClick()
        {
            if (this._checkBtnCallBack != null)
            {
                this._checkBtnCallBack(this._mailData);
            }
        }

        public void OnGetBtnClick()
        {
            if (this._getBtnCallBack != null)
            {
                this._getBtnCallBack(this._mailData);
            }
        }

        private void ResetIconImageSize()
        {
            Image component = base.transform.Find("ItemIconButton/ItemIcon").GetComponent<Image>();
            Image image2 = base.transform.Find("ItemIconButton/ItemIcon/Icon").GetComponent<Image>();
            image2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, component.rectTransform.rect.width);
            image2.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, component.rectTransform.rect.height);
        }

        public void SetupView(MailDataItem mailData, Action<MailDataItem> checkBtnCallBack, Action<MailDataItem> getBtnCallBack)
        {
            this._mailData = mailData;
            this._checkBtnCallBack = checkBtnCallBack;
            this._getBtnCallBack = getBtnCallBack;
            Image component = base.transform.Find("ItemIconButton/ItemIcon").GetComponent<Image>();
            Image image2 = base.transform.Find("ItemIconButton/ItemIcon/Icon").GetComponent<Image>();
            this.ResetIconImageSize();
            if (this._mailData.hasAttachment)
            {
                RewardUIData data = this._mailData.attachment.itemList[0];
                component.color = MiscData.GetColor("TotalWhite");
                if (data.rewardType == ResourceType.Item)
                {
                    image2.transform.GetComponent<MonoImageFitter>().enabled = true;
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(data.itemID, 1);
                    component.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[dummyStorageDataItem.rarity]);
                }
                else
                {
                    component.sprite = Miscs.GetSpriteByPrefab(MiscData.Config.ItemRarityBGImgPath[0]);
                    image2.transform.GetComponent<MonoImageFitter>().enabled = false;
                }
                image2.sprite = data.GetIconSprite();
                base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = "\x00d7" + data.value;
            }
            else
            {
                image2.transform.GetComponent<MonoImageFitter>().enabled = true;
                if (Singleton<MailModule>.Instance.IsMailRead(this._mailData))
                {
                    component.color = MiscData.GetColor("MailUnreadGrey");
                    image2.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Mail/IconMailOpen");
                    base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_MailReaded", new object[0]);
                }
                else
                {
                    component.color = MiscData.GetColor("Blue");
                    image2.sprite = Miscs.GetSpriteByPrefab("SpriteOutput/Mail/IconMailUnread");
                    base.transform.Find("ItemIconButton/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_MailUnRead", new object[0]);
                }
            }
            base.transform.Find("ItemIconButton/NewMark").gameObject.SetActive(Singleton<MailModule>.Instance.IsMailNew(this._mailData));
            base.transform.Find("Time/Time").GetComponent<Text>().text = Miscs.GetBeforeTimeToShow(this._mailData.time);
            base.transform.Find("Info/Content").GetComponent<Text>().text = this.GetMailContentAbstract();
            base.transform.Find("Info/Sender").GetComponent<Text>().text = this._mailData.sender;
            base.transform.Find("Title/Text").GetComponent<Text>().text = this._mailData.title;
            base.transform.Find("ActionBtns/GetBtn").gameObject.SetActive(this._mailData.hasAttachment);
        }
    }
}

