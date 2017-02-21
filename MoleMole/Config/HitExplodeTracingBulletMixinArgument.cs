namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class HitExplodeTracingBulletMixinArgument : IndexedConfig<HitExplodeTracingBulletMixinArgument>, IMixinArgument
    {
        public string BulletName;
        public DynamicFloat BulletSpeed;
        public string[] RandomBulletNames;
        public float XZAngleOffset;

        public override int CompareTo(HitExplodeTracingBulletMixinArgument other)
        {
            if (other == null)
            {
                return 1;
            }
            int num = IndexedConfig.Compare(this.BulletName, other.BulletName);
            if (num != 0)
            {
                return num;
            }
            num = this.XZAngleOffset.CompareTo(other.XZAngleOffset);
            if (num != 0)
            {
                return num;
            }
            num = IndexedConfig.Compare(this.RandomBulletNames, other.RandomBulletNames);
            if (num != 0)
            {
                return num;
            }
            return IndexedConfig.Compare(this.BulletSpeed, other.BulletSpeed);
        }

        public override int ContentHash()
        {
            int lastHash = 0;
            HashUtils.ContentHashOnto(this.BulletName, ref lastHash);
            HashUtils.ContentHashOnto(this.XZAngleOffset, ref lastHash);
            if (this.RandomBulletNames != null)
            {
                for (int i = 0; i < this.RandomBulletNames.Length; i++)
                {
                    HashUtils.ContentHashOnto(this.RandomBulletNames[i], ref lastHash);
                }
            }
            if (this.BulletSpeed != null)
            {
                HashUtils.ContentHashOnto(this.BulletSpeed.isDynamic, ref lastHash);
                HashUtils.ContentHashOnto(this.BulletSpeed.dynamicKey, ref lastHash);
                HashUtils.ContentHashOnto(this.BulletSpeed.fixedValue, ref lastHash);
            }
            return 0;
        }
    }
}

