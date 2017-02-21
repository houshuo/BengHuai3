namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IAIEntity
    {
        IAIController GetActiveAIController();
        float GetProperty(string key);
        uint GetRuntimeID();
        bool IsActive();

        BaseMonoEntity AttackTarget { get; }

        Vector3 FaceDirection { get; }

        Vector3 RootNodePosition { get; }

        float TimeScale { get; }

        Transform transform { get; }

        Vector3 XZPosition { get; }
    }
}

