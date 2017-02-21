namespace MoleMole
{
    using UnityEngine;

    public class MonoEffectOverrideSetting : MonoBehaviour
    {
        [HideInInspector]
        public ColorOverrideEntry[] colorOverrides = ColorOverrideEntry.EMPTY;
        [HideInInspector]
        public FloatOverrideEntry[] floatOverrides = FloatOverrideEntry.EMPTY;
        [HideInInspector]
        public MaterialOverrideEntry[] materialOverrides = MaterialOverrideEntry.EMPTY;
    }
}

