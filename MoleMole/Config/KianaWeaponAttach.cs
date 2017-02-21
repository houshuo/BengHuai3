namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class KianaWeaponAttach : ConfigWeaponAttach
    {
        public const string AVATAR_LEFT_GUN_ATTACH_POINT_NAME = "LeftGunPoint";
        public const string AVATAR_RIGHT_GUN_ATTACH_POINT_NAME = "RightGunPoint";
        public const string LEFT_ATTACH_POINT = "WeaponLeftHand";
        public const string LEFT_GUN_ATTACH_POINT_TRANSFORM = "LeftPistol/LeftGunPoint";
        public const string LEFT_PART_PATH = "LeftPistol";
        public const string RIGHT_ATTACH_POINT = "WeaponRightHand";
        public const string RIGHT_GUN_ATTACH_POINT_TRANSFORM = "RightPistol/RightGunPoint";
        public const string RIGHT_PART_PATH = "RightPistol";
        public string WeaponEffectPattern;

        public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
        {
            return new WeaponAttach.WeaponAttachHandler(WeaponAttach.KianaAttachHandler);
        }

        public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
        {
            return new WeaponAttach.WeaponDetachHandler(WeaponAttach.KianaDetachHandler);
        }

        public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
        {
            return new WeaponAttach.RuntimeWeaponAttachHandler(WeaponAttach.KianaRuntimeAttachHandler);
        }
    }
}

