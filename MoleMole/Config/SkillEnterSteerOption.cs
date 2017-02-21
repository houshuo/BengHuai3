namespace MoleMole.Config
{
    using System;

    public class SkillEnterSteerOption
    {
        public float MaxSteeringAngle = 180f;
        public float MaxSteerNormalizedTimeEnd = 0.5f;
        public float MaxSteerNormalizedTimeStart;
        public bool MuteSteerWhenNoEnemy;
        public float SteerLerpRatio = 3f;
        public EnterSteerType SteerType;

        public enum EnterSteerType
        {
            Instant,
            LimitSteerRatioAndNormalizedEnd
        }
    }
}

