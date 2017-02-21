namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ConfigEntityCameraShake : IndexedConfig<ConfigEntityCameraShake>
    {
        public bool ClearPreviousShake;
        public float? ShakeAngle;
        public bool ShakeOnNotHit;
        public float ShakeRange;
        public int ShakeStepFrame = 1;
        public float ShakeTime;

        public override int CompareTo(ConfigEntityCameraShake other)
        {
            if (other == null)
            {
                return 1;
            }
            int num = this.ShakeOnNotHit.CompareTo(other.ShakeOnNotHit);
            if (num != 0)
            {
                return num;
            }
            num = this.ShakeRange.CompareTo(other.ShakeRange);
            if (num != 0)
            {
                return num;
            }
            num = this.ShakeTime.CompareTo(other.ShakeTime);
            if (num != 0)
            {
                return num;
            }
            num = this.ShakeAngle.HasValue.CompareTo(other.ShakeAngle.HasValue);
            if (num != 0)
            {
                return num;
            }
            if (this.ShakeAngle.HasValue && other.ShakeAngle.HasValue)
            {
                num = this.ShakeAngle.Value.CompareTo(other.ShakeAngle.Value);
                if (num != 0)
                {
                    return num;
                }
            }
            num = this.ShakeStepFrame.CompareTo(other.ShakeStepFrame);
            if (num != 0)
            {
                return num;
            }
            return this.ClearPreviousShake.CompareTo(other.ClearPreviousShake);
        }

        public override int ContentHash()
        {
            int lastHash = 0;
            HashUtils.ContentHashOnto(this.ShakeOnNotHit, ref lastHash);
            HashUtils.ContentHashOnto(this.ShakeRange, ref lastHash);
            HashUtils.ContentHashOnto(this.ShakeTime, ref lastHash);
            HashUtils.ContentHashOnto(!this.ShakeAngle.HasValue ? 0f : this.ShakeAngle.Value, ref lastHash);
            HashUtils.ContentHashOnto(this.ShakeStepFrame, ref lastHash);
            HashUtils.ContentHashOnto(this.ClearPreviousShake, ref lastHash);
            return lastHash;
        }
    }
}

