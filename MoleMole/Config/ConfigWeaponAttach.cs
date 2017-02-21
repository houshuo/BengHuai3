namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public abstract class ConfigWeaponAttach
    {
        [NonSerialized]
        public string PrefabPath;

        protected ConfigWeaponAttach()
        {
        }

        public abstract WeaponAttach.WeaponAttachHandler GetAttachHandler();
        public abstract WeaponAttach.WeaponDetachHandler GetDetachHandler();
        public abstract WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler();
    }
}

