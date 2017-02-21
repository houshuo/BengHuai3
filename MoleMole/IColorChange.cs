namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IColorChange
    {
        Material[] GetAllMaterials();
        uint GetRuntimeID();
        bool IsActive();
        bool IsToBeRemove();

        Color EmissionColor { get; set; }

        Vector3 XZPosition { get; }
    }
}

