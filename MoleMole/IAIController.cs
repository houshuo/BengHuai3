namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IAIController
    {
        void SetActive(bool active);
        void TryClearAttackTarget();
        void TryMove(float speed);
        void TryMoveHorizontal(float speed);
        void TrySetAttackTarget(BaseMonoEntity attackTarget);
        void TrySteer(Vector3 dir);
        void TrySteer(Vector3 dir, float lerpRatio);
        void TrySteerInstant(Vector3 dir);
        void TryStop();
        bool TryUseSkill(string skillName);
    }
}

