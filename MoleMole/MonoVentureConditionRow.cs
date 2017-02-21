namespace MoleMole
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoVentureConditionRow : MonoBehaviour
    {
        private VentureCondition _condition;
        private int _index;
        private VentureDataItem _ventureData;

        public void SetupView(int index, VentureDataItem ventureData = null)
        {
            this._ventureData = ventureData;
            this._index = index;
            this._condition = ventureData.GetVentureCondition(this._index);
            base.transform.gameObject.SetActive(this._condition != null);
            if (this._condition != null)
            {
                bool flag = this._ventureData.IsConditionMatch(this._condition);
                base.transform.Find("Check").gameObject.SetActive(flag);
                base.transform.Find("UnCheck").gameObject.SetActive(!flag);
                base.transform.Find("Check/Desc").GetComponent<Text>().text = this._condition.desc;
                base.transform.Find("UnCheck/Desc").GetComponent<Text>().text = this._condition.desc;
            }
        }
    }
}

