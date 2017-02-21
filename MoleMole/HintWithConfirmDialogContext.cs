namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class HintWithConfirmDialogContext : BaseSequenceDialogContext
    {
        private string _cancelBtnText;
        private string _desc;
        private Action<bool> _doubleButtonCallBack;
        private string _okBtnText;
        private Action _singleButtonCallBack;
        private string _title;
        private ButtonType _type;

        public HintWithConfirmDialogContext(string desc, string okBtnText, Action buttonCallBack, string title)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "HintWithConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/HintWithConfirmDialog"
            };
            base.config = pattern;
            this._title = title;
            this._desc = desc;
            this._okBtnText = okBtnText;
            this._singleButtonCallBack = buttonCallBack;
            this._type = ButtonType.SingleButton;
        }

        public HintWithConfirmDialogContext(string desc, string okBtnText, string cancelBtnText, Action<bool> buttonCallBack, string title)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "HintWithConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/HintWithConfirmDialog"
            };
            base.config = pattern;
            this._title = title;
            this._desc = desc;
            this._okBtnText = okBtnText;
            this._cancelBtnText = cancelBtnText;
            this._doubleButtonCallBack = buttonCallBack;
            this._type = ButtonType.DoubleButton;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK").GetComponent<Button>(), new UnityAction(this.OnOKButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").GetComponent<Button>(), new UnityAction(this.OnCancelButtonCallBack));
        }

        public void OnCancelButtonCallBack()
        {
            this.OnDoubleButtonClick(false);
        }

        private void OnDoubleButtonClick(bool isOk)
        {
            this.Destroy();
            if (this._doubleButtonCallBack != null)
            {
                this._doubleButtonCallBack(isOk);
            }
        }

        public void OnOKButtonCallBack()
        {
            if (this._type == ButtonType.SingleButton)
            {
                this.Destroy();
                if (this._singleButtonCallBack != null)
                {
                    this._singleButtonCallBack();
                }
            }
            else
            {
                this.OnDoubleButtonClick(true);
            }
        }

        protected override bool SetupView()
        {
            string str = !string.IsNullOrEmpty(this._okBtnText) ? this._okBtnText : LocalizationGeneralLogic.GetText("Menu_OK", new object[0]);
            string str2 = !string.IsNullOrEmpty(this._cancelBtnText) ? this._cancelBtnText : LocalizationGeneralLogic.GetText("Menu_Cancel", new object[0]);
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnOK/Text").GetComponent<Text>().text = str;
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel/Text").GetComponent<Text>().text = str2;
            base.view.transform.Find("Dialog/Title/Text").GetComponent<Text>().text = this._title;
            base.view.transform.Find("Dialog/Content/DescText").GetComponent<Text>().text = this._desc;
            base.view.transform.Find("Dialog/Content/ActionBtns/BtnCancel").gameObject.SetActive(this._type == ButtonType.DoubleButton);
            return false;
        }

        public enum ButtonType
        {
            SingleButton,
            DoubleButton
        }
    }
}

