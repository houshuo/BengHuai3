namespace MoleMole.Config
{
    using System;

    public class SelectMonsterAndPropByEllipse : AvatarAttackTargetSelect
    {
        public SelectMonsterAndPropByEllipse()
        {
            base.selectMethod = new Action<BaseMonoAvatar>(AvatarAttackTargetSelectPattern.SelectMonsterAndPropByEllipse);
        }
    }
}

