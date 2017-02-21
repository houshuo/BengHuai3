namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class BronyaWeaponAttach : ConfigWeaponAttach
    {
        public const string AVATAR_GUN_ATTACH_POINT_NAME = "GunPoint";
        public const string GUN_ATTACH_POINT_TRANSFORM = "GunIn/GunPointAttach";
        public const string IN_ATTACH_POINT = "WeaponLeftArmIn";
        public const string IN_PART_PATH = "GunIn";
        public const string LAYER = "Weapon";
        public const string MATERIAL_SOURCE_NAME = "MC_Body";
        public const string OUT_ATTACH_POINT = "WeaponLeftArmOut";
        public const string OUT_PART_PATH = "GunOut";
        public const string SHADER_NAME = "miHoYo/Character/Machine";
        public string WeaponEffectPattern;

        public override WeaponAttach.WeaponAttachHandler GetAttachHandler()
        {
            return new WeaponAttach.WeaponAttachHandler(WeaponAttach.BronyaAttachHandler);
        }

        public override WeaponAttach.WeaponDetachHandler GetDetachHandler()
        {
            return new WeaponAttach.WeaponDetachHandler(WeaponAttach.BronyaDetachHandler);
        }

        public override WeaponAttach.RuntimeWeaponAttachHandler GetRuntimeWeaponAttachHandler()
        {
            return new WeaponAttach.RuntimeWeaponAttachHandler(WeaponAttach.BronyaRuntimeAttachHandler);
        }
    }
}

