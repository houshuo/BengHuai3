namespace MoleMole
{
    using System;
    using UnityEngine;

    public interface IWeaponAttacher
    {
        Transform GetAttachPoint(string name);
        bool HasAttachPoint(string name);

        GameObject gameObject { get; }
    }
}

