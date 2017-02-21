namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    public static class ConfigAtmosphereUtil
    {
        public static int ChooseRandomly(int[] rates)
        {
            int max = 0;
            foreach (int num2 in rates)
            {
                max += num2;
            }
            int num4 = UnityEngine.Random.Range(0, max);
            max = 0;
            for (int i = 0; i < rates.Length; i++)
            {
                max += rates[i];
                if (max > num4)
                {
                    return i;
                }
            }
            return UnityEngine.Random.Range(0, rates.Length);
        }
    }
}

