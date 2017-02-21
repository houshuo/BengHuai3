namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class GeneralDialogContext : BaseSequenceDialogContext
    {
        public Action<bool> buttonCallBack;
        public string cancelBtnText;
        public string desc;
        public Action destroyCallBack;
        public bool hideCloseBtn;
        public bool notDestroyAfterCallback;
        public bool notDestroyAfterTouchBG;
        public string okBtnText;
        public string title;
        public ButtonType type;

        public GeneralDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GeneralDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/GeneralDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            if (this.type == ButtonType.SingleButton)
            {
                base.BindViewCallback(base.view.transform.Find("Dialog/Content/SingleButton/Btn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            }
            else
            {
                base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn").GetComponent<Button>(), new UnityAction(this.OnCancelButtonCallBack));
                base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            }
            if (!this.notDestroyAfterTouchBG)
            {
                base.BindViewCallback(base.view.transform.Find("BG"), EventTriggerType.PointerClick, new Action<BaseEventData>(this.OnBGClick));
            }
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Close));
        }

        public void Close()
        {
            this.Destroy();
            if (this.destroyCallBack != null)
            {
                this.destroyCallBack();
            }
        }

        public void OnBGClick(BaseEventData evtData = null)
        {
            this.Destroy();
        }

        private void OnButtonClick(bool isOk)
        {
            if (!this.notDestroyAfterCallback)
            {
                this.Destroy();
            }
            if (this.buttonCallBack != null)
            {
                this.buttonCallBack(isOk);
            }
        }

        public void OnCancelButtonCallBack()
        {
            this.OnButtonClick(false);
        }

        public void OnOKButtonCallBack()
        {
            this.OnButtonClick(true);
        }

        protected override bool SetupView()
        {
            if (this.type == ButtonType.SingleButton)
            {
                string str = !string.IsNullOrEmpty(this.okBtnText) ? this.okBtnText : LocalizationGeneralLogic.GetText("Menu_OK", new object[0]);
                base.view.transform.Find("Dialog/Content/SingleButton/Btn/Text").GetComponent<Text>().text = str;
                base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(true);
                base.view.transform.Find("Dialog/Content/DoubleButton").gameObject.SetActive(false);
            }
            else if (this.type == ButtonType.DoubleButton)
            {
                string str2 = !string.IsNullOrEmpty(this.okBtnText) ? this.okBtnText : LocalizationGeneralLogic.GetText("Menu_OK", new object[0]);
                string str3 = !string.IsNullOrEmpty(this.cancelBtnText) ? this.cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel", new object[0]);
                base.view.transform.Find("Dialog/Content/DoubleButton/CancelBtn/Text").GetComponent<Text>().text = str3;
                base.view.transform.Find("Dialog/Content/DoubleButton/OKBtn/Text").GetComponent<Text>().text = str2;
                base.view.transform.Find("Dialog/Content/SingleButton").gameObject.SetActive(false);
                base.view.transform.Find("Dialog/Content/DoubleButton").gameObject.SetActive(true);
            }
            base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = this.title;
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.desc;
            base.view.transform.Find("Dialog/CloseBtn").gameObject.SetActive(!this.hideCloseBtn);
            return false;
        }

        public enum ButtonType
        {
            SingleButton,
            DoubleButton
        }
    }
}

