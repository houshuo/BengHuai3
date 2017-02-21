namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public abstract class BaseDialogContext : BaseContext
    {
        public BasePageContext pageContext;

        public BaseDialogContext()
        {
            base.uiType = UIType.Dialog;
        }

        private void CheckAndSetNewbieDialogAvailable()
        {
            NewbieDialogContext context = null;
            if (this is NewbieDialogContext)
            {
                context = (NewbieDialogContext) this;
            }
            else if (this.pageContext != null)
            {
                context = this.pageContext.TryToGetNewbieDialogContext();
            }
            if ((context != null) && (context.view != null))
            {
                Transform transform = null;
                if (this.pageContext.dialogContextList.Count > 0)
                {
                    List<BaseDialogContext> list = new List<BaseDialogContext>();
                    list.AddRange(this.pageContext.dialogContextList);
                    list.Reverse();
                    foreach (BaseDialogContext context2 in list)
                    {
                        if (((context2 != null) && (context2.view != null)) && ((context2.view.transform != null) && (context2.view.transform.gameObject != null)))
                        {
                            transform = context2.view.transform;
                            break;
                        }
                    }
                }
                if (context.view.transform != transform)
                {
                    context.SetAvailable(false);
                }
                else if (!this.pageContext.CheckHasDialogExceptNewbie(false))
                {
                    context.SetAvailable(true);
                }
                else
                {
                    if (context.referredContext != null)
                    {
                        BaseDialogContext context3 = null;
                        List<BaseDialogContext> list2 = new List<BaseDialogContext>();
                        list2.AddRange(this.pageContext.dialogContextList);
                        list2.Reverse();
                        foreach (BaseDialogContext context4 in list2)
                        {
                            if (!(context4 is NewbieDialogContext))
                            {
                                context3 = context4;
                                break;
                            }
                        }
                        if (context3 == context.referredContext)
                        {
                            context.SetAvailable(true);
                            return;
                        }
                    }
                    context.SetAvailable(false);
                }
            }
        }

        public override void Destroy()
        {
            if (this.pageContext != null)
            {
                this.pageContext.dialogContextList.Remove(this);
            }
            base.Destroy();
            if (!(this is NewbieDialogContext))
            {
                this.CheckAndSetNewbieDialogAvailable();
            }
            Singleton<MainUIManager>.Instance.LockUI(false, 3f);
        }

        public virtual bool NeedRecoverWhenPageStartUp()
        {
            return true;
        }

        public override void SetActive(bool enabled)
        {
            base.SetActive(enabled);
            if (!(this is NewbieDialogContext))
            {
                this.CheckAndSetNewbieDialogAvailable();
            }
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent)
        {
            base.StartUp(canvasTrans, viewParent);
            base.view.transform.SetAsLastSibling();
            this.CheckAndSetNewbieDialogAvailable();
        }
    }
}

