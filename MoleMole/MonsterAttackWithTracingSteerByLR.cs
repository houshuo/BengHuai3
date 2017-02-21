namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class MonsterAttackWithTracingSteerByLR : MonsterAttackWithTracingSteer
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Steer Limit Type")]
        public SteerLimitState steerType;

        protected override void SteerStep()
        {
            Vector3 faceDirection = base._monster.FaceDirection;
            if (base._monster.AttackTarget != null)
            {
                faceDirection = base._monster.AttackTarget.XZPosition - base._monster.XZPosition;
                faceDirection.y = 0f;
                faceDirection.Normalize();
                Debug.DrawLine(base._monster.transform.position, base._monster.transform.position + ((Vector3) (faceDirection * 10f)), Color.blue);
                Debug.DrawLine(base._monster.transform.position, base._monster.transform.position + ((Vector3) (base._monster.FaceDirection * 10f)), Color.red);
                faceDirection = base._monster.FaceDirection + faceDirection;
                float num = Miscs.AngleFromToIgnoreY(base._monster.FaceDirection, faceDirection);
                if (this.steerType == SteerLimitState.OnlySteerLeft)
                {
                    if (num > 0f)
                    {
                        faceDirection = -faceDirection;
                    }
                }
                else if ((this.steerType == SteerLimitState.OnlySteerRight) && (num < 0f))
                {
                    faceDirection = -faceDirection;
                }
                Debug.DrawLine(base._monster.transform.position, base._monster.transform.position + ((Vector3) (faceDirection * 10f)), Color.yellow);
                base._monster.SteerFaceDirectionTo(Vector3.Slerp(base._monster.FaceDirection, faceDirection, (base.steerRatio * base._monster.TimeScale) * Time.deltaTime));
            }
        }

        public enum SteerLimitState
        {
            OnlySteerLeft,
            OnlySteerRight
        }
    }
}

