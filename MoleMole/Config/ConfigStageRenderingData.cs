namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConfigStageRenderingData : ConfigBaseRenderingData
    {
        private static BaseRenderingProperty[] DEFAULT_RENDERING_PROPERTIES = new BaseRenderingProperty[] { 
            new ColorRenderingProperty("_FogColorNear", new Color(0.66f, 0.65f, 0.99f, 1f)), new ColorRenderingProperty("_FogColorFar", new Color(0.66f, 0.65f, 0.99f, 1f)), new FloatRenderingProperty("_FogIntensity", 0.01f, 0f, 0.4f), new FloatRenderingProperty("_FogColorIntensity", 0.01f, 0f, 0.2f), new FloatRenderingProperty("_FogEffectStart", 0f, 0f, 1f), new FloatRenderingProperty("_FogEffectLimit", 0f, 0f, 1f), new FloatRenderingProperty("_SkyBoxFogEffectLimit", 0f, 0f, 1f), new FloatRenderingProperty("_SkyBoxBloomFactor", 1f, 0f, 100f), new ColorRenderingProperty("_SkyBoxColor", new Color(1f, 1f, 1f, 1f)), new FloatRenderingProperty("_SkyBoxColorScaler", 1f, 0f, 10f), new FloatRenderingProperty("_FogStartDistance", 30f, 0f, 10f), new ColorRenderingProperty("_SkyBoxTexRColor", (Color) new Color32(0xff, 0xff, 0xff, 0xff)), new ColorRenderingProperty("_SkyBoxTexGColor", (Color) new Color32(0xa8, 0xcc, 0xea, 0xff)), new ColorRenderingProperty("_SkyBoxTexBColor", (Color) new Color32(0x39, 0xf4, 0xd5, 0xff)), new FloatRenderingProperty("_SkyBoxTexXLocation", 0.177f, 0f, 1f), new FloatRenderingProperty("_SkyBoxTexYLocation", 0.092f, 0f, 1f), 
            new FloatRenderingProperty("_SkyBoxTexHigh", 1f, 0.01f, 1f), new ColorRenderingProperty("_SkyBoxGradBottomColor", (Color) new Color32(0x58, 0xca, 0xe1, 0xff)), new ColorRenderingProperty("_SkyBoxGradTopColor", (Color) new Color32(0x19, 0x43, 0xa5, 0xff)), new FloatRenderingProperty("_SkyBoxGradLocation", 0.36f, -5f, 1f), new FloatRenderingProperty("_SkyBoxGradHigh", 0.1f, 0.01f, 10f), new ColorRenderingProperty("_SkyBoxMountainColor", new Color(1f, 1f, 1f, 1f))
         };

        public static void CompleteStageRenderingDataWithDefault(ConfigStageRenderingData data)
        {
            List<BaseRenderingProperty> list = new List<BaseRenderingProperty>(data.properties);
            foreach (BaseRenderingProperty property in DEFAULT_RENDERING_PROPERTIES)
            {
                bool flag = false;
                foreach (BaseRenderingProperty property2 in data.properties)
                {
                    if (property.propertyName == property2.propertyName)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    list.Add(property);
                }
            }
            data.properties = list.ToArray();
        }

        public static ConfigStageRenderingData CreateDefault()
        {
            ConfigStageRenderingData data = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
            data.properties = new BaseRenderingProperty[DEFAULT_RENDERING_PROPERTIES.Length];
            for (int i = 0; i < DEFAULT_RENDERING_PROPERTIES.Length; i++)
            {
                data.properties[i] = DEFAULT_RENDERING_PROPERTIES[i].Clone();
            }
            return data;
        }

        public static ConfigStageRenderingData CreateStageRenderingDataFromInstancedProperties(BaseInstancedRenderingProperty[] properties)
        {
            ConfigStageRenderingData data = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
            HashSet<string> set = new HashSet<string>();
            List<BaseRenderingProperty> list = new List<BaseRenderingProperty>();
            foreach (BaseInstancedRenderingProperty property in properties)
            {
                foreach (BaseRenderingProperty property2 in DEFAULT_RENDERING_PROPERTIES)
                {
                    if ((Shader.PropertyToID(property2.propertyName) == property.propertyID) && !set.Contains(property2.propertyName))
                    {
                        list.Add(property.CreateBaseRenderingProperty(property2.propertyName));
                        set.Add(property2.propertyName);
                    }
                }
            }
            data.properties = list.ToArray();
            return data;
        }

        public static ConfigStageRenderingData CreateStageRenderingDataFromMaterials(Material[] mats)
        {
            ConfigStageRenderingData data = ScriptableObject.CreateInstance<ConfigStageRenderingData>();
            HashSet<string> set = new HashSet<string>();
            List<BaseRenderingProperty> list = new List<BaseRenderingProperty>();
            foreach (Material material in mats)
            {
                foreach (BaseRenderingProperty property in DEFAULT_RENDERING_PROPERTIES)
                {
                    if (material.HasProperty(property.propertyName) && !set.Contains(property.propertyName))
                    {
                        if (property is ColorRenderingProperty)
                        {
                            list.Add(new ColorRenderingProperty(property.propertyName, material.GetColor(property.propertyName)));
                        }
                        else if (property is FloatRenderingProperty)
                        {
                            list.Add(new FloatRenderingProperty(property.propertyName, material.GetFloat(property.propertyName), 0f, 100f));
                        }
                        set.Add(property.propertyName);
                    }
                }
            }
            data.properties = list.ToArray();
            return data;
        }

        public static BaseRenderingProperty[] GetDefaultProperties()
        {
            return DEFAULT_RENDERING_PROPERTIES;
        }
    }
}

