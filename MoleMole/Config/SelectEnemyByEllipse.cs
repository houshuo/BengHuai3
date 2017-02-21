namespace MoleMole.Config
{
    using System;

    public class SelectEnemyByEllipse : AvatarAttackTargetSelect
    {
        public float TargetSelectionEccentricity;

        public SelectEnemyByEllipse()
        {
            base.selectMethod = new Action<BaseMonoAvatar>(AvatarAttackTargetSelectPattern.SelectEnemyByEllipse);
        }
    }
}

