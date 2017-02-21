namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.Events;

    public class InLevelGiveUpConfirmDialogContext : BaseDialogContext
    {
        private BaseDialogContext _fromDialogContext;
        private Action _giveUpCallBack;

        public InLevelGiveUpConfirmDialogContext(BaseDialogContext fromDialogContext, Action giveUpCallBack = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "InLevelGiveUpConfirmDialogContext",
                viewPrefabPath = "UI/Menus/Dialog/InLevelGiveUpConfirmDialog"
            };
            base.config = pattern;
            this._fromDialogContext = fromDialogContext;
            this._giveUpCallBack = giveUpCallBack;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("BG").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/CloseBtn").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/Think").GetComponent<Button>(), new UnityAction(this.OnBGBtnClick));
            base.BindViewCallback(base.view.transform.Find("Dialog/Content/ActionBtns/GiveUp").GetComponent<Button>(), new UnityAction(this.OnGiveUpBtnClick));
        }

        private void OnBGBtnClick()
        {
            this._fromDialogContext.view.SetActive(true);
            this.Destroy();
        }

        private void OnGiveUpBtnClick()
        {
            this.Destroy();
            if (this._giveUpCallBack != null)
            {
                this._giveUpCallBack();
            }
        }

        protected override bool SetupView()
        {
            this._fromDialogContext.view.SetActive(false);
            if (Singleton<LevelScoreManager>.Instance.LevelType == 4)
            {
                base.view.transform.Find("Dialog/Content/InfoPanel/Hint").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_EndlessGiveUp", new object[0]);
            }
            else
            {
                base.view.transform.Find("Dialog/Content/InfoPanel/Hint").GetComponent<Text>().text = LocalizationGeneralLogic.GetText("Menu_Desc_InLevelGiveUpHint", new object[0]);
            }
            return false;
        }
    }
}

