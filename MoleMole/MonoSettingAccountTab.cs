namespace MoleMole
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoSettingAccountTab : MonoBehaviour
    {
        [CompilerGenerated]
        private static Action<bool> <>f__am$cache0;

        public bool CheckNeedSave()
        {
            return false;
        }

        public void OnBindAccountBtnClick()
        {
            Singleton<AccountManager>.Instance.manager.BindUI();
        }

        public void OnBindEmailBtnClick()
        {
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(string.Format(manager.ORIGINAL_BIND_EMAIL_URL + "?uid={0}&token={1}", accountUid, accountToken), false);
            }
        }

        public void OnBindMobileBtnClick()
        {
            string accountUid = Singleton<AccountManager>.Instance.manager.AccountUid;
            string accountToken = Singleton<AccountManager>.Instance.manager.AccountToken;
            TheOriginalAccountManager manager = Singleton<AccountManager>.Instance.manager as TheOriginalAccountManager;
            if (manager != null)
            {
                WebViewGeneralLogic.LoadUrl(string.Format(manager.ORIGINAL_BIND_MOBILE_URL + "?uid={0}&token={1}", accountUid, accountToken), false);
            }
        }

        public void OnCDKEYValueChanged()
        {
        }

        public void OnExchangePresentBtnClick()
        {
        }

        public void OnLogoutBtnClick()
        {
            GeneralDialogContext dialogContext = new GeneralDialogContext {
                type = GeneralDialogContext.ButtonType.DoubleButton,
                title = LocalizationGeneralLogic.GetText("Menu_Title_Logout", new object[0]),
                desc = LocalizationGeneralLogic.GetText("Menu_Desc_Logout", new object[0])
            };
            if (<>f__am$cache0 == null)
            {
                <>f__am$cache0 = delegate (bool confirmed) {
                    if (confirmed)
                    {
                        Singleton<MainUIManager>.Instance.ShowWidget(new LoadingWheelWidgetContext(), UIType.Any);
                        Singleton<MiHoYoGameData>.Instance.GeneralLocalData.ClearLastLoginUser();
                        Singleton<MiHoYoGameData>.Instance.SaveGeneralData();
                        GeneralLogicManager.RestartGame();
                    }
                };
            }
            dialogContext.buttonCallBack = <>f__am$cache0;
            Singleton<MainUIManager>.Instance.ShowDialog(dialogContext, UIType.Any);
        }

        public void OnVerifyEmailApplyBtnClick()
        {
            Singleton<AccountManager>.Instance.manager.VerifyEmailApply();
        }

        public void SetupView()
        {
            bool flag = !string.IsNullOrEmpty(Singleton<AccountManager>.Instance.manager.AccountUid);
            base.transform.Find("LogoutBtn").gameObject.SetActive(flag);
            base.transform.Find("BindAccountBtn").gameObject.SetActive(!flag);
            base.transform.Find("Content/Info/AccountName").gameObject.SetActive(flag);
            base.transform.Find("Content/Info/TryUserLabel").gameObject.SetActive(!flag);
            if (flag)
            {
                base.transform.Find("Content/Info/AccountName/Text").GetComponent<Text>().text = Singleton<AccountManager>.Instance.manager.GetAccountName();
            }
            base.transform.Find("Content/Info/PlayerId/Text").GetComponent<Text>().text = Singleton<PlayerModule>.Instance.playerData.userId.ToString();
            GeneralLocalDataItem.AccountData account = Singleton<MiHoYoGameData>.Instance.GeneralLocalData.Account;
            bool flag2 = !flag || string.IsNullOrEmpty(account.email);
            bool flag3 = (flag && !string.IsNullOrEmpty(account.email)) && !account.isEmailVerify;
            bool flag4 = (flag && !string.IsNullOrEmpty(account.email)) && account.isEmailVerify;
            bool flag5 = !flag || string.IsNullOrEmpty(account.mobile);
            base.transform.Find("Content/Security/BindEmail/BindBtn").gameObject.SetActive(flag && string.IsNullOrEmpty(account.email));
            base.transform.Find("Content/Security/BindEmail/Status/NotVerify").gameObject.SetActive(flag3);
            base.transform.Find("Content/Security/BindEmail/Status/NotBind").gameObject.SetActive(flag2);
            base.transform.Find("Content/Security/BindEmail/Status/AlreadyBind").gameObject.SetActive(flag4);
            Button component = base.transform.Find("Content/Security/BindEmail/VerifyBtn").GetComponent<Button>();
            component.gameObject.SetActive(flag3);
            component.interactable = !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasSendVerifyEmailApply;
            base.transform.Find("Content/Security/BindMobile/BindBtn").gameObject.SetActive(flag && string.IsNullOrEmpty(account.mobile));
            base.transform.Find("Content/Security/BindMobile/Status/AlreadyBind").gameObject.SetActive(!flag5);
            base.transform.Find("Content/Security/BindMobile/Status/NotBind").gameObject.SetActive(flag5);
            if (Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                base.transform.Find("Content/ExchangeReward").gameObject.SetActive(false);
            }
            else
            {
                base.transform.Find("Content/ExchangeReward").gameObject.SetActive(true);
            }
        }
    }
}

