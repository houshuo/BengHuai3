namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    [Serializable, StructLayout(LayoutKind.Sequential)]
    public struct LightProbProperties
    {
        public Color bodyColor;
        public Color shadowColor;
        public static LightProbProperties Lerp(LightProbProperties a, LightProbProperties b, float t)
        {
            return new LightProbProperties { bodyColor = Color.Lerp(a.bodyColor, b.bodyColor, t), shadowColor = Color.Lerp(a.shadowColor, b.shadowColor, t) };
        }

        public static LightProbProperties operator +(LightProbProperties a, LightProbProperties b)
        {
            return new LightProbProperties { bodyColor = a.bodyColor + b.bodyColor, shadowColor = a.shadowColor + b.shadowColor };
        }

        public static LightProbProperties operator *(LightProbProperties a, float b)
        {
            return new LightProbProperties { bodyColor = (Color) (a.bodyColor * b), shadowColor = (Color) (a.shadowColor * b) };
        }

        public static LightProbProperties operator /(LightProbProperties a, float b)
        {
            return new LightProbProperties { bodyColor = (Color) (a.bodyColor / b), shadowColor = (Color) (a.shadowColor / b) };
        }
    }
}

