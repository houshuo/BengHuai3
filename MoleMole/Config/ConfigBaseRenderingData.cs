namespace MoleMole.Config
{
    using FullInspector;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConfigBaseRenderingData : BaseScriptableObject
    {
        public BaseRenderingProperty[] properties;

        public virtual void ApplyGlobally()
        {
            for (int i = 0; i < this.properties.Length; i++)
            {
                this.properties[i].ApplyGlobally();
            }
        }

        public virtual ConfigBaseRenderingData Clone()
        {
            return UnityEngine.Object.Instantiate<ConfigStageRenderingData>(this as ConfigStageRenderingData);
        }

        public void CopyFrom(ConfigBaseRenderingData source)
        {
            this.properties = new BaseRenderingProperty[source.properties.Length];
            for (int i = 0; i < this.properties.Length; i++)
            {
                this.properties[i] = source.properties[i].Clone();
            }
        }

        public virtual void LerpStep(float t)
        {
            for (int i = 0; i < this.properties.Length; i++)
            {
                this.properties[i].LerpStep(t);
            }
        }

        public virtual void SetupTransition(ConfigBaseRenderingData target)
        {
            Dictionary<string, BaseRenderingProperty> dictionary = new Dictionary<string, BaseRenderingProperty>();
            for (int i = 0; i < target.properties.Length; i++)
            {
                dictionary.Add(target.properties[i].propertyName, target.properties[i]);
            }
            for (int j = 0; j < this.properties.Length; j++)
            {
                if (dictionary.ContainsKey(this.properties[j].propertyName))
                {
                    this.properties[j].SetupTransition(dictionary[this.properties[j].propertyName]);
                }
                else
                {
                    this.properties[j].SetupTransition(null);
                }
            }
        }
    }
}

