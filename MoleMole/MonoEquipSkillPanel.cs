namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoEquipSkillPanel : MonoBehaviour
    {
        private List<EquipSkillDataItem> _skills;

        public void SetupView(List<EquipSkillDataItem> skills, int equipLevel = 1)
        {
            this._skills = skills;
            if (this._skills != null)
            {
                for (int i = 0; i < base.transform.childCount; i++)
                {
                    Transform transform = base.transform.Find("Skill_" + i);
                    if (transform != null)
                    {
                        if (i < skills.Count)
                        {
                            transform.gameObject.SetActive(true);
                            EquipSkillDataItem item = skills[i];
                            transform.Find("Name").GetComponent<Text>().text = item.skillName;
                            transform.Find("Desc").GetComponent<Text>().text = item.GetSkillDisplay(equipLevel);
                        }
                        else
                        {
                            transform.gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }
}

