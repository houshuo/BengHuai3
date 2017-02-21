namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class MonsterAttackWithTracingSteer : MonsterAttack
    {
        public float endNormalizedTime;
        public bool moveBack;
        public float startNormalizedTime;
        public float steerRatio = 3f;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Index of skill IDs for monster to steer.")]
        public int steerSkillIDIndex;

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override TaskStatus OnUpdate()
        {
            TaskStatus status = base.OnUpdate();
            if ((base._state == MonsterAttack.State.Doing) && (base._skillIx == this.steerSkillIDIndex))
            {
                float currentNormalizedTime = base._monster.GetCurrentNormalizedTime();
                if ((currentNormalizedTime > this.startNormalizedTime) && (currentNormalizedTime < this.endNormalizedTime))
                {
                    this.SteerStep();
                }
            }
            return status;
        }

        protected virtual void SteerStep()
        {
            Vector3 faceDirection = base._monster.FaceDirection;
            if (base._monster.AttackTarget != null)
            {
                int num = !this.moveBack ? 1 : -1;
                faceDirection = (Vector3) (num * (base._monster.AttackTarget.XZPosition - base._monster.XZPosition));
                faceDirection.y = 0f;
                faceDirection.Normalize();
            }
            base._monster.SteerFaceDirectionTo(Vector3.Slerp(base._monster.FaceDirection, faceDirection, (this.steerRatio * base._monster.TimeScale) * Time.deltaTime));
        }
    }
}

