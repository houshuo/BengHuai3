namespace MoleMole
{
    using System;
    using UnityEngine;

    public class AvatarControlData
    {
        public BaseMonoEntity attackTarget;
        public static AvatarControlData emptyControlData = new AvatarControlData();
        public bool hasAnyControl;
        public bool hasOrderMove;
        public bool hasSetAttackTarget;
        public bool hasSteer;
        public float lerpRatio;
        public bool orderMove;
        public Vector3 steerDirection;
        public bool useAttack;
        public bool useHoldAttack;
        public bool[] useSkills = new bool[4];

        public AvatarControlData CopyFrom(AvatarControlData controlData)
        {
            this.hasSteer = controlData.hasSteer;
            this.steerDirection = controlData.steerDirection;
            this.lerpRatio = controlData.lerpRatio;
            this.hasOrderMove = controlData.hasOrderMove;
            this.orderMove = controlData.orderMove;
            this.hasSetAttackTarget = controlData.hasSetAttackTarget;
            this.attackTarget = controlData.attackTarget;
            this.useAttack = controlData.useAttack;
            this.useHoldAttack = controlData.useHoldAttack;
            this.hasAnyControl = controlData.hasAnyControl;
            controlData.useSkills.CopyTo(this.useSkills, 0);
            return null;
        }

        public void FrameReset()
        {
            this.hasAnyControl = false;
            this.hasSteer = false;
            this.hasOrderMove = false;
            this.hasSetAttackTarget = false;
            this.useAttack = false;
            this.useHoldAttack = false;
            for (int i = 0; i < this.useSkills.Length; i++)
            {
                this.useSkills[i] = false;
            }
        }
    }
}

