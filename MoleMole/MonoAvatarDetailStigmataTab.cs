namespace MoleMole
{
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using UnityEngine;

    public class MonoAvatarDetailStigmataTab : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private bool _isRemoteAvatar;
        private FriendDetailDataItem _userData;

        private List<string> GenerateEffectDesc(Dictionary<int, EquipSkillDataItem> setSkills)
        {
            List<string> list = new List<string>();
            OrderedDictionary dictionary = new OrderedDictionary();
            foreach (KeyValuePair<int, EquipSkillDataItem> pair in setSkills)
            {
                dictionary.Add(pair.Key, pair.Value);
            }
            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DictionaryEntry current = (DictionaryEntry) enumerator.Current;
                    list.Add("<color=#FEDF4CFF>【" + current.Key.ToString() + "件】</color><color=#00d7ffFF>" + ((EquipSkillDataItem) current.Value).GetSkillDisplay(1) + "</color>");
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
            return list;
        }

        private string GetGrayDesc(int index)
        {
            string text = LocalizationGeneralLogic.GetText(string.Format("EquipmentSkill_NotReady_{0}", index), new object[0]);
            return string.Format("<color=#FEDF4CFF>【{0}件】</color><color=#96b1c0FF>{1}</color>", index, text);
        }

        private void SetupSetEffect()
        {
            Transform transform = base.transform.Find("Effect");
            int count = 0;
            Dictionary<int, EquipSkillDataItem> setSkills = null;
            EquipSetDataItem ownEquipSetData = this._avatarData.GetOwnEquipSetData();
            if (ownEquipSetData == null)
            {
                count = 0;
            }
            else
            {
                setSkills = ownEquipSetData.GetOwnSetSkills();
                count = setSkills.Count;
            }
            if (count > 0)
            {
                List<string> list = this.GenerateEffectDesc(setSkills);
                Transform transform2 = transform.Find("SetSkillPanel/ScrollView/Content");
                for (int i = 0; i < transform2.childCount; i++)
                {
                    Transform child = transform2.GetChild(i);
                    if (i >= list.Count)
                    {
                        child.Find("Desc").GetComponent<Text>().text = this.GetGrayDesc(i + 2);
                    }
                    else
                    {
                        child.Find("Desc").GetComponent<Text>().text = list[i];
                    }
                }
            }
            else
            {
                Transform transform4 = transform.Find("SetSkillPanel/ScrollView/Content");
                for (int j = 0; j < transform4.childCount; j++)
                {
                    transform4.GetChild(j).Find("Desc").GetComponent<Text>().text = this.GetGrayDesc(j + 2);
                }
            }
        }

        private void SetupSlots()
        {
            Transform transform = base.transform.Find("Slots");
            EquipmentSlot[] slotArray = new EquipmentSlot[] { 2, 3, 4 };
            for (int i = 1; i <= slotArray.Length; i++)
            {
                transform.Find(i.ToString()).gameObject.GetComponent<MonoAvatarStigmataSlot>().SetupView(this._avatarData, slotArray[i - 1], i, this._isRemoteAvatar);
            }
        }

        public void SetupView(AvatarDataItem avatarData)
        {
            this._isRemoteAvatar = false;
            this._avatarData = avatarData;
            this.SetupSlots();
            this.SetupSetEffect();
        }

        public void SetupView(FriendDetailDataItem userData)
        {
            this._isRemoteAvatar = true;
            this._userData = userData;
            this._avatarData = this._userData.leaderAvatar;
            this.SetupSlots();
            this.SetupSetEffect();
        }
    }
}

