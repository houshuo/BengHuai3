namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoStigmataSetSkillPanel : MonoBehaviour
    {
        public void SetupView(StigmataDataItem stigmataData, SortedDictionary<int, EquipSkillDataItem> setSkills)
        {
            for (int i = 0; i < base.transform.childCount; i++)
            {
                int key = i + 2;
                Transform transform = base.transform.Find("SetSkill_" + key);
                if (transform != null)
                {
                    EquipSkillDataItem item;
                    setSkills.TryGetValue(key, out item);
                    if (item == null)
                    {
                        transform.gameObject.SetActive(false);
                    }
                    else
                    {
                        transform.Find("Desc").GetComponent<Text>().text = item.GetSkillDisplay(1);
                    }
                }
            }
        }
    }
}

