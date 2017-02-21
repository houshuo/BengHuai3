namespace MoleMole
{
    using System;
    using UnityEngine;

    public static class Bezier
    {
        public static void GetControlPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float smooth_value, ref Vector3 c1, ref Vector3 c2)
        {
            float x = p0.x;
            float y = p0.y;
            float z = p0.z;
            float num4 = p1.x;
            float num5 = p1.y;
            float num6 = p1.z;
            float num7 = p2.x;
            float num8 = p2.y;
            float num9 = p2.z;
            float num10 = p3.x;
            float num11 = p3.y;
            float num12 = p3.z;
            float num13 = (x + num4) / 2f;
            float num14 = (y + num5) / 2f;
            float num15 = (z + num6) / 2f;
            float num16 = (num4 + num7) / 2f;
            float num17 = (num5 + num8) / 2f;
            float num18 = (num6 + num9) / 2f;
            float num19 = (num7 + num10) / 2f;
            float num20 = (num8 + num11) / 2f;
            float num21 = (num9 + num12) / 2f;
            float num22 = (float) Math.Sqrt((double) ((((num4 - x) * (num4 - x)) + ((num5 - y) * (num5 - y))) + ((num6 - z) * (num6 - z))));
            float num23 = (float) Math.Sqrt((double) ((((num7 - num4) * (num7 - num4)) + ((num8 - num5) * (num8 - num5))) + ((num9 - num6) * (num9 - num6))));
            float num24 = (float) Math.Sqrt((double) ((((num10 - num7) * (num10 - num7)) + ((num11 - num8) * (num11 - num8))) + ((num12 - num9) * (num12 - num9))));
            float num25 = num22 / (num22 + num23);
            float num26 = num23 / (num23 + num24);
            float num27 = num13 + ((num16 - num13) * num25);
            float num28 = num14 + ((num17 - num14) * num25);
            float num29 = num15 + ((num18 - num15) * num25);
            float num30 = num16 + ((num19 - num16) * num26);
            float num31 = num17 + ((num20 - num17) * num26);
            float num32 = num18 + ((num21 - num18) * num26);
            smooth_value = Mathf.Clamp01(smooth_value);
            float num33 = ((num27 + ((num16 - num27) * smooth_value)) + num4) - num27;
            float num34 = ((num28 + ((num17 - num28) * smooth_value)) + num5) - num28;
            float num35 = ((num29 + ((num18 - num29) * smooth_value)) + num6) - num29;
            float num36 = ((num30 + ((num16 - num30) * smooth_value)) + num7) - num30;
            float num37 = ((num31 + ((num17 - num31) * smooth_value)) + num8) - num31;
            float num38 = ((num32 + ((num18 - num32) * smooth_value)) + num9) - num32;
            c1 = new Vector3(num33, num34, num35);
            c2 = new Vector3(num36, num37, num38);
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            return (Vector3) (((2f * (1f - t)) * (p1 - p0)) + ((2f * t) * (p2 - p1)));
        }

        public static Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float num = 1f - t;
            return (Vector3) (((((3f * num) * num) * (p1 - p0)) + (((6f * num) * t) * (p2 - p1))) + (((3f * t) * t) * (p3 - p2)));
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
        {
            t = Mathf.Clamp01(t);
            float num = 1f - t;
            return (Vector3) ((((num * num) * p0) + (((2f * num) * t) * p1)) + ((t * t) * p2));
        }

        public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            t = Mathf.Clamp01(t);
            float num = 1f - t;
            return (Vector3) ((((((num * num) * num) * p0) + ((((3f * num) * num) * t) * p1)) + ((((3f * num) * t) * t) * p2)) + (((t * t) * t) * p3));
        }
    }
}

