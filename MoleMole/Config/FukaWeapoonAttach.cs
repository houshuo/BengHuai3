namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class FukaWeapoonAttach : ConfigWeaponAttach
    {
        public const string TAIL_ATTACH_POINT = "WeaponTail";
        public const string TAIL_PATH = "Tail";

        public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
        {
            return new WeaponAttach.WeaponAttachHandler(WeaponAttach.FukaAttachHandler);
        }

        public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
        {
            return new WeaponAttach.WeaponDetachHandler(WeaponAttach.FukaDetachHandler);
        }

        public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
        {
            return new WeaponAttach.RuntimeWeaponAttachHandler(WeaponAttach.FukaRuntimeAttachHandler);
        }
    }
}

