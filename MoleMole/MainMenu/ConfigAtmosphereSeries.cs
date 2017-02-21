namespace MoleMole.MainMenu
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class ConfigAtmosphereSeries : ScriptableObject
    {
        private int _beginKey;
        private float _beginTime;
        private float _duration;
        private bool _fistEvaluate = true;
        private bool _inTransition;
        private int _lastKey;
        private string _path;
        public ConfigAtmosphereCommon Common;
        public ConfigAtmosphere[] ConfigList;
        private static readonly float MIN_KEY_INTERVAL = 0.1f;
        [HideInInspector]
        public SortedList<float, ConfigAtmosphere> SortedConfigList = new SortedList<float, ConfigAtmosphere>();

        public void Add(ConfigAtmosphere config)
        {
            this.SortedConfigList.Add(config.FrameTime, config);
            this.ConfigList = new ConfigAtmosphere[this.SortedConfigList.Count];
            this.SortedConfigList.Values.CopyTo(this.ConfigList, 0);
        }

        public void Delete(int key)
        {
            this.SortedConfigList.RemoveAt(key);
            this.ConfigList = new ConfigAtmosphere[this.SortedConfigList.Count];
            this.SortedConfigList.Values.CopyTo(this.ConfigList, 0);
        }

        public ConfigAtmosphere Evaluate(float time, bool isEditorMode)
        {
            float num;
            float num2;
            float num5;
            if (this.SortedConfigList == null)
            {
                return null;
            }
            if (!this.GetTimeRange(time, out num, out num2))
            {
                return null;
            }
            float num3 = time - num;
            float a = num2 - num;
            if (num3 < 0f)
            {
                num3 += 24f;
            }
            if (a < 0f)
            {
                a += 24f;
            }
            if (isEditorMode)
            {
                num5 = num3 / a;
                this._lastKey = this.KeyAtTime(num);
                return ConfigAtmosphere.Lerp(this.SortedConfigList[num], this.SortedConfigList[num2], num5);
            }
            a = Mathf.Min(a, this.Common.TransitionTime / 3600f);
            int key = this.KeyBeforeTime(num - MIN_KEY_INTERVAL);
            if (a < float.Epsilon)
            {
                num5 = 1f;
            }
            else
            {
                num5 = Mathf.Clamp01(num3 / a);
            }
            this._lastKey = key;
            return ConfigAtmosphere.Lerp(this.Value(key), this.SortedConfigList[num], num5);
        }

        public bool Evaluate(float time, out ConfigAtmosphere config)
        {
            int key = this.KeyBeforeTime(time);
            if (this._fistEvaluate)
            {
                this._fistEvaluate = false;
                config = this.Value(key);
                this._lastKey = key;
                return true;
            }
            if (!this._inTransition)
            {
                if (key == this._lastKey)
                {
                    config = this.Value(key);
                    this._lastKey = key;
                    return false;
                }
                this._inTransition = true;
                this._beginTime = time;
                this._duration = 0f;
                this._beginKey = this._lastKey;
                this._lastKey = key;
            }
            else if (key != this._lastKey)
            {
                this._lastKey = key;
                this._beginTime = time - this._duration;
            }
            else
            {
                this._duration = time - this._beginTime;
            }
            float t = (this._duration * 3600f) / this.Common.TransitionTime;
            if (t > 1f)
            {
                this._inTransition = false;
                config = this.Value(key);
                this._lastKey = key;
            }
            else
            {
                config = ConfigAtmosphere.Lerp(this.Value(this._beginKey), this.Value(key), t);
            }
            return true;
        }

        public int GetSceneIdRandomly()
        {
            return this.Common.GetSceneIdRandomly();
        }

        private bool GetTimeRange(float t, out float tStart, out float tEnd)
        {
            tStart = 0f;
            tEnd = 0f;
            bool flag = false;
            bool flag2 = false;
            IEnumerator<float> enumerator = this.SortedConfigList.Keys.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    float current = enumerator.Current;
                    if (current > t)
                    {
                        tEnd = current;
                        flag2 = true;
                        goto Label_0061;
                    }
                    flag = true;
                    tStart = current;
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        Label_0061:
            if (!flag && !flag2)
            {
                return false;
            }
            if (!flag)
            {
                tStart = this.SortedConfigList.Keys[this.SortedConfigList.Keys.Count - 1];
            }
            if (!flag2)
            {
                tEnd = this.SortedConfigList.Keys[0];
            }
            return true;
        }

        public void InitAfterLoad()
        {
            this.Common.InitAfterLoad();
            if ((this.ConfigList != null) && (this.ConfigList.Length != 0))
            {
                foreach (ConfigAtmosphere atmosphere in this.ConfigList)
                {
                    atmosphere.InitAfterLoad();
                    this.SortedConfigList.Add(atmosphere.FrameTime, atmosphere);
                }
                this.ConfigList = new ConfigAtmosphere[this.SortedConfigList.Count];
                this.SortedConfigList.Values.CopyTo(this.ConfigList, 0);
            }
        }

        public bool IsValid()
        {
            return ((this.SortedConfigList != null) && (this.SortedConfigList.Count != 0));
        }

        public int KeyAtTime(float time)
        {
            if (this.IsValid())
            {
                int num7;
                float num8;
                int key = this.KeyBeforeTime(time);
                if (key == -1)
                {
                    return -1;
                }
                int num2 = key + 1;
                if (num2 == this.SortedConfigList.Count)
                {
                    num2 = 0;
                }
                float num3 = this.TimeAtKey(key);
                float num4 = this.TimeAtKey(num2);
                float num5 = Mathf.Abs((float) (time - num3));
                float num6 = Mathf.Abs((float) (num4 - time));
                if (num5 < num6)
                {
                    num7 = key;
                    num8 = num5;
                }
                else
                {
                    num7 = num2;
                    num8 = num6;
                }
                if (num8 < MIN_KEY_INTERVAL)
                {
                    return num7;
                }
            }
            return -1;
        }

        public int KeyBeforeTime(float time)
        {
            if (this.SortedConfigList.Keys.Count == 0)
            {
                return -1;
            }
            int num = -1;
            for (int i = 0; i < this.SortedConfigList.Keys.Count; i++)
            {
                if (this.SortedConfigList.Keys[i] > time)
                {
                    num = i;
                    break;
                }
            }
            if ((num != -1) && (num != 0))
            {
                return (num - 1);
            }
            return (this.SortedConfigList.Keys.Count - 1);
        }

        public int KeyCount()
        {
            return this.SortedConfigList.Count;
        }

        public static ConfigAtmosphereSeries LoadFromFile(string path)
        {
            ConfigAtmosphereSeries series = ConfigUtil.LoadConfig<ConfigAtmosphereSeries>(path);
            series.InitAfterLoad();
            series._path = path;
            return series;
        }

        public static ConfigAtmosphereSeries LoadFromFileAndDetach(string path)
        {
            ConfigAtmosphereSeries series = UnityEngine.Object.Instantiate<ConfigAtmosphereSeries>(ConfigUtil.LoadConfig<ConfigAtmosphereSeries>(path));
            series.InitAfterLoad();
            series._path = path;
            return series;
        }

        public void SetSceneId(int id)
        {
            this.Common.SceneId = id;
        }

        public float TimeAtKey(int key)
        {
            return this.SortedConfigList.Keys[key];
        }

        public ConfigAtmosphere Value(int key)
        {
            return this.ConfigList[key];
        }

        public string Path
        {
            get
            {
                return this._path;
            }
        }
    }
}

