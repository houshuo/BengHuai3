namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.UI;

    public class MonoCabinMainInfo : MonoBehaviour
    {
        private CabinDataItemBase _cabinData;
        private MonoIslandCameraSM _cameraSM;
        private Transform _Day;
        private Text _Day_TextComp;
        private Transform _DayText;
        private float _fetch_scoin_time;
        private Transform _Hrs;
        private Text _Hrs_TextComp;
        private Transform _HrsText;
        private bool _isUpdating;
        private bool _isUpLevel;
        private CabinStatus _lastFrame_cabinStatus;
        private bool _lastFrame_output_active;
        private E_TimeFormat _lastFrame_timeFormat;
        private Transform _Locked;
        private Transform _LvInfo;
        private Transform _LvInfo_Button;
        private Button _LvInfo_Button_UI;
        private Transform _LvInfo_Lv_Lv;
        private Text _LvInfo_Lv_Lv_TextComp;
        private Transform _LvInfo_Lv_Name;
        private Text _LvInfo_Lv_Name_TextComp;
        private Transform _LvInfo_Lv_PopUp_New;
        private Transform _LvInfo_Lv_PopUp_PopUp;
        private Transform _LvInfo_LvUpProgress;
        private Transform _LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider;
        private Image _LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp;
        private Transform _LvInfo_LvUpProgress_TimeRemain_Time;
        private Camera _mainCamera;
        private Transform _Min;
        private Text _Min_TextComp;
        private Transform _MinText;
        private Vector3 _offset = new Vector3(0f, 10f, 0f);
        private Transform _Output;
        private Transform _Sec;
        private Text _Sec_TextComp;
        private Transform _SecText;
        private MonoIslandBuilding _target;
        private Camera _uiCamera;

        private void Awake()
        {
            this._mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
            this._uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            this._cameraSM = GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>();
            this._LvInfo_Lv_PopUp_New = base.transform.Find("LvInfo/Lv/PopUp/New");
            this._LvInfo_Lv_PopUp_PopUp = base.transform.Find("LvInfo/Lv/PopUp/PopUp");
            this._Locked = base.transform.Find("Locked");
            this._LvInfo = base.transform.Find("LvInfo");
            this._Output = base.transform.Find("Output");
            this._LvInfo_Lv_Name = base.transform.Find("LvInfo/Lv/Name");
            this._LvInfo_Lv_Lv = base.transform.Find("LvInfo/Lv/Lv");
            this._LvInfo_LvUpProgress = base.transform.Find("LvInfo/LvUpProgress");
            this._LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider = base.transform.Find("LvInfo/LvUpProgress/TimeRemain/HPSlider/Slider");
            this._LvInfo_LvUpProgress_TimeRemain_Time = base.transform.Find("LvInfo/LvUpProgress/TimeRemain/Time");
            this._Day = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("Day");
            this._Hrs = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("Hrs");
            this._Min = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("Min");
            this._Sec = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("Sec");
            this._DayText = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("DayText");
            this._HrsText = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("HrsText");
            this._MinText = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("MinText");
            this._SecText = this._LvInfo_LvUpProgress_TimeRemain_Time.Find("SecText");
            this._LvInfo_Lv_Name_TextComp = this._LvInfo_Lv_Name.GetComponent<Text>();
            this._LvInfo_Lv_Lv_TextComp = this._LvInfo_Lv_Lv.GetComponent<Text>();
            this._LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp = this._LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider.GetComponent<Image>();
            this._Day_TextComp = this._Day.GetComponent<Text>();
            this._Hrs_TextComp = this._Hrs.GetComponent<Text>();
            this._Min_TextComp = this._Min.GetComponent<Text>();
            this._Sec_TextComp = this._Sec.GetComponent<Text>();
            this._LvInfo_Button = base.transform.Find("LvInfo/Button");
            this._LvInfo_Button_UI = this._LvInfo_Button.GetComponent<Button>();
            this.BindViewCallback(this._LvInfo_Button_UI, new UnityAction(this.OnUIClick));
        }

        public void BindingTargetBuilding(MonoIslandBuilding target, CabinDataItemBase cabinData)
        {
            this._cabinData = cabinData;
            this._target = target;
            this._isUpdating = true;
            Dictionary<CabinType, bool> cabinNeedToShowNewUnlockDict = Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowNewUnlockDict;
            bool flag = !cabinNeedToShowNewUnlockDict.ContainsKey(this._cabinData.cabinType) ? false : cabinNeedToShowNewUnlockDict[this._cabinData.cabinType];
            this._LvInfo_Lv_PopUp_New.gameObject.SetActive(flag);
            this._LvInfo_Lv_PopUp_PopUp.gameObject.SetActive(false);
            if (!flag)
            {
                this.RefreshPopUp();
            }
            this._target.GetModel().RefreshLockStyle(this._cabinData.status);
            bool flag2 = (this._cabinData is CabinCollectDataItem) && (this._cabinData as CabinCollectDataItem).CanFetchScoin();
            this._Output.gameObject.SetActive(flag2);
            this._lastFrame_output_active = flag2;
            this._Locked.gameObject.SetActive(this._cabinData.status == CabinStatus.Locked);
            this._LvInfo.gameObject.SetActive(this._cabinData.status == CabinStatus.UnLocked);
            this._lastFrame_cabinStatus = this._cabinData.status;
            if (this._cabinData.status == CabinStatus.UnLocked)
            {
                this._LvInfo_Lv_Name_TextComp.text = this._cabinData.GetCabinName();
            }
            bool flag3 = this._cabinData.levelUpEndTime > TimeUtil.Now;
            this._LvInfo_LvUpProgress.gameObject.SetActive(flag3);
            if (flag3)
            {
                E_TimeFormat timeFormat = this.GetTimeFormat((TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now));
                this.SetUITimeFormat(timeFormat);
                this._lastFrame_timeFormat = timeFormat;
            }
        }

        private void BindViewCallback(Button button, UnityAction callback)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(callback);
        }

        private E_TimeFormat GetTimeFormat(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1.0)
            {
                return E_TimeFormat.Day;
            }
            if (timeSpan.TotalHours >= 1.0)
            {
                return E_TimeFormat.Hour;
            }
            return E_TimeFormat.Minute;
        }

        private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
        {
            Vector3 position = this._mainCamera.WorldToScreenPoint(worldPosition);
            position.z = this._uiCamera.nearClipPlane + 0.1f;
            return this._uiCamera.ScreenToWorldPoint(position);
        }

        private void OnDestroy()
        {
            this.UnbindViewCallback(this._LvInfo_Button_UI);
        }

        public void OnScoinBtnClick()
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnIslandScoinBtnClick, this._target));
        }

        public void OnUIClick()
        {
            this._cameraSM.GotoState(E_IslandCameraState.ToLanded, this._target);
        }

        public void RefreshPopUp()
        {
            if ((this._cabinData.cabinType == 5) && (Singleton<IslandModule>.Instance.GetVentureDoneNum() > 0))
            {
                this._LvInfo_Lv_PopUp_PopUp.gameObject.SetActive(true);
            }
        }

        public void ReStart()
        {
            this.BindingTargetBuilding(this._target, this._cabinData);
        }

        private void SetUITimeFormat(E_TimeFormat timeFormat)
        {
            if (timeFormat == E_TimeFormat.Day)
            {
                this._Day.gameObject.SetActive(true);
                this._DayText.gameObject.SetActive(true);
                this._Hrs.gameObject.SetActive(true);
                this._HrsText.gameObject.SetActive(true);
                this._Min.gameObject.SetActive(false);
                this._MinText.gameObject.SetActive(false);
                this._Sec.gameObject.SetActive(false);
                this._SecText.gameObject.SetActive(false);
            }
            else if (timeFormat == E_TimeFormat.Hour)
            {
                this._Day.gameObject.SetActive(false);
                this._DayText.gameObject.SetActive(false);
                this._Hrs.gameObject.SetActive(true);
                this._HrsText.gameObject.SetActive(true);
                this._Min.gameObject.SetActive(true);
                this._MinText.gameObject.SetActive(true);
                this._Sec.gameObject.SetActive(false);
                this._SecText.gameObject.SetActive(false);
            }
            else
            {
                this._Day.gameObject.SetActive(false);
                this._DayText.gameObject.SetActive(false);
                this._Hrs.gameObject.SetActive(false);
                this._HrsText.gameObject.SetActive(false);
                this._Min.gameObject.SetActive(true);
                this._MinText.gameObject.SetActive(true);
                this._Sec.gameObject.SetActive(true);
                this._SecText.gameObject.SetActive(true);
            }
        }

        private void UnbindViewCallback(Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (this._isUpdating)
            {
                this.UpdateUIPosition();
                this.UpdateStatus();
                if (this._cabinData.status == CabinStatus.UnLocked)
                {
                    this.UpdateOutput();
                    this.UpdateBackgroundVenture();
                    this.UpdateLevelUp();
                    this.UpdateCabinLevel();
                }
                this._lastFrame_cabinStatus = this._cabinData.status;
            }
        }

        private void UpdateBackgroundVenture()
        {
            if ((this._cabinData.cabinType == 5) && Singleton<IslandModule>.Instance.RefreshVentureBackground())
            {
                Singleton<NetworkManager>.Instance.RequestIslandVenture();
                Singleton<IslandModule>.Instance.UnRegisterVentureInProgress();
            }
        }

        private void UpdateCabinLevel()
        {
            this._LvInfo_Lv_Lv_TextComp.text = "Lv." + this._cabinData.level;
        }

        private void UpdateLevelUp()
        {
            bool flag = this._cabinData.levelUpEndTime > TimeUtil.Now;
            if (this._LvInfo_LvUpProgress.gameObject.activeSelf != flag)
            {
                this._LvInfo_LvUpProgress.gameObject.SetActive(flag);
            }
            if (flag)
            {
                this._isUpLevel = true;
                TimeSpan span = (TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now);
                float num = ((float) span.TotalSeconds) / ((float) this._cabinData.GetCabinLevelUpTimeCost());
                this._LvInfo_LvUpProgress_TimeRemain_HPSlider_Slider_ImageComp.fillAmount = num;
                this.UpdateRemainTime();
            }
            else if (this._isUpLevel)
            {
                this._isUpLevel = false;
                Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinLevelUpSucc, this._target));
                Singleton<NetworkManager>.Instance.RequestGetIsland();
            }
        }

        private void UpdateOutput()
        {
            CabinCollectDataItem item = this._cabinData as CabinCollectDataItem;
            if (item != null)
            {
                bool flag = item.HasScoin();
                if (this._lastFrame_output_active != flag)
                {
                    this._Output.gameObject.SetActive(flag);
                }
                this._lastFrame_output_active = flag;
                if ((Time.time > (this._fetch_scoin_time + 2f)) && item.TimeToFetch())
                {
                    Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
                    this._fetch_scoin_time = Time.time;
                }
            }
        }

        private void UpdateRemainTime()
        {
            TimeSpan timeSpan = (TimeSpan) (this._cabinData.levelUpEndTime - TimeUtil.Now);
            this._Day_TextComp.text = string.Format("{0:D2}", timeSpan.Days);
            this._Hrs_TextComp.text = string.Format("{0:D2}", timeSpan.Hours);
            this._Min_TextComp.text = string.Format("{0:D2}", timeSpan.Minutes);
            this._Sec_TextComp.text = string.Format("{0:D2}", timeSpan.Seconds);
            E_TimeFormat timeFormat = this.GetTimeFormat(timeSpan);
            if (this._lastFrame_timeFormat != timeFormat)
            {
                this.SetUITimeFormat(timeFormat);
            }
            this._lastFrame_timeFormat = timeFormat;
        }

        private void UpdateStatus()
        {
            if (this._lastFrame_cabinStatus != this._cabinData.status)
            {
                this._Locked.gameObject.SetActive(this._cabinData.status == CabinStatus.Locked);
                this._LvInfo.gameObject.SetActive(this._cabinData.status == CabinStatus.UnLocked);
            }
        }

        private void UpdateUIPosition()
        {
            Vector3 worldToUIPosition = this.GetWorldToUIPosition(this._target.transform.position + this._offset);
            base.transform.position = worldToUIPosition;
        }
    }
}

