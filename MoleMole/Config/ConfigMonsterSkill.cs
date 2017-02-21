namespace MoleMole.Config
{
    using System;

    public class ConfigMonsterSkill
    {
        public string AnimatorEventPattern;
        public string[] AnimatorStateNames;
        public float AnimDefenceNormalizedTimeStart;
        public float AnimDefenceNormalizedTimeStop = 1f;
        public float AnimDefenceRatio;
        public float AttackNormalizedTimeStart;
        public float AttackNormalizedTimeStop;
        public bool HighSpeedMovement;
        public float MassRatio = 1f;
        public bool NeedClearEffect;
        public bool SteerToTargetOnEnter;
        public bool Unselectable;
    }
}

