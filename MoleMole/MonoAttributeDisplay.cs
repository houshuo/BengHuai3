namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteInEditMode]
    public class MonoAttributeDisplay : MonoBehaviour
    {
        private AvatarDataItem _avatarData;
        private StorageDataItemBase _item;
        private AvatarDataItem _ownerData;
        private readonly float posX_two_Column;
        private readonly float[] spacingX;

        public MonoAttributeDisplay()
        {
            float[] singleArray1 = new float[4];
            singleArray1[2] = 140f;
            singleArray1[3] = 36f;
            this.spacingX = singleArray1;
            this.posX_two_Column = -23f;
        }

        private void SetupGrid()
        {
            int num = 0;
            IEnumerator enumerator = base.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    if (current.gameObject.activeSelf)
                    {
                        num++;
                    }
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
            GridLayoutGroup component = base.transform.GetComponent<GridLayoutGroup>();
            int num2 = (num <= 3) ? 1 : 2;
            int index = Mathf.CeilToInt((1f * num) / ((float) num2));
            RectTransform transform2 = base.GetComponent<RectTransform>();
            float x = (index != 2) ? 0f : this.posX_two_Column;
            transform2.anchoredPosition = new Vector2(x, transform2.anchoredPosition.y);
            component.constraintCount = num2;
            component.spacing = new Vector2(this.spacingX[index], component.spacing.y);
        }

        private void SetupStatus()
        {
            if (this._item != null)
            {
                StigmataDataItem item = this._item as StigmataDataItem;
                float hPAdd = this._item.GetHPAdd();
                if (item != null)
                {
                    hPAdd = item.GetHPAddWithAffix(this._ownerData);
                }
                base.transform.Find("HP").gameObject.SetActive(hPAdd > 0f);
                base.transform.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(hPAdd).ToString();
                float sPAdd = this._item.GetSPAdd();
                if (item != null)
                {
                    sPAdd = item.GetSPAddWithAffix(this._ownerData);
                }
                base.transform.Find("SP").gameObject.SetActive(sPAdd > 0f);
                base.transform.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(sPAdd).ToString();
                float attackAdd = this._item.GetAttackAdd();
                if (item != null)
                {
                    attackAdd = item.GetAttackAddWithAffix(this._ownerData);
                }
                base.transform.Find("ATK").gameObject.SetActive(attackAdd > 0f);
                base.transform.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(attackAdd).ToString();
                float defenceAdd = this._item.GetDefenceAdd();
                if (item != null)
                {
                    defenceAdd = item.GetDefenceAddWithAffix(this._ownerData);
                }
                base.transform.Find("DEF").gameObject.SetActive(defenceAdd > 0f);
                base.transform.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(defenceAdd).ToString();
                float criticalAdd = this._item.GetCriticalAdd();
                if (item != null)
                {
                    criticalAdd = item.GetCriticalAddWithAffix(this._ownerData);
                }
                base.transform.Find("CRT").gameObject.SetActive(criticalAdd > 0f);
                base.transform.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(criticalAdd).ToString();
                if (base.transform.Find("Cost") != null)
                {
                    base.transform.Find("Cost").gameObject.SetActive(this._item.GetCost() > 0);
                    base.transform.Find("Cost/Num").GetComponent<Text>().text = Mathf.FloorToInt((float) this._item.GetCost()).ToString();
                }
            }
            else if (this._avatarData != null)
            {
                base.transform.Find("HP").gameObject.SetActive(this._avatarData.FinalHPUI > 0f);
                base.transform.Find("HP/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalHPUI).ToString();
                base.transform.Find("SP").gameObject.SetActive(this._avatarData.FinalSPUI > 0f);
                base.transform.Find("SP/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalSPUI).ToString();
                base.transform.Find("ATK").gameObject.SetActive(this._avatarData.FinalAttackUI > 0f);
                base.transform.Find("ATK/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalAttackUI).ToString();
                base.transform.Find("DEF").gameObject.SetActive(this._avatarData.FinalDefenseUI > 0f);
                base.transform.Find("DEF/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalDefenseUI).ToString();
                base.transform.Find("CRT").gameObject.SetActive(this._avatarData.FinalDefenseUI > 0f);
                base.transform.Find("CRT/Num").GetComponent<Text>().text = Mathf.FloorToInt(this._avatarData.FinalCriticalUI).ToString();
                if (base.transform.Find("Cost") != null)
                {
                    base.transform.Find("Cost").gameObject.SetActive(this._avatarData.GetCurrentCost() > 0);
                    base.transform.Find("Cost/Num").GetComponent<Text>().text = Mathf.FloorToInt((float) this._avatarData.GetCurrentCost()).ToString();
                }
            }
        }

        public void SetupView(AvatarDataItem avatarData)
        {
            this._avatarData = avatarData;
            this.SetupStatus();
            this.SetupGrid();
        }

        public void SetupView(StorageDataItemBase item, AvatarDataItem ownerData = null)
        {
            this._item = item;
            this._ownerData = ownerData;
            this.SetupStatus();
            this.SetupGrid();
        }
    }
}

