namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    public class MonsterShowPageContext : BasePageContext
    {
        private int _currentMonsterIndex;
        private Dictionary<int, MonsterUIMetaData> _monsterDataDict;
        private Dictionary<int, GameObject> _monsterGameObjectDict;
        private List<int> _monsterIDList;
        private const string MONSTER_SKILL_PREFAB_PATH = "UI/Menus/Widget/Map/MonsterSkillRow";

        public MonsterShowPageContext(List<int> monsterIDList)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "MonsterShowPageContext",
                viewPrefabPath = "UI/Menus/Page/Map/MonsterShowPage"
            };
            base.config = pattern;
            this._monsterIDList = monsterIDList;
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("FuncBtn/LastPage").GetComponent<Button>(), new UnityAction(this.OnLastPageButtonClick));
            base.BindViewCallback(base.view.transform.Find("FuncBtn/NextPage").GetComponent<Button>(), new UnityAction(this.OnNextPageButtonClick));
            base.BindViewCallback(base.view.transform.Find("BackBtn").GetComponent<Button>(), new UnityAction(this.OnBackBtnClick));
        }

        private void Init()
        {
            if (this._monsterIDList.Count > 0)
            {
                this._monsterDataDict = new Dictionary<int, MonsterUIMetaData>();
                this._monsterGameObjectDict = new Dictionary<int, GameObject>();
                foreach (int num in this._monsterIDList)
                {
                    MonsterUIMetaData monsterUIMetaDataByKey = MonsterUIMetaDataReader.GetMonsterUIMetaDataByKey(num);
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(monsterUIMetaDataByKey.prefabPath, BundleType.RESOURCE_FILE));
                    obj2.transform.SetParent(base.view.transform.Find("Monster3dModel"), false);
                    obj2.SetActive(false);
                    this._monsterDataDict.Add(num, monsterUIMetaDataByKey);
                    this._monsterGameObjectDict.Add(num, obj2);
                }
                this._currentMonsterIndex = 0;
                this.ShowMonsterByIndex(this._monsterIDList[this._currentMonsterIndex]);
            }
        }

        public void OnBackBtnClick()
        {
            Singleton<MainUIManager>.Instance.BackPage();
        }

        public void OnLastPageButtonClick()
        {
            this._currentMonsterIndex--;
            this._currentMonsterIndex += this._monsterIDList.Count;
            this._currentMonsterIndex = this._currentMonsterIndex % this._monsterIDList.Count;
            this.ShowMonsterByIndex(this._monsterIDList[this._currentMonsterIndex]);
        }

        public void OnNextPageButtonClick()
        {
            this._currentMonsterIndex++;
            this._currentMonsterIndex = this._currentMonsterIndex % this._monsterIDList.Count;
            this.ShowMonsterByIndex(this._monsterIDList[this._currentMonsterIndex]);
        }

        public override bool OnNotify(Notify ntf)
        {
            return false;
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            return false;
        }

        protected override bool SetupView()
        {
            this.Init();
            return false;
        }

        private void ShowMonsterByIndex(int index)
        {
            MonsterUIMetaData data;
            this._monsterDataDict.TryGetValue(index, out data);
            if (data != null)
            {
                foreach (KeyValuePair<int, GameObject> pair in this._monsterGameObjectDict)
                {
                    pair.Value.SetActive(pair.Key == index);
                }
                base.view.transform.Find("Info/Name/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(data.displayTitle, new object[0]);
                base.view.transform.Find("Info/Type/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(data.displayType, new object[0]);
                base.view.transform.Find("Desc/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(data.displayIntroduction, new object[0]);
                base.view.transform.Find("Info/HP/Slider").GetComponent<Slider>().value = data.HP;
                base.view.transform.Find("Info/ATK/Slider").GetComponent<Slider>().value = data.attack;
                base.view.transform.Find("Info/DEF/Slider").GetComponent<Slider>().value = data.defence;
                base.view.transform.Find("Info/SPD/Slider").GetComponent<Slider>().value = data.speed;
                base.view.transform.Find("Info/RND/Slider").GetComponent<Slider>().value = data.range;
                Transform parent = base.view.transform.Find("Skill/SkillListPanel");
                IEnumerator enumerator = parent.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        UnityEngine.Object.Destroy(current.gameObject);
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
                foreach (int num in data.monsterSkillIDList)
                {
                    MonsterSkillMetaData monsterSkillMetaDataByKey = MonsterSkillMetaDataReader.GetMonsterSkillMetaDataByKey(num);
                    GameObject obj2 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Map/MonsterSkillRow", BundleType.RESOURCE_FILE));
                    obj2.transform.SetParent(parent, false);
                    obj2.transform.Find("SkillName/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(monsterSkillMetaDataByKey.displayName, new object[0]);
                    obj2.transform.Find("SkillDescription/Text").GetComponent<Text>().text = LocalizationGeneralLogic.GetText(monsterSkillMetaDataByKey.displayDetail, new object[0]);
                }
            }
        }
    }
}

