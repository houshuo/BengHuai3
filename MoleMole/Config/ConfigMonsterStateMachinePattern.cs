namespace MoleMole.Config
{
    using FullInspector;
    using System;

    public class ConfigMonsterStateMachinePattern : ConfigEntityStateMachinePattern
    {
        public string AIMode;
        public float BackHitDegreeThreshold = 60f;
        public ConfigEntityAttackEffect BeHitEffect;
        public ConfigEntityAttackEffect BeHitEffectBig;
        public ConfigEntityAttackEffect BeHitEffectMid;
        public string DieAnimEventID;
        public float FastDieAnimationWaitDuration = 0.5f;
        [InspectorNullable]
        public string FastDieEffectPattern;
        public float HeavyRetreatThreshold = 25f;
        public bool KeepHitboxStanding;
        public float KeepHitboxStandingMinHeight = 0.7f;
        public float LeftRightHitAngleRange = 60f;
        public float RetreatBlowVelocityRatio = 1f;
        public float RetreatToVelocityScaleRatio = 0.02f;
        public float ThrowAnimDefenceRatio;
        [InspectorNullable]
        public string ThrowBlowAirNamedState;
        public float ThrowBlowAirNamedStateRetreatStopNormalizedTime = 1f;
        [InspectorNullable]
        public string ThrowBlowDieNamedState;
        [InspectorNullable]
        public string ThrowBlowNamedState;
        [InspectorNullable]
        public string ThrowDieEffectPattern;
        [InspectorNullable]
        public string ThrowUpNamedState;
        public float ThrowUpNamedStateRetreatStopNormalizedTime = 1f;
        public bool UseAbsMoveSpeed;
        public bool UseBackHitAngleCheck;
        public bool UseLeftRightHitAngleCheck;
        public bool UseRandomLeftRightHitEffectAsNormal;
        public bool UseStandByWalkSteer;
        public string WalkSteerAnimatorStateName;
        public float WalkSteerTimeThreshold = 0.3f;
    }
}

