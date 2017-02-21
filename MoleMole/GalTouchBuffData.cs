namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class GalTouchBuffData
    {
        public static Dictionary<int, ConfigGalTouchBuffEntry> _galTouchBuffMap;

        public static void ApplyGalTouchBuffEntry(AvatarActor avatarActor, int buffId, float calculatedParam1, float calculatedParam2, float calculatedParam3)
        {
            if (buffId > 0)
            {
                ConfigGalTouchBuffEntry galTouchBuffEntry = GetGalTouchBuffEntry(buffId);
                ConfigAbility abilityConfig = AbilityData.GetAbilityConfig(galTouchBuffEntry.AbilityName, galTouchBuffEntry.AbilityOverride);
                Dictionary<string, object> overrideMap = avatarActor.CreateAppliedAbility(abilityConfig);
                if (galTouchBuffEntry.ParamSpecial1 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial1, galTouchBuffEntry.ParamMethod1, calculatedParam1);
                }
                if (galTouchBuffEntry.ParamSpecial2 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial2, galTouchBuffEntry.ParamMethod2, calculatedParam2);
                }
                if (galTouchBuffEntry.ParamSpecial3 != null)
                {
                    AbilityData.SetupParamSpecial(abilityConfig, overrideMap, galTouchBuffEntry.ParamSpecial3, galTouchBuffEntry.ParamMethod3, calculatedParam3);
                }
            }
        }

        public static float GetCalculatedParam(float baseParam, float addParam, int level)
        {
            return (baseParam + (addParam * (level - 1)));
        }

        public static ConfigGalTouchBuffEntry GetGalTouchBuffEntry(int buffId)
        {
            if (_galTouchBuffMap == null)
            {
                return null;
            }
            if (!_galTouchBuffMap.ContainsKey(buffId))
            {
                return null;
            }
            return _galTouchBuffMap[buffId];
        }

        public static void ReloadFromFile()
        {
            _galTouchBuffMap = new Dictionary<int, ConfigGalTouchBuffEntry>();
            foreach (string str in GlobalDataManager.metaConfig.galTouchBuffRegistryPathes)
            {
                foreach (ConfigGalTouchBuffEntry entry in ConfigUtil.LoadJSONConfig<ConfigGalTouchBuffRegistry>(str))
                {
                    _galTouchBuffMap.Add(entry.GalTouchBuffID, entry);
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__IteratorD { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__IteratorD : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal string[] <$s_909>__1;
            internal int <$s_910>__2;
            internal List<ConfigGalTouchBuffEntry>.Enumerator <$s_911>__6;
            internal AsyncAssetRequst <asyncRequest>__4;
            internal ConfigGalTouchBuffEntry <buff>__7;
            internal ConfigGalTouchBuffRegistry <buffList>__5;
            internal string <path>__3;
            internal float <step>__0;
            internal Action<float> moveOneStepCallback;
            internal float progressSpan;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        GalTouchBuffData._galTouchBuffMap = new Dictionary<int, ConfigGalTouchBuffEntry>();
                        this.<step>__0 = this.progressSpan / ((float) GlobalDataManager.metaConfig.galTouchBuffRegistryPathes.Length);
                        this.<$s_909>__1 = GlobalDataManager.metaConfig.galTouchBuffRegistryPathes;
                        this.<$s_910>__2 = 0;
                        goto Label_0181;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__0);
                        }
                        this.<buffList>__5 = ConfigUtil.LoadJSONStrConfig<ConfigGalTouchBuffRegistry>(this.<asyncRequest>__4.asset.ToString());
                        this.<$s_911>__6 = this.<buffList>__5.GetEnumerator();
                        try
                        {
                            while (this.<$s_911>__6.MoveNext())
                            {
                                this.<buff>__7 = this.<$s_911>__6.Current;
                                GalTouchBuffData._galTouchBuffMap.Add(this.<buff>__7.GalTouchBuffID, this.<buff>__7);
                            }
                        }
                        finally
                        {
                            this.<$s_911>__6.Dispose();
                        }
                        break;

                    default:
                        goto Label_019B;
                }
            Label_0173:
                this.<$s_910>__2++;
            Label_0181:
                if (this.<$s_910>__2 < this.<$s_909>__1.Length)
                {
                    this.<path>__3 = this.<$s_909>__1[this.<$s_910>__2];
                    this.<asyncRequest>__4 = ConfigUtil.LoadJsonConfigAsync(this.<path>__3, BundleType.DATA_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__4 != null, "assetRequest is null galTouchBuffRegistryPath :" + this.<path>__3);
                    if (this.<asyncRequest>__4 != null)
                    {
                        this.$current = this.<asyncRequest>__4.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_0173;
                }
                this.$PC = -1;
            Label_019B:
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

