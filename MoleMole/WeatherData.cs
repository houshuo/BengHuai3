namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public class WeatherData
    {
        private static Dictionary<string, ConfigWeatherData> _weatherDataDict;

        public static ConfigWeatherData GetWeatherDataConfig(string name)
        {
            return _weatherDataDict[name];
        }

        public static void ReloadFromFile()
        {
            _weatherDataDict = new Dictionary<string, ConfigWeatherData>();
            string[] weatherEntryPathes = GlobalDataManager.metaConfig.weatherEntryPathes;
            for (int i = 0; i < weatherEntryPathes.Length; i++)
            {
                ConfigWeatherRegistry registry = ConfigUtil.LoadConfig<ConfigWeatherRegistry>(weatherEntryPathes[i]);
                if (registry.entries != null)
                {
                    for (int j = 0; j < registry.entries.Length; j++)
                    {
                        ConfigWeatherEntry entry = registry.entries[j];
                        ConfigWeather config = Miscs.LoadResource<ConfigWeather>(entry.dataPath, BundleType.RESOURCE_FILE);
                        _weatherDataDict.Add(entry.name, ConfigWeatherData.LoadFromFile(config));
                    }
                }
            }
        }

        [DebuggerHidden]
        public static IEnumerator ReloadFromFileAsync(float progressSpan = 0, Action<float> moveOneStepCallback = null)
        {
            return new <ReloadFromFileAsync>c__Iterator15 { progressSpan = progressSpan, moveOneStepCallback = moveOneStepCallback, <$>progressSpan = progressSpan, <$>moveOneStepCallback = moveOneStepCallback };
        }

        [CompilerGenerated]
        private sealed class <ReloadFromFileAsync>c__Iterator15 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<float> <$>moveOneStepCallback;
            internal float <$>progressSpan;
            internal AsyncAssetRequst <asyncRequest>__3;
            internal ConfigWeather <configWeather>__7;
            internal ConfigWeatherEntry <entry>__6;
            internal int <ix>__2;
            internal int <jx>__5;
            internal float <step>__1;
            internal ConfigWeatherRegistry <weatherRegistry>__4;
            internal string[] <weatherRegistryPathes>__0;
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
                        WeatherData._weatherDataDict = new Dictionary<string, ConfigWeatherData>();
                        this.<weatherRegistryPathes>__0 = GlobalDataManager.metaConfig.weatherEntryPathes;
                        this.<step>__1 = this.progressSpan / ((float) this.<weatherRegistryPathes>__0.Length);
                        this.<ix>__2 = 0;
                        goto Label_01DA;

                    case 1:
                        if (this.moveOneStepCallback != null)
                        {
                            this.moveOneStepCallback(this.<step>__1);
                        }
                        this.<weatherRegistry>__4 = (ConfigWeatherRegistry) this.<asyncRequest>__3.asset;
                        SuperDebug.VeryImportantAssert(this.<weatherRegistry>__4 != null, "weatherRegistry is null weatherRegistryPath :" + this.<weatherRegistryPathes>__0[this.<ix>__2]);
                        if ((this.<weatherRegistry>__4 != null) && (this.<weatherRegistry>__4.entries != null))
                        {
                            this.<jx>__5 = 0;
                            while (this.<jx>__5 < this.<weatherRegistry>__4.entries.Length)
                            {
                                this.<entry>__6 = this.<weatherRegistry>__4.entries[this.<jx>__5];
                                this.<configWeather>__7 = Miscs.LoadResource<ConfigWeather>(this.<entry>__6.dataPath, BundleType.RESOURCE_FILE);
                                WeatherData._weatherDataDict.Add(this.<entry>__6.name, ConfigWeatherData.LoadFromFile(this.<configWeather>__7));
                                this.<jx>__5++;
                            }
                        }
                        break;

                    default:
                        goto Label_01F4;
                }
            Label_01CC:
                this.<ix>__2++;
            Label_01DA:
                if (this.<ix>__2 < this.<weatherRegistryPathes>__0.Length)
                {
                    this.<asyncRequest>__3 = ConfigUtil.LoadConfigAsync(this.<weatherRegistryPathes>__0[this.<ix>__2], BundleType.RESOURCE_FILE);
                    SuperDebug.VeryImportantAssert(this.<asyncRequest>__3 != null, "assetRequest is null weatherRegistryPath :" + this.<weatherRegistryPathes>__0[this.<ix>__2]);
                    if (this.<asyncRequest>__3 != null)
                    {
                        this.$current = this.<asyncRequest>__3.operation;
                        this.$PC = 1;
                        return true;
                    }
                    goto Label_01CC;
                }
                this.$PC = -1;
            Label_01F4:
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

