namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IFadeOff
    {
        Material[] GetAllMaterials();
        uint GetRuntimeID();
        float GetTargetAlpha();
        bool IsActive();
        bool IsToBeRemove();

        Vector3 XZPosition { get; }
    }
}

