namespace MoleMole.Config
{
    using System;

    public class SelectNearestEnemyV1 : AvatarAttackTargetSelect
    {
        public SelectNearestEnemyV1()
        {
            base.selectMethod = new Action<BaseMonoAvatar>(AvatarAttackTargetSelectPattern.SelectNearestEnemyV1);
        }
    }
}

