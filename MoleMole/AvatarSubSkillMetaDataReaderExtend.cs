namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class AvatarSubSkillMetaDataReaderExtend
    {
        private static Dictionary<int, List<int>> _skillMap;

        public static List<int> GetAvatarSubSkillIdList(int skillId)
        {
            if (!_skillMap.ContainsKey(skillId))
            {
                return new List<int>();
            }
            return _skillMap[skillId];
        }

        public static void LoadFromFileAndBuildMap()
        {
            AvatarSubSkillMetaDataReader.LoadFromFile();
            List<AvatarSubSkillMetaData> itemList = AvatarSubSkillMetaDataReader.GetItemList();
            _skillMap = new Dictionary<int, List<int>>();
            foreach (AvatarSubSkillMetaData data in itemList)
            {
                if (!_skillMap.ContainsKey(data.skillId))
                {
                    _skillMap.Add(data.skillId, new List<int>());
                }
                _skillMap[data.skillId].Add(data.avatarSubSkillId);
            }
        }
    }
}

