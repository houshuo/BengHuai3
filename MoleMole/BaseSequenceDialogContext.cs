namespace MoleMole
{
    using System;

    public class BaseSequenceDialogContext : BaseDialogContext
    {
        private Action OnDestroy;

        public override void Destroy()
        {
            if (this.OnDestroy != null)
            {
                this.OnDestroy();
            }
            base.Destroy();
        }

        public void SetDestroyCallBack(Action callBack)
        {
            this.OnDestroy = callBack;
        }
    }
}

