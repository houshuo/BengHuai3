namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine.Events;

    public class EndlessInfoDialogContext : BaseDialogContext
    {
        public EndlessInfoDialogContext()
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "EndlessInfoDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/EndlessInfoDialog"
            };
            base.config = pattern;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.Destroy));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.Destroy));
        }

        protected override bool SetupView()
        {
            base.view.transform.Find("Dialog/Content/ScrollView/Viewport/Content/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessInfo", new object[0]);
            return false;
        }
    }
}

