namespace MoleMole.Config
{
    using System;

    public abstract class AvatarAttackTargetSelect
    {
        public Action<BaseMonoAvatar> selectMethod;

        protected AvatarAttackTargetSelect()
        {
        }
    }
}

