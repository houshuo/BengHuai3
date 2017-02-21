namespace MoleMole
{
    using proto;
    using System;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public class MonoBattleReportRow : MonoBehaviour
    {
        private string _content;
        private string _contentNoColor;
        private string _targetName;
        private string _userName;
        private EndlessMainPageContext.ViewStatus _viewStatus;

        private string GetNoColorText(string input)
        {
            return Regex.Replace(input, "<color=#.+?>", string.Empty).Replace("</color>", string.Empty);
        }

        private string GetPlayerName(int uid)
        {
            if (this._viewStatus == EndlessMainPageContext.ViewStatus.ShowCurrentGroup)
            {
                return UIUtil.GetPlayerNickname(Singleton<EndlessModule>.Instance.GetPlayerBriefData(uid));
            }
            return UIUtil.GetPlayerNickname(Singleton<EndlessModule>.Instance.GetTopGroupPlayerBriefData(uid));
        }

        public void SetFullColorText()
        {
            base.transform.Find("Text").GetComponent<Text>().text = this._content;
        }

        public void SetNoColorText()
        {
            base.transform.Find("Text").GetComponent<Text>().text = this._contentNoColor;
        }

        public void SetupView(EndlessWarInfo battleInfo, EndlessMainPageContext.ViewStatus viewStatus)
        {
            this._viewStatus = viewStatus;
            EndlessToolDataItem item = new EndlessToolDataItem((int) battleInfo.get_item_id(), 1);
            this._targetName = !battleInfo.get_target_uidSpecified() ? string.Empty : this.GetPlayerName((int) battleInfo.get_target_uid());
            this._userName = this.GetPlayerName((int) battleInfo.get_uid());
            if (item.ApplyToSelf)
            {
                object[] replaceParams = new object[] { this._userName };
                this._content = LocalizationGeneralLogic.GetText(item.ReportTextMapId, replaceParams);
            }
            else
            {
                object[] objArray2 = new object[] { this._userName, this._targetName };
                this._content = LocalizationGeneralLogic.GetText(item.ReportTextMapId, objArray2);
            }
            this._contentNoColor = this.GetNoColorText(this._content);
            base.transform.Find("Text").GetComponent<Text>().text = this._content;
        }
    }
}

