namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class MonoLevelDebug : MonoBehaviour
    {
        private const float BEGIN_POS_X = 330f;
        private const float BEGIN_POS_Y = -60f;
        public Image dynamicLvImg;
        public string luaName = "Level0.lua";
        private const float POS_Y_OFFSET = 60f;
        public Transform scrollArea;
        public List<MonoLevelDebugToggle> toggleList;
        public bool useDynamicLevel;

        public void OnClickDebugButton()
        {
            base.transform.gameObject.SetActive(!base.transform.gameObject.activeSelf);
        }

        public void OnClickDynamicLvButton()
        {
            this.useDynamicLevel = !this.useDynamicLevel;
            if (this.useDynamicLevel)
            {
                this.dynamicLvImg.color = Color.red;
            }
            else
            {
                this.dynamicLvImg.color = Color.white;
            }
        }

        public void OnClickLevelButton()
        {
            Singleton<LevelScoreManager>.Create();
            Singleton<LevelScoreManager>.Instance.SetDebugLevelBeginIntent(this.luaName);
            Singleton<MainUIManager>.Instance.CurrentPageContext.BackPage();
            ChapterSelectPageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext as ChapterSelectPageContext;
            if (currentPageContext != null)
            {
                currentPageContext.OnDoLevelBegin();
            }
            Singleton<MainUIManager>.Instance.MoveToNextScene("TestLevel01", true, true, true, null, true);
        }

        public void Refresh(MonoLevelDebugToggle theToggle)
        {
            for (int i = 0; i < this.toggleList.Count; i++)
            {
                if (this.toggleList[i] != theToggle)
                {
                    this.toggleList[i].toggle.isOn = false;
                }
            }
        }

        private void Start()
        {
            this.toggleList = new List<MonoLevelDebugToggle>();
            int num = 0;
            foreach (string str in DesignDataTemp.LEVEL_LUA_ENTRY_FILE_NAMES["Common"])
            {
                Transform transform = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/DevLevel/DebugLevelToggle", BundleType.RESOURCE_FILE)).transform;
                transform.SetParent(this.scrollArea, false);
                RectTransform transform2 = (RectTransform) transform;
                transform2.anchoredPosition = new Vector3(330f, -60f - (num * 60f), 0f);
                MonoLevelDebugToggle component = transform.GetComponent<MonoLevelDebugToggle>();
                component.luaName = str;
                component.luaNameText.text = str;
                component.levelDebug = this;
                this.toggleList.Add(component);
                num++;
            }
            RectTransform scrollArea = (RectTransform) this.scrollArea;
            scrollArea.sizeDelta = new Vector2(0f, num * 60f);
        }
    }
}

