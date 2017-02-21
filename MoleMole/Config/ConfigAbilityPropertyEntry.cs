namespace MoleMole.Config
{
    using MoleMole;
    using System;

    public class ConfigAbilityPropertyEntry
    {
        public float Ceiling = float.MaxValue;
        public float Default;
        public float Floor = float.MinValue;
        public FixedFloatStack.StackMethod Stacking = FixedFloatStack.StackMethod.Sum;
        public PropertyType Type;

        public FixedSafeFloatStack CreatePropertySafeStack()
        {
            return FixedSafeFloatStack.CreateDefault(this.Default, this.Stacking, this.Floor, this.Ceiling, null);
        }

        public FixedFloatStack CreatePropertyStack()
        {
            return FixedFloatStack.CreateDefault(this.Default, this.Stacking, this.Floor, this.Ceiling, null);
        }

        public enum PropertyType
        {
            Entity,
            Actor
        }
    }
}

