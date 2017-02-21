namespace MoleMole
{
    using System;
    using UnityEngine;

    public abstract class BaseWidgetContext : BaseContext
    {
        public BaseWidgetContext()
        {
            base.uiType = UIType.Root;
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent)
        {
            base.StartUp(canvasTrans, viewParent);
        }
    }
}

