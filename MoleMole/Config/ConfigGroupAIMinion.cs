namespace MoleMole.Config
{
    using MoleMole;
    using System;
    using System.Collections.Generic;

    public class ConfigGroupAIMinion : IOnLoaded
    {
        public string AIName;
        public ConfigDynamicArguments AIParams = ConfigDynamicArguments.EMPTY;
        public ConfigDynamicArguments AISpecials = ConfigDynamicArguments.EMPTY;
        private static Dictionary<string, ConfigGroupAIMinionParam[]> EMPTY_TRIGGER_ACTIONS = new Dictionary<string, ConfigGroupAIMinionParam[]>();
        public string MonsterName;
        public Dictionary<string, ConfigGroupAIMinionParam[]> TriggerActions = EMPTY_TRIGGER_ACTIONS;

        public void OnLoaded()
        {
            this.PopulateDynamicArguments(this.AISpecials, this.AIParams);
            foreach (ConfigGroupAIMinionParam[] paramArray in this.TriggerActions.Values)
            {
                foreach (ConfigGroupAIMinionParam param in paramArray)
                {
                    this.PopulateDynamicFloat(this.AISpecials, param.Delay);
                    this.PopulateDynamicArguments(this.AISpecials, param.AIParams);
                }
            }
        }

        private void PopulateDynamicArguments(ConfigDynamicArguments source, ConfigDynamicArguments target)
        {
            List<string> list = new List<string>(target.Keys);
            foreach (string str in list)
            {
                string str2 = target[str] as string;
                if ((str2 != null) && (str2[0] == '%'))
                {
                    string str3 = str2.Substring(1);
                    target[str] = source[str3];
                }
            }
        }

        private void PopulateDynamicFloat(ConfigDynamicArguments source, DynamicFloat df)
        {
            if (df.isDynamic)
            {
                object obj2 = source[df.dynamicKey];
                if (obj2 is int)
                {
                    df.fixedValue = (int) obj2;
                }
                else
                {
                    df.fixedValue = (float) obj2;
                }
                df.isDynamic = false;
                df.dynamicKey = null;
            }
        }
    }
}

