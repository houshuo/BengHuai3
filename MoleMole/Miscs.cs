namespace MoleMole
{
    using SimpleJSON;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public static class Miscs
    {
        private static bool _bEnableDraw = false;
        private static List<RenderLineInfo> _renderLines = null;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map6;
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map7;
        public const string AVATARCARD_ICON_PREFAB_PATH = "SpriteOutput/AvatarCardIcons";
        public const string AVATARFRAGMENT_ICON_PREFAB_PATH = "SpriteOutput/AvatarFragmentIcons";
        public const float DEGREE_LERP_END_DIFF = 2f;
        public static string[] EMPTY_STRINGS = new string[0];
        public const float EPSILON = 0.0001f;
        public const string EXP_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconEXP";
        public const string FRIENDPOINT_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconFP";
        public const string HCOIN_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconHC";
        public const float LENGTH_LERP_END_DIFF = 0.1f;
        public const string MATERIAL_ICON_PREFAB_PATH = "SpriteOutput/MaterialIcons";
        public const string SCOIN_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconSC";
        public const string SKILLPOINT_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconSP";
        public const string STAMINA_ICON_PREFAB_PATH = "SpriteOutput/GeneralIcon/IconST";
        public const string STIGMATA_ICON_PREFAB_PATH = "SpriteOutput/StigmataIcons";
        public const float VERY_BIG_FLOAT = 999999f;
        public const string WEAPON_ICON_PREFAB_PATH = "SpriteOutput/WeaponIcons";

        public static float AbsAngleDiff(float from, float to)
        {
            return Mathf.Abs(SignedAngleDiff(from, to));
        }

        public static float AngleForVec3IgnoreY(Vector3 DirFrom, Vector3 DirTo)
        {
            Vector3 vector = new Vector3(DirFrom.x, 0f, DirFrom.z);
            Vector3 vector2 = new Vector3(DirTo.x, 0f, DirTo.z);
            return Mathf.Acos(Vector3.Dot(vector.normalized, vector2.normalized));
        }

        public static float AngleFromToIgnoreY(Vector3 dirFrom, Vector3 dirTo)
        {
            dirFrom.y = 0f;
            dirTo.y = 0f;
            float num = Vector3.Angle(dirFrom, dirTo);
            float num2 = Mathf.Sign(Vector3.Dot(Vector3.up, Vector3.Cross(dirFrom, dirTo)));
            return (num * num2);
        }

        public static float AngleToGround(Vector3 dir)
        {
            Vector3 to = Vector3.ProjectOnPlane(dir, Vector3.up);
            float num = Vector3.Angle(dir, to);
            return ((dir.y <= 0f) ? -num : num);
        }

        public static void ArrayAppend<T>(ref T[] array, T element)
        {
            T[] destinationArray = new T[array.Length + 1];
            Array.Copy(array, destinationArray, array.Length);
            destinationArray[destinationArray.Length - 1] = element;
            array = destinationArray;
        }

        public static void ArrayClearOutNulls<T>(ref T[] array)
        {
            List<T> list = new List<T>();
            foreach (T local in array)
            {
                if (local != null)
                {
                    list.Add(local);
                }
            }
            array = list.ToArray();
        }

        public static bool ArrayContains<T>(T[] array, T element)
        {
            return (Array.IndexOf<T>(array, element) != -1);
        }

        public static int ArrayRefCountOf<T>(T[] arr, T element) where T: class
        {
            int num = 0;
            for (int i = 0; i < arr.Length; i++)
            {
                if (object.ReferenceEquals(element, arr[i]))
                {
                    num++;
                }
            }
            return num;
        }

        public static bool CheckOutsideWallAndDrag(Transform transform)
        {
            RaycastHit hit;
            BaseMonoAvatar localAvatar = Singleton<AvatarManager>.Instance.GetLocalAvatar();
            if (localAvatar == null)
            {
                return false;
            }
            bool flag = false;
            Vector3 start = new Vector3(localAvatar.transform.position.x, 0.1f, localAvatar.transform.position.z);
            Vector3 end = new Vector3(transform.position.x, 0.1f, transform.position.z);
            if (Physics.Linecast(start, end, out hit, ((int) 1) << InLevelData.OBSTACLE_COLLIDER_LAYER))
            {
                flag = true;
            }
            if ((!flag && Physics.Linecast(start, end, out hit, ((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)) && !Physics.Linecast(end, start, (int) (((int) 1) << InLevelData.STAGE_COLLIDER_LAYER)))
            {
                flag = true;
            }
            if (flag)
            {
                Vector3 point = hit.point;
                Vector3 vector10 = start - end;
                Vector3 normalized = vector10.normalized;
                CapsuleCollider componentInChildren = transform.GetComponentInChildren<CapsuleCollider>();
                float num = 0f;
                if (componentInChildren != null)
                {
                    num = componentInChildren.radius + 0.1f;
                }
                else
                {
                    num = 0.5f;
                }
                Vector3 vector5 = point + ((Vector3) (normalized * num));
                transform.position = new Vector3(vector5.x, transform.position.y, vector5.z);
            }
            return flag;
        }

        public static void ClearLine_GameView()
        {
            foreach (RenderLineInfo info in _renderLines)
            {
                info.Hide();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        public static void CreateExceptionWarningLogger()
        {
            GameObject target = new GameObject();
            target.AddComponent<MonoDeviceDebugGUI>();
            UnityEngine.Object.DontDestroyOnLoad(target);
        }

        public static float DistancForVec3IgnoreY(Vector3 Vec3A, Vector3 Vec3B)
        {
            Vec3A.y = 0f;
            Vec3B.y = 0f;
            return Vector3.Distance(Vec3A, Vec3B);
        }

        public static void DrawLine_GameView()
        {
            if (_bEnableDraw)
            {
                if (_renderLines == null)
                {
                    _renderLines = new List<RenderLineInfo>();
                }
                if (_renderLines.Count <= 0)
                {
                    RenderLineInfo item = new RenderLineInfo(Camera.main, new Vector3(0f, 0.5f, 10f), new Vector3(1f, 0.5f, 10f), Color.red, 0.02f);
                    RenderLineInfo info2 = new RenderLineInfo(Camera.main, new Vector3(0.5f, 1f, 10f), new Vector3(0.5f, 0f, 10f), Color.red, 0.02f);
                    _renderLines.Add(item);
                    _renderLines.Add(info2);
                }
                bool flag = false;
                foreach (RenderLineInfo info3 in _renderLines)
                {
                    if (info3.IsValid())
                    {
                        info3.Draw();
                    }
                    else
                    {
                        flag = true;
                    }
                }
                if (flag)
                {
                    _renderLines.Clear();
                }
            }
        }

        public static int ExtractOneNumFromString(string Str)
        {
            return int.Parse(Regex.Match(Str, @"\d+").Value);
        }

        public static Transform FindFirstChildGivenLayerAndCollider(Transform start, int layer)
        {
            Transform transform = null;
            IEnumerator enumerator = start.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if ((current.gameObject.layer == layer) && (current.gameObject.GetComponent<Collider>() != null))
                    {
                        return current;
                    }
                    transform = FindFirstChildGivenLayerAndCollider(current, layer);
                    if (transform != null)
                    {
                        return transform;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            return transform;
        }

        public static string FixNumberStringToLength(uint number, int length)
        {
            if (length <= 0)
            {
                throw new Exception("Invalid Type or State!");
            }
            string str = string.Empty + number;
            while (str.Length < length)
            {
                str = "0" + str;
            }
            return str;
        }

        public static string GetAnimIDAttackPropertyOutput(BaseActor actor, string animEventID)
        {
            if (actor != null)
            {
                if (actor is AvatarActor)
                {
                    return SharedAnimEventData.ResolveAnimEvent(((AvatarActor) actor).config, animEventID).AttackProperty.GetDebugOutput();
                }
                if (actor is MonsterActor)
                {
                    return SharedAnimEventData.ResolveAnimEvent(((MonsterActor) actor).config, animEventID).AttackProperty.GetDebugOutput();
                }
                if (actor is PropObjectActor)
                {
                    return SharedAnimEventData.ResolveAnimEvent(((PropObjectActor) actor).config, animEventID).AttackProperty.GetDebugOutput();
                }
            }
            return string.Format("<!null attack proeprty {0} on {1}>", GetDebugActorName(actor), animEventID);
        }

        public static string GetBaseName(string path)
        {
            int num = path.LastIndexOf('/');
            return ((num != -1) ? path.Substring(num + 1) : path);
        }

        public static string GetBeforeTimeToShow(DateTime time)
        {
            TimeSpan span = (TimeSpan) (TimeUtil.Now - time);
            if (span.TotalMinutes < 60.0)
            {
                object[] replaceParams = new object[] { span.Minutes };
                return LocalizationGeneralLogic.GetText("Menu_Desc_TimeMinutesBefore", replaceParams);
            }
            if (span.TotalHours < 24.0)
            {
                object[] objArray2 = new object[] { span.Hours };
                return LocalizationGeneralLogic.GetText("Menu_Desc_TimeHoursBefore", objArray2);
            }
            if (span.TotalDays < 7.0)
            {
                object[] objArray3 = new object[] { span.Days };
                return LocalizationGeneralLogic.GetText("Menu_Desc_TimeDaysBefore", objArray3);
            }
            return time.ToString("yyyy-MM-dd");
        }

        public static DateTime GetDateTimeFromTimeStamp(uint timeStamp)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return time.AddSeconds((double) timeStamp).ToLocalTime();
        }

        public static string GetDebugActorName(BaseActor actor)
        {
            if (actor == null)
            {
                return "<!null>";
            }
            if (actor.IsActive())
            {
                return string.Format("<{0}({1:x})>", Truncate(actor.gameObject.name, 10), actor.runtimeID);
            }
            if (actor.gameObject != null)
            {
                return string.Format("<!inactive {0}({1:x})>", Truncate(actor.gameObject.name, 10), actor.runtimeID);
            }
            return string.Format("<!dead:{0}>", actor.runtimeID);
        }

        public static string GetDebugEntityName(BaseMonoEntity entity)
        {
            if (entity == null)
            {
                return "<!null>";
            }
            return string.Format("<{0}({1:x})>", Truncate(entity.gameObject.name, 10), entity.GetRuntimeID());
        }

        public static Color GetDifficultyColor(LevelDiffculty difficulty)
        {
            switch (difficulty)
            {
                case LevelDiffculty.Normal:
                    return MiscData.GetColor("ChapterDifficultyNormal");

                case LevelDiffculty.Hard:
                    return MiscData.GetColor("ChapterDifficultyHard");

                case LevelDiffculty.Hell:
                    return MiscData.GetColor("ChapterDifficultyHell");
            }
            return MiscData.GetColor("ChapterDifficultyNormal");
        }

        public static string GetDifficultyDesc(LevelDiffculty difficulty)
        {
            switch (difficulty)
            {
                case LevelDiffculty.Normal:
                    return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyNormal", new object[0]);

                case LevelDiffculty.Hard:
                    return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyHard", new object[0]);

                case LevelDiffculty.Hell:
                    return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyHell", new object[0]);
            }
            return LocalizationGeneralLogic.GetText("Menu_Desc_DifficultyNormal", new object[0]);
        }

        public static int GetDiffTimeToShow(DateTime from, DateTime to, out string label)
        {
            TimeSpan span = (TimeSpan) (to - from);
            if (span.TotalMinutes < 60.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Minute", new object[0]);
                return span.Minutes;
            }
            if (span.TotalHours < 24.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Hour", new object[0]);
                return span.Hours;
            }
            if (span.TotalDays < 7.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Day", new object[0]);
                return span.Days;
            }
            label = string.Empty;
            return 0;
        }

        public static Sprite GetItemSpriteByPrefab(int id)
        {
            return GetSpriteByPrefab(Singleton<StorageModule>.Instance.GetDummyStorageDataItem(id, 1).GetIconPath());
        }

        public static Sprite GetSpriteByPrefab(string prefabPath)
        {
            Sprite sprite = LoadResource<Sprite>(prefabPath, BundleType.RESOURCE_FILE);
            if (sprite != null)
            {
                return sprite;
            }
            GameObject obj2 = LoadResource<GameObject>(prefabPath, BundleType.RESOURCE_FILE);
            if (obj2 == null)
            {
                obj2 = LoadResource<GameObject>("SpriteOutput/SpecialIcons/ItemEmpty", BundleType.RESOURCE_FILE);
            }
            return obj2.GetComponent<SpriteRenderer>().sprite;
        }

        public static int GetTimeSpanToShow(DateTime time, out string label)
        {
            TimeSpan span;
            DateTime now = TimeUtil.Now;
            if (now > time)
            {
                span = (TimeSpan) (now - time);
            }
            else
            {
                span = (TimeSpan) (time - now);
            }
            if (span.TotalMinutes < 60.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Minute", new object[0]);
                return span.Minutes;
            }
            if (span.TotalHours < 24.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Hour", new object[0]);
                return span.Hours;
            }
            if (span.TotalDays < 7.0)
            {
                label = LocalizationGeneralLogic.GetText("Menu_Desc_Day", new object[0]);
                return span.Days;
            }
            label = string.Empty;
            return 0;
        }

        public static uint GetTimeStampFromDateTime(DateTime datetime)
        {
            DateTime time = new DateTime(0x7b2, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan span = (TimeSpan) (datetime - time.ToLocalTime());
            return (uint) span.TotalSeconds;
        }

        public static string GetTimeString(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm");
        }

        public static bool IsAlmostZero(float f)
        {
            return ((f < 0.0001f) && (f > -0.0001f));
        }

        public static bool IsFloatInRange(float value, float lower, float upper)
        {
            return ((value > lower) && (value < upper));
        }

        public static bool IsPosition2InRect(Vector2 position, Rect rect)
        {
            return ((((position.x >= rect.xMin) && (position.x <= rect.xMax)) && (position.y >= rect.yMin)) && (position.y <= rect.yMax));
        }

        public static Vector3 LerpAngleForVec3IgnoreY(Vector3 DirFrom, Vector3 DirTo, float ratio)
        {
            Vector3 vector = new Vector3(DirFrom.x, 0f, DirFrom.z);
            Vector3 vector2 = new Vector3(DirTo.x, 0f, DirTo.z);
            vector.Normalize();
            vector2.Normalize();
            float num = Mathf.Atan2(DirFrom.z, DirFrom.x);
            float num2 = Mathf.Atan2(DirTo.z, DirTo.x);
            float f = (ratio * num2) + ((1f - ratio) * num);
            return new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
        }

        public static string ListToString(List<int> list)
        {
            StringBuilder builder = new StringBuilder();
            foreach (int num in list)
            {
                builder.Append(num.ToString());
                builder.Append(", ");
            }
            return builder.ToString();
        }

        [DebuggerHidden]
        public static IEnumerator LoadAsyncAsset(string path, Action<object> callback)
        {
            return new <LoadAsyncAsset>c__Iterator7B { path = path, callback = callback, <$>path = path, <$>callback = callback };
        }

        public static UnityEngine.Object LoadResource(string path, BundleType type = 1)
        {
            return LoadResource<UnityEngine.Object>(path, type);
        }

        public static T LoadResource<T>(string path, BundleType type = 1) where T: UnityEngine.Object
        {
            if (Singleton<AssetBundleManager>.Instance == null)
            {
                return Resources.Load<T>(path);
            }
            BundleType type2 = type;
            if (type2 != BundleType.DATA_FILE)
            {
                if (type2 == BundleType.RESOURCE_FILE)
                {
                    return Singleton<AssetBundleManager>.Instance.LoadRes<T>(path);
                }
                return null;
            }
            return Singleton<AssetBundleManager>.Instance.LoadData<T>(path);
        }

        public static AsyncAssetRequst LoadResourceAsync(string path, BundleType type = 1)
        {
            if (Singleton<AssetBundleManager>.Instance == null)
            {
                return new AsyncAssetRequst(Resources.LoadAsync(path));
            }
            BundleType type2 = type;
            if (type2 != BundleType.DATA_FILE)
            {
                if (type2 == BundleType.RESOURCE_FILE)
                {
                    return Singleton<AssetBundleManager>.Instance.LoadResAsync(path);
                }
                return null;
            }
            return Singleton<AssetBundleManager>.Instance.LoadDataAsync(path);
        }

        public static string LoadTextFileToString(string filePath)
        {
            TextAsset assetToUnload = LoadResource(filePath, BundleType.DATA_FILE) as TextAsset;
            string text = assetToUnload.text;
            Resources.UnloadAsset(assetToUnload);
            return text;
        }

        public static float NormalizedClamp(float value, float min, float max)
        {
            value = Mathf.Clamp(value, min, max);
            return ((value - min) / (max - min));
        }

        public static float NormalizedRotateAngle(float from, float to)
        {
            float num = SignedAngleDiff(from, to);
            return (from + num);
        }

        public static Color ParseColor(string hexString)
        {
            Color white = Color.white;
            if (!ColorUtility.TryParseHtmlString(hexString, out white))
            {
            }
            return white;
        }

        public static void ParseJsonParameterGroup(JSONNode aNode, Hashtable dynamicParamTable)
        {
            ParseJsonParameterGroupWithParamPrefix(aNode, dynamicParamTable, string.Empty);
        }

        public static void ParseJsonParameterGroupWithParamPrefix(JSONNode aNode, Hashtable dynamicParamTable, string paramPrefix)
        {
            for (int i = 0; i < aNode["Parameters"].Count; i++)
            {
                dynamicParamTable.Add(paramPrefix + aNode["Parameters"][i]["Param"].Value, ParseJsonParameterValue(aNode["Parameters"][i]));
            }
        }

        public static object ParseJsonParameterValue(JSONNode aNode)
        {
            string key = (string) aNode["Type"];
            if (key != null)
            {
                Dictionary<string, int> dictionary;
                int num6;
                if (<>f__switch$map7 == null)
                {
                    dictionary = new Dictionary<string, int>(6);
                    dictionary.Add("Float", 0);
                    dictionary.Add("Int", 1);
                    dictionary.Add("Double", 2);
                    dictionary.Add("Bool", 3);
                    dictionary.Add("String", 4);
                    dictionary.Add("Array", 5);
                    <>f__switch$map7 = dictionary;
                }
                if (<>f__switch$map7.TryGetValue(key, out num6))
                {
                    switch (num6)
                    {
                        case 0:
                            return aNode["Value"].AsFloat;

                        case 1:
                            return aNode["Value"].AsInt;

                        case 2:
                            return aNode["Value"].AsDouble;

                        case 3:
                            return aNode["Value"].AsBool;

                        case 4:
                            return aNode["Value"].Value;

                        case 5:
                        {
                            JSONArray asArray = aNode["Value"].AsArray;
                            string str2 = (string) aNode["ArrayElemType"];
                            if (str2 != null)
                            {
                                int num7;
                                if (<>f__switch$map6 == null)
                                {
                                    dictionary = new Dictionary<string, int>(5);
                                    dictionary.Add("Float", 0);
                                    dictionary.Add("Int", 1);
                                    dictionary.Add("Double", 2);
                                    dictionary.Add("Bool", 3);
                                    dictionary.Add("String", 4);
                                    <>f__switch$map6 = dictionary;
                                }
                                if (<>f__switch$map6.TryGetValue(str2, out num7))
                                {
                                    switch (num7)
                                    {
                                        case 0:
                                        {
                                            float[] numArray = new float[asArray.Count];
                                            for (int i = 0; i < asArray.Count; i++)
                                            {
                                                numArray[i] = asArray[i].AsFloat;
                                            }
                                            return numArray;
                                        }
                                        case 1:
                                        {
                                            int[] numArray2 = new int[asArray.Count];
                                            for (int j = 0; j < asArray.Count; j++)
                                            {
                                                numArray2[j] = asArray[j].AsInt;
                                            }
                                            return numArray2;
                                        }
                                        case 2:
                                        {
                                            double[] numArray3 = new double[asArray.Count];
                                            for (int k = 0; k < asArray.Count; k++)
                                            {
                                                numArray3[k] = asArray[k].AsDouble;
                                            }
                                            return numArray3;
                                        }
                                        case 3:
                                        {
                                            bool[] flagArray = new bool[asArray.Count];
                                            for (int m = 0; m < asArray.Count; m++)
                                            {
                                                flagArray[m] = asArray[m].AsBool;
                                            }
                                            return flagArray;
                                        }
                                        case 4:
                                        {
                                            string[] strArray = new string[asArray.Count];
                                            for (int n = 0; n < asArray.Count; n++)
                                            {
                                                strArray[n] = asArray[n].Value;
                                            }
                                            return strArray;
                                        }
                                    }
                                }
                            }
                            throw new Exception("Invalid Type or State!");
                        }
                    }
                }
            }
            throw new Exception("Invalid Type or State!");
        }

        public static void PrettyPrintLayerMask(LayerMask mask)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(mask.value.ToString() + "\n");
            for (int i = 0; i < 0x20; i++)
            {
                if (mask.ContainsLayer(i))
                {
                    builder.Append(LayerMask.LayerToName(i) + "\n");
                }
            }
        }

        public static int RightMost1BitIndex(int x)
        {
            int num = -1;
            while (x != 0)
            {
                num++;
                x = x >> 1;
            }
            return num;
        }

        public static void SetPitch(Transform tran, float pitch)
        {
            Vector3 eulerAngles = tran.eulerAngles;
            eulerAngles.x = pitch;
            tran.eulerAngles = eulerAngles;
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int count = list.Count;
            while (count > 1)
            {
                count--;
                int num2 = UnityEngine.Random.Range(0, count);
                T local = list[num2];
                list[num2] = list[count];
                list[count] = local;
            }
        }

        public static float SignedAngleDiff(float from, float to)
        {
            float num = to - from;
            while (num > 180f)
            {
                num -= 360f;
            }
            while (num < -180f)
            {
                num += 360f;
            }
            return num;
        }

        public static void TriggerDrawLine()
        {
            _bEnableDraw = !_bEnableDraw;
            if (!_bEnableDraw)
            {
                ClearLine_GameView();
            }
        }

        public static string Truncate(string str, int n = 10)
        {
            return ((str.Length <= n) ? str : str.Substring(0, n));
        }

        public static bool WildcardMatch(string wildcard, string input, bool ignoreCase = false)
        {
            int indexA = 0;
            int num2 = 0;
            int indexB = 0;
            int num4 = 0;
            while (((indexA < input.Length) && (indexB < wildcard.Length)) && (wildcard[indexB] != '*'))
            {
                if ((wildcard[indexB] != '?') && (string.Compare(input, indexA, wildcard, indexB, 1, ignoreCase) != 0))
                {
                    return false;
                }
                indexA++;
                indexB++;
            }
            while (indexA < input.Length)
            {
                if ((indexB < wildcard.Length) && (wildcard[indexB] == '*'))
                {
                    if (++indexB >= wildcard.Length)
                    {
                        return true;
                    }
                    num2 = indexA + 1;
                    num4 = indexB;
                }
                else
                {
                    if ((indexB < wildcard.Length) && ((string.Compare(input, indexA, wildcard, indexB, 1, ignoreCase) == 0) || (wildcard[indexB] == '?')))
                    {
                        indexA++;
                        indexB++;
                        continue;
                    }
                    indexA = num2++;
                    indexB = num4;
                }
            }
            while ((indexB < wildcard.Length) && (wildcard[indexB] == '*'))
            {
                indexB++;
            }
            return (indexB >= wildcard.Length);
        }

        [DebuggerHidden]
        public static IEnumerator WWWRequestWithRetry(string url, Action<string> callback, Action timeOutCallback, float timeoutSecond = 5f, int retryTime = 3, byte[] postData = null, Dictionary<string, string> headers = null)
        {
            return new <WWWRequestWithRetry>c__Iterator7D { retryTime = retryTime, url = url, postData = postData, headers = headers, timeoutSecond = timeoutSecond, timeOutCallback = timeOutCallback, callback = callback, <$>retryTime = retryTime, <$>url = url, <$>postData = postData, <$>headers = headers, <$>timeoutSecond = timeoutSecond, <$>timeOutCallback = timeOutCallback, <$>callback = callback };
        }

        [DebuggerHidden]
        public static IEnumerator WWWRequestWithTimeOut(string url, Action<string> callback, Action timeOutCallback, float timeoutSecond = 5f, byte[] postData = null, Dictionary<string, string> headers = null, bool needDispose = true)
        {
            return new <WWWRequestWithTimeOut>c__Iterator7C { url = url, postData = postData, headers = headers, timeoutSecond = timeoutSecond, timeOutCallback = timeOutCallback, callback = callback, needDispose = needDispose, <$>url = url, <$>postData = postData, <$>headers = headers, <$>timeoutSecond = timeoutSecond, <$>timeOutCallback = timeOutCallback, <$>callback = callback, <$>needDispose = needDispose };
        }

        [CompilerGenerated]
        private sealed class <LoadAsyncAsset>c__Iterator7B : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<object> <$>callback;
            internal string <$>path;
            internal ResourceRequest <resReq>__0;
            internal Action<object> callback;
            internal string path;

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
                        this.<resReq>__0 = Resources.LoadAsync(this.path);
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0087;
                }
                if (!this.<resReq>__0.isDone)
                {
                    this.$current = 0;
                    this.$PC = 1;
                    return true;
                }
                if (this.callback != null)
                {
                    this.callback(this.<resReq>__0.asset);
                }
                this.$PC = -1;
            Label_0087:
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

        [CompilerGenerated]
        private sealed class <WWWRequestWithRetry>c__Iterator7D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>callback;
            internal Dictionary<string, string> <$>headers;
            internal byte[] <$>postData;
            internal int <$>retryTime;
            internal Action <$>timeOutCallback;
            internal float <$>timeoutSecond;
            internal string <$>url;
            internal int <counter>__0;
            internal WWW <localWWW>__3;
            internal bool <timeout>__2;
            internal float <timer>__1;
            internal string <warningMsg>__4;
            internal Action<string> callback;
            internal Dictionary<string, string> headers;
            internal byte[] postData;
            internal int retryTime;
            internal Action timeOutCallback;
            internal float timeoutSecond;
            internal string url;

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
                        this.<counter>__0 = 0;
                        goto Label_015E;

                    case 1:
                        break;

                    default:
                        goto Label_0176;
                }
            Label_00B1:
                while (!this.<localWWW>__3.isDone)
                {
                    if (this.<timer>__1 > this.timeoutSecond)
                    {
                        this.<timeout>__2 = true;
                        break;
                    }
                    this.<timer>__1 += Time.deltaTime;
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                if (!string.IsNullOrEmpty(this.<localWWW>__3.error) || this.<timeout>__2)
                {
                    this.<warningMsg>__4 = !this.<timeout>__2 ? this.<localWWW>__3.error : "timeout";
                    if ((this.<counter>__0 >= this.retryTime) && (this.timeOutCallback != null))
                    {
                        this.timeOutCallback();
                    }
                }
                else
                {
                    if (this.callback != null)
                    {
                        this.callback(this.<localWWW>__3.text);
                    }
                    goto Label_016F;
                }
            Label_015E:
                if (this.<counter>__0 < this.retryTime)
                {
                    this.<counter>__0++;
                    this.<timer>__1 = 0f;
                    this.<timeout>__2 = false;
                    this.<localWWW>__3 = new WWW(this.url, this.postData, this.headers);
                    goto Label_00B1;
                }
            Label_016F:
                this.$PC = -1;
            Label_0176:
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

        [CompilerGenerated]
        private sealed class <WWWRequestWithTimeOut>c__Iterator7C : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal Action<string> <$>callback;
            internal Dictionary<string, string> <$>headers;
            internal bool <$>needDispose;
            internal byte[] <$>postData;
            internal Action <$>timeOutCallback;
            internal float <$>timeoutSecond;
            internal string <$>url;
            internal bool <timeout>__1;
            internal float <timer>__0;
            internal WWW <www>__2;
            internal Action<string> callback;
            internal Dictionary<string, string> headers;
            internal bool needDispose;
            internal byte[] postData;
            internal Action timeOutCallback;
            internal float timeoutSecond;
            internal string url;

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
                        this.<timer>__0 = 0f;
                        this.<timeout>__1 = false;
                        this.<www>__2 = new WWW(this.url, this.postData, this.headers);
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_0125;
                }
                if (!this.<www>__2.isDone)
                {
                    if (this.<timer>__0 <= this.timeoutSecond)
                    {
                        this.<timer>__0 += Time.deltaTime;
                        this.$current = null;
                        this.$PC = 1;
                        return true;
                    }
                    this.<timeout>__1 = true;
                }
                if (string.IsNullOrEmpty(this.<www>__2.error))
                {
                    if (this.<timeout>__1)
                    {
                        if (this.timeOutCallback != null)
                        {
                            this.timeOutCallback();
                        }
                    }
                    else if (this.callback != null)
                    {
                        this.callback(this.<www>__2.text);
                    }
                }
                if (this.needDispose)
                {
                    this.<www>__2.Dispose();
                }
                this.$PC = -1;
            Label_0125:
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

        public class RenderLineInfo
        {
            private Camera _camera;
            private GameObject _lineObj;
            private LineRenderer _lr;
            private Vector3 _screenPos_end;
            private Vector3 _screenPos_start;
            private static string _shaderName = "miHoYo/UI/Image Tint Color";

            public RenderLineInfo(Camera camera, Vector3 start, Vector3 end, Color color, float width)
            {
                this._camera = camera;
                this._screenPos_start = start;
                this._screenPos_end = end;
                this._lineObj = new GameObject();
                this._lineObj.AddComponent<LineRenderer>();
                this._lr = this._lineObj.GetComponent<LineRenderer>();
                this._lr.material = new Material(Shader.Find(_shaderName));
                this._lr.SetColors(color, color);
                this._lr.SetWidth(width, width);
            }

            public void Draw()
            {
                this._lineObj.SetActive(true);
                Vector3 position = this._screenPos_start;
                position.x = this._camera.pixelWidth * position.x;
                position.y = this._camera.pixelHeight * position.y;
                Vector3 vector2 = this._screenPos_end;
                vector2.x = this._camera.pixelWidth * vector2.x;
                vector2.y = this._camera.pixelHeight * vector2.y;
                Vector3 vector3 = this._camera.ScreenToWorldPoint(position);
                Vector3 vector4 = this._camera.ScreenToWorldPoint(vector2);
                this._lineObj.transform.position = vector3;
                this._lr.SetPosition(0, vector3);
                this._lr.SetPosition(1, vector4);
            }

            public void Hide()
            {
                this._lineObj.SetActive(false);
            }

            public bool IsValid()
            {
                return (this._lineObj != null);
            }
        }
    }
}

