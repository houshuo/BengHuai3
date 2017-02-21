namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoVentureInfoRow : MonoBehaviour
    {
        private Action<VentureDataItem> _onCancelBtnClick;
        private Action<VentureDataItem> _onFetchBtnClick;
        private Action<VentureDataItem> _onGoBtnClick;
        private Action<VentureDataItem> _onSpeedUpBtnClick;
        private VentureDataItem _ventureData;

        private void DoSetupView()
        {
            base.transform.Find("Icon").GetComponent<Image>().sprite = Miscs.GetSpriteByPrefab(this._ventureData.IconPath);
            base.transform.Find("Title/Desc").GetComponent<Text>().text = this._ventureData.VentureName;
            base.transform.Find("Title/Label").GetComponent<Text>().text = (this._ventureData.StaminaCost <= 0) ? LocalizationGeneralLogic.GetText("Menu_Label_CabinVentureTypeWithoutStamina", new object[0]) : LocalizationGeneralLogic.GetText("Menu_Label_CabinVentureTypeWithStamina", new object[0]);
            Transform transform = base.transform.Find("Title/Difficulty");
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive((i + 1) == this._ventureData.Difficulty);
            }
            base.transform.Find("TimeCost").gameObject.SetActive(false);
            base.transform.Find("RemainTime").gameObject.SetActive(false);
            base.transform.Find("Finish").gameObject.SetActive(false);
            base.transform.Find("SpeedUp").gameObject.SetActive(false);
            IEnumerator enumerator = base.transform.Find("Buttons").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.gameObject.SetActive(false);
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
            switch (this._ventureData.status)
            {
                case VentureDataItem.VentureStatus.None:
                    base.transform.Find("TimeCost").gameObject.SetActive(true);
                    base.transform.Find("TimeCost/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(this._ventureData.TimeCost);
                    base.transform.Find("Buttons/Go").gameObject.SetActive(true);
                    break;

                case VentureDataItem.VentureStatus.InProgress:
                {
                    base.transform.Find("RemainTime").gameObject.SetActive(true);
                    base.transform.Find("RemainTime/RemainTimer").GetComponent<MonoRemainTimer>().SetTargetTime(this._ventureData.endTime, null, new Action(this.OnVentureFinish), false);
                    base.transform.Find("Buttons/Cancel").gameObject.SetActive(true);
                    bool flag = Singleton<StorageModule>.Instance.GetAllVentureSpeedUpMaterial().Count > 0;
                    base.transform.Find("SpeedUp").gameObject.SetActive(flag);
                    break;
                }
                case VentureDataItem.VentureStatus.Done:
                    base.transform.Find("Finish").gameObject.SetActive(true);
                    base.transform.Find("Buttons/Fetch").gameObject.SetActive(true);
                    break;
            }
            base.transform.Find("Level/Num").GetComponent<Text>().text = this._ventureData.Level.ToString();
            base.transform.Find("StaminaCost/Num").GetComponent<Text>().text = this._ventureData.StaminaCost.ToString();
            Transform transform3 = base.transform.Find("Rewards");
            int rewardExp = this._ventureData.RewardExp;
            transform3.GetChild(0).gameObject.SetActive(rewardExp > 0);
            if (rewardExp > 0)
            {
                transform3.GetChild(0).Find("Num/Desc").GetComponent<Text>().text = "\x00d7" + rewardExp;
            }
            List<int> rewardItemIDListToShow = this._ventureData.RewardItemIDListToShow;
            for (int j = 1; j < transform3.childCount; j++)
            {
                Transform child = transform3.GetChild(j);
                if (j > rewardItemIDListToShow.Count)
                {
                    child.gameObject.SetActive(false);
                }
                else
                {
                    child.gameObject.SetActive(true);
                    StorageDataItemBase dummyStorageDataItem = Singleton<StorageModule>.Instance.GetDummyStorageDataItem(rewardItemIDListToShow[j - 1], 1);
                    child.GetComponent<MonoLevelDropIconButton>().SetupView(dummyStorageDataItem, new DropItemButtonClickCallBack(this.OnDropItemButtonClick), false, false, false, true);
                }
            }
        }

        public void OnCancelBtnClick()
        {
            if (this._onCancelBtnClick != null)
            {
                this._onCancelBtnClick(this._ventureData);
            }
        }

        private void OnDropItemButtonClick(StorageDataItemBase dropItemData)
        {
            UIUtil.ShowItemDetail(dropItemData, true, true);
        }

        public void OnFetchBtnClick()
        {
            if (this._onFetchBtnClick != null)
            {
                this._onFetchBtnClick(this._ventureData);
            }
        }

        public void OnGoBtnClick()
        {
            if (this._onGoBtnClick != null)
            {
                this._onGoBtnClick(this._ventureData);
            }
        }

        public void OnSpeedUpBtnClick()
        {
            if (this._onSpeedUpBtnClick != null)
            {
                this._onSpeedUpBtnClick(this._ventureData);
            }
        }

        private void OnVentureFinish()
        {
            this._ventureData.status = VentureDataItem.VentureStatus.Done;
            Singleton<NetworkManager>.Instance.RequestIslandVenture();
            this.DoSetupView();
        }

        public void SetupView(VentureDataItem ventureData, Action<VentureDataItem> fetchBtnCallBack, Action<VentureDataItem> goBtnCallBack, Action<VentureDataItem> cancelBtnCallBack, Action<VentureDataItem> speedUpBtnCallBack)
        {
            this._ventureData = ventureData;
            this._onFetchBtnClick = fetchBtnCallBack;
            this._onGoBtnClick = goBtnCallBack;
            this._onCancelBtnClick = cancelBtnCallBack;
            this._onSpeedUpBtnClick = speedUpBtnCallBack;
            this.DoSetupView();
        }
    }
}

