namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class AgreementDialogContext : BaseSequenceDialogContext
    {
        public Action<bool> buttonCallBack;

        public AgreementDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "AgreementDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/UserAgreementDialog",
                cacheType = ViewCacheType.DontCache
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/Toggle").GetComponent<Toggle>(), new UnityAction<bool>(this.OnAgreeToggleCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>(), new UnityAction(this.OnAgreeButtonCallBack));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/DoubleButton/NotAgreeBtn").GetComponent<Button>(), new UnityAction(this.OnNotAgreeButtonCallBack));
        }

        public void OnAgreeButtonCallBack()
        {
            if (this.buttonCallBack != null)
            {
                this.buttonCallBack(true);
            }
            this.Destroy();
        }

        public void OnAgreeToggleCallBack(bool check)
        {
            base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>().interactable = check;
        }

        public void OnNotAgreeButtonCallBack()
        {
            if (this.buttonCallBack != null)
            {
                this.buttonCallBack(false);
            }
            this.Destroy();
        }

        protected override bool SetupView()
        {
            string str = CommonUtils.LoadTextFileToString("Data/Agreement");
            base.view.transform.Find("Dialog/Content/Scroll View/Viewport/Content/DescText").GetComponent<Text>().text = str;
            base.view.transform.Find("Dialog/Content/DoubleButton/AgreeBtn").GetComponent<Button>().interactable = false;
            return false;
        }
    }
}

