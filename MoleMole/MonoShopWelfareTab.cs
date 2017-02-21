namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoShopWelfareTab : MonoBehaviour
    {
        private Action _onGetBtnClick;
        private Transform _scrollViewTrans;
        private List<WelfareDataItem> _welfareDataItemList;

        private bool IsWelfareDataItemEqual(RectTransform dataNew, RectTransform dataOld)
        {
            if ((dataNew == null) || (dataOld == null))
            {
                return false;
            }
            MonoWelfareItem component = dataOld.GetComponent<MonoWelfareItem>();
            return (dataNew.GetComponent<MonoWelfareItem>().GetWelfareDataItem().vipLevel == component.GetWelfareDataItem().vipLevel);
        }

        private void OnScrollChange(Transform trans, int index)
        {
            WelfareDataItem welfareDataItem = this._welfareDataItemList[index];
            trans.GetComponent<MonoWelfareItem>().SetupView(welfareDataItem, this._onGetBtnClick);
        }

        public void SetupView(Action onGetBtnClick = null)
        {
            this._onGetBtnClick = onGetBtnClick;
            this._scrollViewTrans = base.transform.Find("ScrollView");
            this._welfareDataItemList = Singleton<ShopWelfareModule>.Instance.GetWelfareDataItemList();
            MonoGridScroller component = this._scrollViewTrans.GetComponent<MonoGridScroller>();
            component.Init(new MonoGridScroller.OnChange(this.OnScrollChange), this._welfareDataItemList.Count, new Vector2(0f, 1f));
            if (this._scrollViewTrans.gameObject.activeInHierarchy)
            {
                MonoScrollerFadeManager manager = this._scrollViewTrans.GetComponent<MonoScrollerFadeManager>();
                manager.Init(component.GetItemDict(), null, new Func<RectTransform, RectTransform, bool>(this.IsWelfareDataItemEqual));
                manager.Play();
            }
        }
    }
}

