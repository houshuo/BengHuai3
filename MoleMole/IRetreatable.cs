namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IRetreatable
    {
        string GetCurrentNamedState();
        float GetCurrentNormalizedTime();
        uint GetRuntimeID();
        bool IsActive();
        bool IsToBeRemove();
        void PopProperty(string propertyKey, int stackIx);
        int PushProperty(string propertyKey, float value);
        void SetNeedOverrideVelocity(bool needOverrideVelocity);
        void SetOverrideVelocity(Vector3 speed);

        Vector3 XZPosition { get; }
    }
}

