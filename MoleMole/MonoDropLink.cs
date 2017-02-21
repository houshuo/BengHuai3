namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoDropLink : MonoBehaviour
    {
        private Action<LevelDataItem> _customeLevelClickCallBack;
        private LevelDataItem _levelData;

        public void OnDropLinkBtnClick()
        {
            if (this._levelData != null)
            {
                if (this._customeLevelClickCallBack != null)
                {
                    this._customeLevelClickCallBack(this._levelData);
                }
                else if (Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas)
                {
                    this.ShowChapterSelectPage();
                }
                else
                {
                    Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, new Action(this.ShowChapterSelectPage), true);
                }
            }
        }

        public void SetupView(LevelDataItem levelData, Action<LevelDataItem> customeLevelClickCallBack = null)
        {
            this._levelData = levelData;
            this._customeLevelClickCallBack = customeLevelClickCallBack;
            if (levelData == null)
            {
                base.gameObject.SetActive(false);
            }
            else
            {
                base.gameObject.SetActive(true);
                bool flag = (levelData.status != 1) && (levelData.UnlockPlayerLevel <= Singleton<PlayerModule>.Instance.playerData.teamLevel);
                base.transform.Find("Open").gameObject.SetActive(flag);
                base.transform.Find("Lock").gameObject.SetActive(!flag);
                Text text = !flag ? base.transform.Find("Lock/Text").GetComponent<Text>() : base.transform.Find("Open/Text").GetComponent<Text>();
                text.text = levelData.StageName;
                Button component = base.transform.Find("Open").GetComponent<Button>();
                component.onClick.RemoveAllListeners();
                component.onClick.AddListener(new UnityAction(this.OnDropLinkBtnClick));
            }
        }

        private void ShowChapterSelectPage()
        {
            Singleton<MainUIManager>.Instance.ShowPage(new ChapterSelectPageContext(this._levelData), UIType.Page);
        }
    }
}

