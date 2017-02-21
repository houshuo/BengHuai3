namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ConfigWeapon
    {
        public ConfigEntityWeaponAdditionalAbilityEntry[] AdditionalAbilities = ConfigEntityWeaponAdditionalAbilityEntry.EMPTY;
        public ConfigWeaponAttach Attach;
        public ConfigEffectOverlayEntry[] EffectOverlays = ConfigEffectOverlayEntry.EMPTY;
        public ConfigEffectOverlayEntry[] EffectOverrides = ConfigEffectOverlayEntry.EMPTY;
        [NonSerialized]
        public WeaponMetaData Meta;
        public EntityRoleName OwnerRole;
        public int WeaponID;
    }
}

