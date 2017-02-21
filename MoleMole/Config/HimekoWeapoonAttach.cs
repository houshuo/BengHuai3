namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class HimekoWeapoonAttach : ConfigWeaponAttach
    {
        public const string SWORD_ATTACH_POINT = "WeaponRightHand";
        public const string SWORD_PATH = "Sword";

        public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
        {
            return new WeaponAttach.WeaponAttachHandler(WeaponAttach.HimekoAttachHandler);
        }

        public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
        {
            return new WeaponAttach.WeaponDetachHandler(WeaponAttach.HimekoDetachHandler);
        }

        public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
        {
            return new WeaponAttach.RuntimeWeaponAttachHandler(WeaponAttach.HimekoRuntimeAttachHandler);
        }
    }
}

