namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoSkillButtonChargeCount : MonoBehaviour
    {
        private static Dictionary<int, List<int>> PATTERN_MAP;

        static MonoSkillButtonChargeCount()
        {
            Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
            dictionary.Add(0, new List<int>());
            dictionary.Add(1, new List<int> { 3 });
            dictionary.Add(3, new List<int> { 2, 3, 4 });
            dictionary.Add(5, new List<int> { 1, 2, 3, 4, 5 });
            dictionary.Add(2, new List<int> { 2, 3 });
            dictionary.Add(4, new List<int> { 1, 2, 3, 4 });
            PATTERN_MAP = dictionary;
        }

        public void SetupView(int maxCount, int count)
        {
            if (maxCount > 5)
            {
            }
            Transform transform = base.transform.Find("EvenPattern");
            Transform transform2 = base.transform.Find("OddPattern");
            if (maxCount <= 1)
            {
                transform.gameObject.SetActive(false);
                transform2.gameObject.SetActive(false);
            }
            else
            {
                bool flag = (maxCount % 2) == 0;
                transform.gameObject.SetActive(flag);
                transform2.gameObject.SetActive(!flag);
                Transform transform3 = !flag ? transform2 : transform;
                int childCount = transform3.childCount;
                List<int> list = PATTERN_MAP[maxCount];
                for (int i = 1; i <= childCount; i++)
                {
                    Transform transform4 = transform3.Find(i.ToString());
                    if (list.Contains(i))
                    {
                        transform4.gameObject.SetActive(true);
                        bool flag2 = list.IndexOf(i) < count;
                        transform4.Find("Active").gameObject.SetActive(flag2);
                        transform4.Find("Unactive").gameObject.SetActive(!flag2);
                    }
                    else
                    {
                        transform4.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}

