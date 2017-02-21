namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimatorParameterEntry
    {
        public int stateHash;
        public AnimatorControllerParameterType type;
        public int intValue;
        public float floatValue;
        public bool boolValue;
        public override string ToString()
        {
            object boolValue = null;
            if (this.type == AnimatorControllerParameterType.Bool)
            {
                boolValue = this.boolValue;
            }
            else if (this.type == AnimatorControllerParameterType.Int)
            {
                boolValue = this.intValue;
            }
            else if (this.type == AnimatorControllerParameterType.Float)
            {
                boolValue = this.floatValue;
            }
            return string.Format("param {0}, {1}", this.stateHash, boolValue);
        }
    }
}

