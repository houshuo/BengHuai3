namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class MonsterAttackWithFixedSteer : MonsterAttack
    {
        protected Vector3 _targetForward;
        public float endNormalizedTime;
        public bool moveBack;
        public float startNormalizedTime;
        public float steerRatio = 3f;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("Index of skill IDs for monster to steer.")]
        public int steerSkillIDIndex;

        protected void CalcTargetForward()
        {
            if (base._monster.AttackTarget != null)
            {
                int num = !this.moveBack ? 1 : -1;
                this._targetForward = (Vector3) (num * (base._monster.AttackTarget.XZPosition - base._monster.XZPosition));
                this._targetForward.Normalize();
            }
            else
            {
                this._targetForward = base._monster.FaceDirection;
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnTransit(MonsterAttack.State from, MonsterAttack.State to)
        {
            if (to == MonsterAttack.State.Doing)
            {
                this.CalcTargetForward();
            }
        }

        public override TaskStatus OnUpdate()
        {
            TaskStatus status = base.OnUpdate();
            if ((base._state == MonsterAttack.State.Doing) && (base._skillIx == this.steerSkillIDIndex))
            {
                float currentNormalizedTime = base._monster.GetCurrentNormalizedTime();
                if ((currentNormalizedTime > this.startNormalizedTime) && (currentNormalizedTime < this.endNormalizedTime))
                {
                    base._monster.SteerFaceDirectionTo(Vector3.Slerp(base._monster.FaceDirection, this._targetForward, (this.steerRatio * base._monster.TimeScale) * Time.deltaTime));
                }
            }
            return status;
        }
    }
}

