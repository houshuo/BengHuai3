namespace MoleMole.Config
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class ConfigAnimatorEntity
    {
        public ConfigBindAnimatorStateToParameter[] AnimatorStateParamBinds = ConfigBindAnimatorStateToParameter.EMPTY;
        [NonSerialized]
        public ConfigCommonEntity CommonConfig;
        public ConfigMPArguments MPArguments;
        [NonSerialized]
        public Dictionary<int, AnimatorStateToParameterConfig> StateToParamBindMap;

        public virtual void OnLevelLoaded()
        {
            this.StateToParamBindMap = new Dictionary<int, AnimatorStateToParameterConfig>();
            foreach (ConfigBindAnimatorStateToParameter parameter in this.AnimatorStateParamBinds)
            {
                foreach (string str in parameter.AnimatorStateNames)
                {
                    this.StateToParamBindMap.Add(Animator.StringToHash(str), parameter.ParameterConfig);
                }
            }
        }
    }
}

