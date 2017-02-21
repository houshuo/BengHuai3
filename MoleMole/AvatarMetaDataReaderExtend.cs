namespace MoleMole
{
    using System;
    using System.Collections.Generic;

    public class AvatarMetaDataReaderExtend
    {
        private static Dictionary<int, AvatarIDs> _avatarIDMap;

        public static AvatarIDs GetAvatarIDsByKey(int id)
        {
            AvatarIDs ds;
            _avatarIDMap.TryGetValue(id, out ds);
            return ds;
        }

        public static void LoadFromFileAndBuildMap()
        {
            AvatarMetaDataReader.LoadFromFile();
            _avatarIDMap = new Dictionary<int, AvatarIDs>();
            foreach (AvatarMetaData data in AvatarMetaDataReader.GetItemList())
            {
                AvatarIDs ds = new AvatarIDs {
                    avatarID = data.avatarID,
                    avatarCardID = data.avatarCardID,
                    avatarFragmentID = data.avatarFragmentID
                };
                _avatarIDMap.Add(data.avatarID, ds);
                _avatarIDMap.Add(data.avatarCardID, ds);
                _avatarIDMap.Add(data.avatarFragmentID, ds);
            }
        }
    }
}

