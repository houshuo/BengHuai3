namespace MoleMole
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class MonoDevLevelBenchmarkDeploy : MonoBehaviour
    {
        private GUIContent[] _aiTypes;
        private GUIContent[] _avatarTypes;
        private GUIContent[] _levels;
        private GUIContent[] _monsterNames;
        private GUIContent[][] _monsterTypes;
        private bool _multiMode;
        private GUIContent[] _stages;
        private GUIStyle _style;
        private Popup _widgetAIType = new Popup();
        private Popup _widgetAvatarType = new Popup();
        private Popup _widgetLevels = new Popup();
        private Popup _widgetMonsterNames = new Popup();
        private Popup _widgetMonsterTypes = new Popup();
        private Popup _widgetStages = new Popup();
        [CompilerGenerated]
        private static Func<MonsterConfigMetaData, string> <>f__am$cache10;
        [CompilerGenerated]
        private static Func<IGrouping<string, MonsterConfigMetaData>, MonsterConfigMetaData> <>f__am$cache11;
        [CompilerGenerated]
        private static Func<MonsterConfigMetaData, GUIContent> <>f__am$cache12;
        [CompilerGenerated]
        private static Func<MonsterConfigMetaData, GUIContent> <>f__am$cache13;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cache14;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cache15;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cacheE;
        [CompilerGenerated]
        private static Func<string, GUIContent> <>f__am$cacheF;
        private const string AI_PREFIX = "AI/Avatar/";

        private void Awake()
        {
            this._style = new GUIStyle();
            this._style.normal.textColor = Color.white;
            this._style.hover.textColor = Color.white;
            this._style.focused.textColor = Color.gray;
            this._style.active.textColor = Color.white;
            this._style.onNormal.textColor = Color.white;
            this._style.onHover.textColor = Color.white;
            this._style.onFocused.textColor = Color.gray;
            this._style.onActive.textColor = Color.white;
        }

        private void LoadDevLevel()
        {
            DevLevelConfigData.LEVEL_MODE = !this._multiMode ? LevelActor.Mode.Single : LevelActor.Mode.Multi;
            DevLevelConfigData.configFromScene = true;
            DevLevelConfigData.isBenchmark = true;
            SceneManager.LoadScene("DevLevel");
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(50f, 0f, (float) (Screen.width - 100), (float) Screen.height));
            GUILayout.Space(20f);
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Avatar : ", options);
            this._widgetAvatarType.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._avatarTypes, this._style, this._style, 1);
            GUILayoutOption[] optionArray2 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("AI : ", optionArray2);
            this._widgetAIType.List(GUILayoutUtility.GetRect((float) 50f, (float) (this._style.lineHeight * 1.5f)), this._aiTypes, this._style, this._style, 1);
            GUILayoutOption[] optionArray3 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(100f) };
            if (GUILayout.Button("Add", optionArray3) && (DevLevelConfigData.avatarDevDatas.Count < 3))
            {
                string text = this._avatarTypes[this._widgetAvatarType.GetSelectedItemIndex()].text;
                DevAvatarData item = new DevAvatarData {
                    avatarType = text
                };
                item.avatarTestSkills = new string[] { "Test_UnlockAllAniSkill", "Test_Undamagable" };
                item.avatarAI = "AI/Avatar/" + this._aiTypes[this._widgetAIType.GetSelectedItemIndex()].text;
                item.avatarWeapon = WeaponData.GetFirstWeaponIDForRole(AvatarData.GetAvatarConfig(text).CommonArguments.RoleName);
                item.avatarLevel = 1;
                item.avatarWeaponLevel = 1;
                item.avatarStigmata = new int[] { -1, -1, -1 };
                DevLevelConfigData.avatarDevDatas.Add(item);
            }
            GUILayout.EndHorizontal();
            for (int i = 0; i < DevLevelConfigData.avatarDevDatas.Count; i++)
            {
                DevAvatarData data = DevLevelConfigData.avatarDevDatas[i];
                GUILayout.Label(string.Format("{0}: {1}", data.avatarType, data.avatarAI), new GUILayoutOption[0]);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayoutOption[] optionArray4 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Monster: ", optionArray4);
            int index = this._widgetMonsterNames.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._monsterNames, this._style, this._style, 1);
            GUILayoutOption[] optionArray5 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Type: ", optionArray5);
            this._widgetMonsterTypes.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._monsterTypes[index], this._style, this._style, 1);
            GUILayoutOption[] optionArray6 = new GUILayoutOption[] { GUILayout.ExpandWidth(false), GUILayout.Width(100f) };
            if (GUILayout.Button("Add", optionArray6))
            {
                DevMonsterData data4 = new DevMonsterData {
                    monsterName = this._monsterNames[this._widgetMonsterNames.GetSelectedItemIndex()].text,
                    typeName = this._monsterTypes[this._widgetMonsterNames.GetSelectedItemIndex()][this._widgetMonsterTypes.GetSelectedItemIndex()].text
                };
                data4.abilities = new string[] { "Test_Undamagable" };
                data4.level = 1;
                DevLevelConfigData.monsterDevDatas.Add(data4);
            }
            GUILayout.EndHorizontal();
            for (int j = 0; j < DevLevelConfigData.monsterDevDatas.Count; j++)
            {
                DevMonsterData data2 = DevLevelConfigData.monsterDevDatas[j];
                GUILayout.Label(string.Format("{0}, {1}", data2.monsterName, data2.typeName), new GUILayoutOption[0]);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            DevLevelConfigData.stageDevData.stageName = this._stages[this._widgetStages.GetSelectedItemIndex()].text;
            GUILayoutOption[] optionArray7 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            GUILayout.Label("Level: ", optionArray7);
            this._widgetLevels.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._levels, this._style, this._style, 1);
            DevLevelConfigData.LEVEL_PATH = "Lua/Levels/" + this._levels[this._widgetLevels.GetSelectedItemIndex()].text;
            GUILayoutOption[] optionArray8 = new GUILayoutOption[] { GUILayout.ExpandWidth(false) };
            this._multiMode = GUILayout.Toggle(this._multiMode, "Multi Mode?", optionArray8);
            GUILayout.EndHorizontal();
            this._widgetStages.List(GUILayoutUtility.GetRect((float) 20f, (float) (this._style.lineHeight * 1.5f)), this._stages, this._style, this._style, 3);
            GUILayout.Space(50f);
            GUILayoutOption[] optionArray9 = new GUILayoutOption[] { GUILayout.Height(this._style.lineHeight * 3f) };
            if (GUILayout.Button("Start", optionArray9))
            {
                this.LoadDevLevel();
            }
            GUILayoutOption[] optionArray10 = new GUILayoutOption[] { GUILayout.Height(this._style.lineHeight * 3f) };
            if (GUILayout.Button("Render Scene", optionArray10))
            {
                SceneManager.LoadScene("RenderScene");
            }
            GUILayout.EndArea();
        }

        private void Start()
        {
            MainUIData.USE_VIEW_CACHING = false;
            GeneralLogicManager.InitAll();
            GlobalDataManager.Refresh();
            if (<>f__am$cacheE == null)
            {
                <>f__am$cacheE = x => new GUIContent(x);
            }
            this._avatarTypes = AvatarData.GetAllAvatarData().Keys.Select<string, GUIContent>(<>f__am$cacheE).ToArray<GUIContent>();
            string[] source = new string[] { "AvatarAutoBattleBehavior_Attack", "AvatarAutoBattleBehavior_AlwaysSkill", "AvatarAutoBattleBehavior_BranchAttack_Kiana_ATK02", "AvatarAutoBattleBehavior_BranchAttack_Kiana_ATK03", "AvatarAutoBattleBehavior_BranchAttack_Mei_ATK01", "AvatarAutoBattleBehavior_Empty" };
            if (<>f__am$cacheF == null)
            {
                <>f__am$cacheF = x => new GUIContent(x);
            }
            this._aiTypes = source.Select<string, GUIContent>(<>f__am$cacheF).ToArray<GUIContent>();
            if (<>f__am$cache10 == null)
            {
                <>f__am$cache10 = x => x.monsterName;
            }
            if (<>f__am$cache11 == null)
            {
                <>f__am$cache11 = g => Enumerable.First<MonsterConfigMetaData>(g);
            }
            if (<>f__am$cache12 == null)
            {
                <>f__am$cache12 = x => new GUIContent(x.monsterName);
            }
            this._monsterNames = Enumerable.GroupBy<MonsterConfigMetaData, string>(MonsterData.GetAllMonsterConfigMetaData(), <>f__am$cache10).Select<IGrouping<string, MonsterConfigMetaData>, MonsterConfigMetaData>(<>f__am$cache11).Select<MonsterConfigMetaData, GUIContent>(<>f__am$cache12).ToArray<GUIContent>();
            this._monsterTypes = new GUIContent[this._monsterNames.Length][];
            <Start>c__AnonStoreyBD ybd = new <Start>c__AnonStoreyBD {
                <>f__this = this,
                ix = 0
            };
            while (ybd.ix < this._monsterTypes.Length)
            {
                if (<>f__am$cache13 == null)
                {
                    <>f__am$cache13 = x => new GUIContent(x.typeName);
                }
                this._monsterTypes[ybd.ix] = MonsterData.GetAllMonsterConfigMetaData().Where<MonsterConfigMetaData>(new Func<MonsterConfigMetaData, bool>(ybd.<>m__A2)).Select<MonsterConfigMetaData, GUIContent>(<>f__am$cache13).ToArray<GUIContent>();
                ybd.ix++;
            }
            if (<>f__am$cache14 == null)
            {
                <>f__am$cache14 = x => new GUIContent(x);
            }
            this._stages = StageData.GetAllStageEntries().Keys.Select<string, GUIContent>(<>f__am$cache14).ToArray<GUIContent>();
            string[] textArray2 = new string[] { "Common/Level 0.lua", "Common/Level Keith.lua", "Common/Level Benchmark Baseline.lua" };
            if (<>f__am$cache15 == null)
            {
                <>f__am$cache15 = x => new GUIContent(x);
            }
            this._levels = textArray2.Select<string, GUIContent>(<>f__am$cache15).ToArray<GUIContent>();
            DevLevelConfigData.avatarDevDatas.Clear();
            DevLevelConfigData.monsterDevDatas.Clear();
            DevLevelConfigData.stageDevData = new DevStageData();
        }

        private void Update()
        {
        }

        [CompilerGenerated]
        private sealed class <Start>c__AnonStoreyBD
        {
            internal MonoDevLevelBenchmarkDeploy <>f__this;
            internal int ix;

            internal bool <>m__A2(MonsterConfigMetaData x)
            {
                return (x.monsterName == this.<>f__this._monsterNames[this.ix].text);
            }
        }
    }
}

