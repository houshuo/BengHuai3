namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public static class DesignDataTemp
    {
        public const string LEVEL_LUA_COMMON = "LuaTemp/Common.lua";
        public static Dictionary<string, string[]> LEVEL_LUA_ENTRY_FILE_NAMES;
        public static string LEVEL_LUA_ENTRY_PATH = "Lua/Levels/";

        static DesignDataTemp()
        {
            Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
            string[] textArray1 = new string[] { 
                "Level0.lua", "Level Keith.lua", "Level Test SlowMo.lua", "Level Test SlowMoInsane.lua", "Level infinity.lua", "Level Print Json.lua", "Level Analyse.lua", "Level Monster Analyse.lua", "Level Analyse Auto.lua", "Level PerfBenchmark.lua", "LevelDesignInfoCompute.lua", "TestLevel.lua", "Level Avatar Test lv15.lua", "Level Avatar Test lv40.lua", "Level Avatar Test lv80.lua", "Level Avatar Test lvAll.lua", 
                "Level Avatar Test.lua", "Level Present.lua", "Level Ladder.lua", "Level Scene Exhibition.lua", "Level Boss Titan.lua"
             };
            dictionary.Add("Common", textArray1);
            string[] textArray2 = new string[] { 
                "Level 1-1.lua", "Level 1-2.lua", "Level 1-3.lua", "Level 1-4.lua", "Level 1-5.lua", "Level 1-6.lua", "Level 1-7.lua", "Level 1-8.lua", "Level 1-9.lua", "Level 1-10.lua", "Level 1-11.lua", "Level 1-12.lua", "Level 1-13.lua", "Level 1-14.lua", "Level 1-15.lua", "Level 1-16.lua", 
                "Level 1-17.lua", "Level 1-18.lua", "Level 1-19.lua", "Level 1-20.lua", "Level 1-21.lua"
             };
            dictionary.Add("Stage01", textArray2);
            string[] textArray3 = new string[] { 
                "Level 2-1.lua", "Level 2-2.lua", "Level 2-3.lua", "Level 2-4.lua", "Level 2-5.lua", "Level 2-6.lua", "Level 2-7.lua", "Level 2-8.lua", "Level 2-9.lua", "Level 2-10.lua", "Level 2-11.lua", "Level 2-12.lua", "Level 2-13.lua", "Level 2-14.lua", "Level 2-15.lua", "Level 2-16.lua", 
                "Level 2-17.lua", "Level 2-18.lua", "Level 2-19.lua", "Level 2-20.lua", "Level 2-21.lua", "Level 2-22.lua", "Level 2-23.lua", "Level 2-24.lua", "Level 2-25.lua", "Level 2-26.lua", "Level 2-27.lua"
             };
            dictionary.Add("Stage02", textArray3);
            string[] textArray4 = new string[] { "Level 3-1.lua", "Level 3-2.lua", "Level 3-3.lua", "Level 3-4.lua", "Level 3-5.lua", "Level 3-6.lua", "Level 3-7.lua", "Level 3-8.lua", "Level 3-9.lua", "Level 3-10.lua", "Level 3-11.lua", "Level 3-12.lua", "Level 3-13.lua", "Level 3-14.lua", "Level 3-15.lua", "Level 3-16.lua" };
            dictionary.Add("Stage03", textArray4);
            string[] textArray5 = new string[] { 
                "Level 4-1.lua", "Level 4-2.lua", "Level 4-3.lua", "Level 4-4.lua", "Level 4-5.lua", "Level 4-6.lua", "Level 4-7.lua", "Level 4-8.lua", "Level 4-9.lua", "Level 4-10.lua", "Level 4-11.lua", "Level 4-12.lua", "Level 4-13.lua", "Level 4-14.lua", "Level 4-15.lua", "Level 4-16.lua", 
                "Level 4-17.lua", "Level 4-18.lua", "Level 4-19.lua", "Level 4-20.lua", "Level 4-21.lua", "Level 4-22.lua", "Level 4-23.lua", "Level 4-24.lua", "Level 4-25.lua", "Level 4-26.lua", "Level 4-27.lua", "Level 4-28.lua"
             };
            dictionary.Add("Stage04", textArray5);
            string[] textArray6 = new string[] { 
                "ScoinLevel.lua", "WeaponExpItemLevel.lua", "AvatarExpItemLevel.lua", "StigmataExpItemLevel.lua", "SkillExpItemLevel.lua", "Weekday01.lua", "Weekday02.lua", "Weekday03.lua", "Weekday04.lua", "Weekday05.lua", "Weekday06.lua", "Weekday07.lua", "BossLevel01_H.lua", "BossLevel02_H.lua", "Activity_01.lua", "Activity_02.lua", 
                "Activity_03.lua", "Activity_04.lua", "Activity_05.lua", "Activity_06.lua", "Activity_07.lua", "Activity_08.lua", "LevelInfinite.lua"
             };
            dictionary.Add("ActivityLevel", textArray6);
            LEVEL_LUA_ENTRY_FILE_NAMES = dictionary;
        }
    }
}

