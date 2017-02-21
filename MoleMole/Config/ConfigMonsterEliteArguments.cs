namespace MoleMole.Config
{
    using System;
    using UnityEngine;

    public class ConfigMonsterEliteArguments
    {
        public float AttackRatio = 1f;
        public float DebuffResistanceRatio;
        public float DefenseRatio = 1f;
        [NonSerialized]
        public Color EliteColor1;
        [NonSerialized]
        public Color EliteColor2;
        public float EliteEmissionScaler1 = 1f;
        public float EliteEmissionScaler2 = 1f;
        public float EliteNormalDisplacement1 = 0.02f;
        public float EliteNormalDisplacement2 = 0.04f;
        public string HexColorElite1 = "#FFCC00B9";
        public string HexColorElite2 = "#FFD84449";
        public float HPRatio = 1f;
    }
}

