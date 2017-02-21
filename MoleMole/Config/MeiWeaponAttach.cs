namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class MeiWeaponAttach : ConfigWeaponAttach
    {
        public const string LONG_PART_PATH = "LongSword";
        public const string LONG_RIGHT_ATTACH_POINT = "WeaponRightHand";
        public const string SHORT_LEFT_ATTACH_POINT = "WeaponLeftHand";
        public const string SHORT_PART_PATH = "ShortSword";
        public const string SHORT_RIGHT_ATTACH_POINT = "WeaponRightHand";
        public string WeaponEffectPattern;

        public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
        {
            return new WeaponAttach.WeaponAttachHandler(WeaponAttach.MeiAttachHandler);
        }

        public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
        {
            return new WeaponAttach.WeaponDetachHandler(WeaponAttach.MeiDetachHandler);
        }

        public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
        {
            return new WeaponAttach.RuntimeWeaponAttachHandler(WeaponAttach.MeiRuntimeAttachHandler);
        }
    }
}

