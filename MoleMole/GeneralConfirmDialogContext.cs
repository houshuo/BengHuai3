namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;

    public class GeneralConfirmDialogContext : BaseSequenceDialogContext
    {
        public Action<bool> buttonCallBack;
        public string cancelBtnText;
        public string desc;
        public Action destroyCallBack;
        public bool notDestroyAfterCallback;
        public string okBtnText;
        public ButtonType type;

        public GeneralConfirmDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "GeneralConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/GeneralConfirmDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").GetComponent<Button>(), new UnityAction(this.OnCancelButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
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
            string str = !string.IsNullOrEmpty(this.okBtnText) ? this.okBtnText : LocalizationGeneralLogic.GetText("Menu_OK", new object[0]);
            string str2 = !string.IsNullOrEmpty(this.cancelBtnText) ? this.cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel", new object[0]);
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel/Text").GetComponent<Text>().text = str2;
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK/Text").GetComponent<Text>().text = str;
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this.desc;
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").gameObject.SetActive(this.type == ButtonType.DoubleButton);
            return false;
        }

        public enum ButtonType
        {
            SingleButton,
            DoubleButton
        }
    }
}

