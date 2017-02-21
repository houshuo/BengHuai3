namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public abstract class BasePageContext : BaseContext
    {
        public List<BaseDialogContext> dialogContextList;
        protected bool showSpaceShip;

        public BasePageContext()
        {
            base.uiType = UIType.Page;
            this.dialogContextList = new List<BaseDialogContext>();
            this.showSpaceShip = false;
        }

        public virtual void BackPage()
        {
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public virtual void BackToMainMenuPage()
        {
            Singleton<MainUIManager>.Instance.BackToMainMenuPage();
        }

        public bool CheckHasDialogExceptNewbie(bool ignoreDialogInPage = false)
        {
            foreach (BaseDialogContext context in this.dialogContextList)
            {
                if ((((context != null) && !(context is NewbieDialogContext)) && ((context.view != null) && (context.view.transform != null))) && (context.view.transform.gameObject != null))
                {
                    if (ignoreDialogInPage)
                    {
                        if (context.view.transform.root == context.view.transform.parent)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Clear()
        {
            foreach (BaseDialogContext context in this.dialogContextList)
            {
                context.Destroy();
            }
            this.dialogContextList.Clear();
            this.Destroy();
        }

        public override void Destroy()
        {
            if (this.dialogContextList.Count > 0)
            {
                foreach (BaseDialogContext context in this.dialogContextList.ToArray())
                {
                    context.Destroy();
                }
            }
            base.Destroy();
        }

        private void HandleBackButton()
        {
            bool body = base.config.contextName != "MainPageContext";
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetBackButtonActive, body));
        }

        private void HandleSpaceShip()
        {
            if (this.showSpaceShip)
            {
                MonoMainCanvas mainCanvas = Singleton<MainUIManager>.Instance.GetMainCanvas() as MonoMainCanvas;
                if (mainCanvas != null)
                {
                    mainCanvas.InitMainPageContexts();
                }
            }
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipActive, new Tuple<bool, bool>(this.showSpaceShip, false)));
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.SetSpaceShipLight, base.config.contextName == "MainPageContext"));
        }

        public virtual void OnLandedFromBackPage()
        {
            this.HandleBackButton();
            this.HandleSpaceShip();
        }

        public override void SetActive(bool enabled)
        {
            foreach (BaseDialogContext context in this.dialogContextList.ToArray())
            {
                context.SetActive(enabled);
            }
            base.SetActive(enabled);
        }

        public bool spaceShipVisible()
        {
            return this.showSpaceShip;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            if (this.dialogContextList.Count > 0)
            {
                List<BaseDialogContext> list = new List<BaseDialogContext>();
                foreach (BaseDialogContext context in this.dialogContextList)
                {
                    if (!context.NeedRecoverWhenPageStartUp())
                    {
                        list.Add(context);
                    }
                }
                foreach (BaseDialogContext context2 in list)
                {
                    context2.Destroy();
                }
            }
            this.HandleBackButton();
            this.HandleSpaceShip();
            base.StartUp(canvasTrans, viewParent);
            if (this.dialogContextList.Count > 0)
            {
                List<BaseDialogContext> list2 = new List<BaseDialogContext>(this.dialogContextList);
                foreach (BaseDialogContext context3 in list2)
                {
                    if (!(context3 is NewbieDialogContext))
                    {
                        context3.StartUp(canvasTrans, base.view.transform.parent);
                    }
                }
            }
            if ((this.spaceShipVisible() && (Singleton<PlayerModule>.Instance != null)) && Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasLandedMainPage)
            {
                UIUtil.SpaceshipCheckWeather();
            }
        }

        public NewbieDialogContext TryToGetNewbieDialogContext()
        {
            foreach (BaseDialogContext context in this.dialogContextList)
            {
                if ((((context != null) && (context is NewbieDialogContext)) && ((context.view != null) && (context.view.transform != null))) && (context.view.transform.gameObject != null))
                {
                    return (NewbieDialogContext) context;
                }
            }
            return null;
        }
    }
}

