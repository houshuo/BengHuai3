namespace MoleMole
{
    using BehaviorDesigner.Runtime.Tasks;
    using System;
    using UnityEngine;

    [TaskCategory("Monster")]
    public class MonsterAttackWithFixedSteerByLR : MonsterAttackWithFixedSteer
    {
        private bool _isAttackTypeL;
        public string attackTypeL;

        private void CalcIsAttackTypeL()
        {
            Vector3 rhs = new Vector3(this._targetForward.x, 0f, this._targetForward.z);
            Vector3 lhs = new Vector3(base._monster.FaceDirection.x, 0f, base._monster.FaceDirection.z);
            float num = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(lhs, rhs)));
            this._isAttackTypeL = num <= 0f;
        }

        protected override bool DoAttack()
        {
            if (this._isAttackTypeL)
            {
                return base._controller.TryUseSkill(this.attackTypeL);
            }
            return base._controller.TryUseSkill(base.attackType);
        }

        protected override void DoCalcSteer()
        {
            base.CalcTargetForward();
            this.CalcIsAttackTypeL();
        }

        protected override void OnTransit(MonsterAttack.State from, MonsterAttack.State to)
        {
        }
    }
}

