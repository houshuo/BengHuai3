namespace MoleMole.Config
{
    using System;

    public abstract class BaseUtilityAction : ConfigAbilityAction
    {
        public BaseUtilityAction()
        {
            base.Target = AbilityTargetting.Other;
        }
    }
}

