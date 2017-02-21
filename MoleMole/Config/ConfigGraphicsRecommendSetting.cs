namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ConfigGraphicsRecommendSetting
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map0;
        public bool EnableGyroscope;
        public List<string> ExcludeDeviceModels = new List<string>();
        public Dictionary<PostEffectQualityGrade, int> PostFxGradeBufferSize = new Dictionary<PostEffectQualityGrade, int>();
        public GraphicsRecommendGrade RecommendGrade;
        public int RecommendResolutionX;
        public int RecommendResolutionY;
        public ConfigGraphicsRequirement[] Requirements;
        public Dictionary<ResolutionQualityGrade, int> ResolutionPercentage = new Dictionary<ResolutionQualityGrade, int>();
        public ResolutionQualityGrade ResolutionQuality;
        public int TargetFrameRate;

        public bool MatchRequirements()
        {
            if (this.Requirements != null)
            {
                foreach (ConfigGraphicsRequirement requirement in this.Requirements)
                {
                    bool flag;
                    string str;
                    string[] values;
                    int num2;
                    string info = requirement.Info;
                    if (info != null)
                    {
                        int num3;
                        if (<>f__switch$map0 == null)
                        {
                            Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
                            dictionary.Add("GraphicsDeviceName", 0);
                            dictionary.Add("ProcessorFrequency", 1);
                            dictionary.Add("SystemMemorySize", 2);
                            dictionary.Add("GraphicsMemorySize", 3);
                            <>f__switch$map0 = dictionary;
                        }
                        if (<>f__switch$map0.TryGetValue(info, out num3))
                        {
                            switch (num3)
                            {
                                case 0:
                                    flag = false;
                                    values = requirement.Values;
                                    num2 = 0;
                                    goto Label_00E0;

                                case 1:
                                    if (SystemInfo.processorFrequency >= int.Parse(requirement.Values[0]))
                                    {
                                        goto Label_0111;
                                    }
                                    return false;

                                case 2:
                                    if (SystemInfo.systemMemorySize >= int.Parse(requirement.Values[0]))
                                    {
                                        goto Label_0156;
                                    }
                                    return false;

                                case 3:
                                    if (SystemInfo.graphicsMemorySize >= int.Parse(requirement.Values[0]))
                                    {
                                        goto Label_019B;
                                    }
                                    return false;
                            }
                        }
                    }
                    continue;
                Label_00BA:
                    str = values[num2];
                    if (Miscs.WildcardMatch(str, SystemInfo.graphicsDeviceName, true))
                    {
                        flag = true;
                        goto Label_00EB;
                    }
                    num2++;
                Label_00E0:
                    if (num2 < values.Length)
                    {
                        goto Label_00BA;
                    }
                Label_00EB:
                    if (flag)
                    {
                        continue;
                    }
                    return false;
                Label_0111:
                    if ((requirement.Values.Length <= 1) || (SystemInfo.processorFrequency <= int.Parse(requirement.Values[1])))
                    {
                        continue;
                    }
                    return false;
                Label_0156:
                    if ((requirement.Values.Length <= 1) || (SystemInfo.systemMemorySize <= int.Parse(requirement.Values[1])))
                    {
                        continue;
                    }
                    return false;
                Label_019B:
                    if ((requirement.Values.Length > 1) && (SystemInfo.graphicsMemorySize > int.Parse(requirement.Values[1])))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}

