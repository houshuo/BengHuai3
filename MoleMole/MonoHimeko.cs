namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoHimeko : BaseMonoAvatar, IStaticHitBox
    {
        private static string HIMEKO_WEAPON_HIT_BOX_PATH = "Entities/Avatar/StaticCollider/HimekoWeaponHitBox";
        private MonoStaticHitboxDetect WeaponHitBox;
        public Transform weaponTransform;

        public MonoStaticHitboxDetect GetStaticHitBox()
        {
            return this.WeaponHitBox;
        }

        protected override void PostInit()
        {
            base.PostInit();
            this.WeaponHitBox = ((GameObject) UnityEngine.Object.Instantiate(Miscs.LoadResource<GameObject>(HIMEKO_WEAPON_HIT_BOX_PATH, BundleType.RESOURCE_FILE), this.weaponTransform.position, Quaternion.identity)).GetComponent<MonoStaticHitboxDetect>();
            this.WeaponHitBox.Init(this, AttackPattern.GetLayerMask(this), this.weaponTransform);
        }
    }
}

