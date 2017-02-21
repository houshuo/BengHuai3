namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public interface IAttacker
    {
        event AnimatedHitBoxCreatedHandler onAnimatedHitBoxCreatedCallBack;

        float Evaluate(DynamicFloat target);
        int Evaluate(DynamicInt target);
        void FrameHalt(int frameNum);
        Transform GetAttachPoint(string name);
        uint GetRuntimeID();
        bool IsActive();
        bool IsToBeRemove();
        void onAnimatedHitBoxCreated(MonoAnimatedHitboxDetect hitBox, ConfigEntityAttackPattern attackPattern);

        BaseMonoEntity AttackTarget { get; }

        Vector3 FaceDirection { get; }

        Transform transform { get; }

        Vector3 XZPosition { get; }
    }
}

