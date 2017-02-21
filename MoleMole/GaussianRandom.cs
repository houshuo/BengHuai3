namespace MoleMole
{
    using System;
    using UnityEngine;

    public static class GaussianRandom
    {
        private static bool isSpareReady;
        private static float spare;

        public static float Val(float mean, float stdDev)
        {
            float num;
            float num2;
            float num3;
            if (isSpareReady)
            {
                isSpareReady = false;
                return ((spare * stdDev) + mean);
            }
            do
            {
                num = (UnityEngine.Random.value * 2f) - 1f;
                num2 = (UnityEngine.Random.value * 2f) - 1f;
                num3 = (num * num) + (num2 * num2);
            }
            while ((num3 >= 1f) || (num3 == 0f));
            float num4 = Mathf.Sqrt((-2f * Mathf.Log(num3)) / num3);
            spare = num2 * num4;
            isSpareReady = true;
            return (mean + ((stdDev * num) * num4));
        }
    }
}

