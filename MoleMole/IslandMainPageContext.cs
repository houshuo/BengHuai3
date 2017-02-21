namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public class IslandMainPageContext : BasePageContext
    {
        private Dictionary<MonoIslandBuilding, CabinDataItemBase> _buildingDataDict;
        private Dictionary<CabinType, MonoIslandBuilding> _cabinBuildingDict;
        private CanvasTimer _cabinLevelUpEffectDelayTimer;
        private Camera _mainCamera;
        private Vector3 _offset = new Vector3(0f, 10f, 0f);
        private Camera _uiCamera;
        private const string CABIN_BASE_INFO_PREFAB_PATH = "UI/Menus/Widget/Island/CabinInfoUI";
        private float FETCH_SCOIN_MISSION_RATIO_TOTAL = 200f;

        public IslandMainPageContext(GameObject view, Dictionary<MonoIslandBuilding, CabinDataItemBase> buildingDataDict)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "IslandMainPageContext",
                viewPrefabPath = "UI/Menus/Page/Island/IslandMainPage",
                cacheType = ViewCacheType.AlwaysCached
            };
            base.config = pattern;
            base.view = view;
            this._buildingDataDict = buildingDataDict;
            this._cabinBuildingDict = new Dictionary<CabinType, MonoIslandBuilding>();
            foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair in this._buildingDataDict)
            {
                this._cabinBuildingDict[pair.Value.cabinType] = pair.Key;
            }
        }

        public override void BackPage()
        {
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, null, true);
        }

        public override void BackToMainMenuPage()
        {
            Singleton<MainUIManager>.Instance.MoveToNextScene("MainMenuWithSpaceship", false, true, true, null, true);
        }

        protected override void BindViewCallbacks()
        {
            base.BindViewCallback(base.view.transform.Find("ControllPanel/CamResetBtn").GetComponent<Button>(), new UnityAction(this.OnCamResetBtnClick));
            base.BindViewCallback(base.view.transform.Find("ControllPanel/CamZoomOutBtn").GetComponent<Button>(), new UnityAction(this.OnCamZoomOutBtnClick));
        }

        public override void Destroy()
        {
            base.Destroy();
        }

        private Vector3 GetWorldToUIPosition(Vector3 worldPosition)
        {
            Vector3 position = this._mainCamera.WorldToScreenPoint(worldPosition);
            position.z = Mathf.Clamp(position.z, this._uiCamera.nearClipPlane, this._uiCamera.farClipPlane);
            return this._uiCamera.ScreenToWorldPoint(position);
        }

        private bool OnCabinLevelUpSucc(MonoIslandBuilding building)
        {
            this.PlayEffect(base.view.transform.Find("EffectContainer/IslandCabinLvUp"), building, true);
            return false;
        }

        private void OnCamResetBtnClick()
        {
            GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().CameraToBasePos();
        }

        private void OnCamZoomOutBtnClick()
        {
            GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().ExitFocusing();
        }

        private bool OnGetIslandVentureRsp(GetIslandVentureRsp rsp)
        {
            if (rsp.get_retcode() == null)
            {
                IEnumerator enumerator = base.view.transform.Find("CabinBaseInfoPanel").GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Transform current = (Transform) enumerator.Current;
                        current.GetComponent<MonoCabinMainInfo>().RefreshPopUp();
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
            }
            return false;
        }

        private bool OnIslandCameraLanded(MonoIslandBuilding building)
        {
            Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Building_ZoomIn", null, null, null);
            Singleton<MainUIManager>.Instance.ShowPage(new CabinOverviewPageContext(this._buildingDataDict[building], this._buildingDataDict), UIType.Page);
            return false;
        }

        private bool OnIslandCollectRsp(IslandCollectRsp rsp)
        {
            <OnIslandCollectRsp>c__AnonStoreyFE yfe = new <OnIslandCollectRsp>c__AnonStoreyFE {
                rsp = rsp,
                <>f__this = this
            };
            if (yfe.rsp.get_retcode() == null)
            {
                <OnIslandCollectRsp>c__AnonStoreyFD yfd = new <OnIslandCollectRsp>c__AnonStoreyFD {
                    <>f__ref$254 = yfe,
                    <>f__this = this
                };
                Transform effectTrans = base.view.transform.Find("EffectContainer/IslandCollectionGoldCoin");
                yfd.fetchScoin = (int) yfe.rsp.get_add_scoin();
                CabinCollectDataItem item = this._buildingDataDict[this._cabinBuildingDict[3]] as CabinCollectDataItem;
                yfd.burstRate = (!yfe.rsp.get_is_extraSpecified() || !yfe.rsp.get_is_extra()) ? 1f : item.crtExtraRatio;
                ParticleSystem.EmissionModule emission = effectTrans.GetComponent<ParticleSystem>().emission;
                ParticleSystem.MinMaxCurve curve = new ParticleSystem.MinMaxCurve {
                    constantMax = Mathf.Clamp((float) (((float) yfd.fetchScoin) / item.topLimit), (float) 0.1f, (float) 1f) * this.FETCH_SCOIN_MISSION_RATIO_TOTAL
                };
                emission.rate = curve;
                Singleton<WwiseAudioManager>.Instance.Post("UI_Island_Collect_Gold", null, null, null);
                this.PlayEffect(effectTrans, this._cabinBuildingDict[3], true);
                Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.2f, 0f).timeUpCallback = new Action(yfd.<>m__17E);
            }
            return false;
        }

        public override void OnLandedFromBackPage()
        {
            GameObject.Find("IslandCameraGroup").GetComponent<MonoIslandCameraSM>().BackToBase();
            IEnumerator enumerator = base.view.transform.Find("EffectContainer").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    foreach (ParticleSystem system in current.GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
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
            IEnumerator enumerator2 = base.view.transform.Find("CabinBaseInfoPanel").GetEnumerator();
            try
            {
                while (enumerator2.MoveNext())
                {
                    Transform transform3 = (Transform) enumerator2.Current;
                    transform3.GetComponent<MonoCabinMainInfo>().ReStart();
                }
            }
            finally
            {
                IDisposable disposable2 = enumerator2 as IDisposable;
                if (disposable2 == null)
                {
                }
                disposable2.Dispose();
            }
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.OnIslandCameraPreLanded)
            {
                return this.OnIslandCameraLanded((MonoIslandBuilding) ntf.body);
            }
            if (ntf.type == NotifyTypes.OnCabinLevelUpSucc)
            {
                return this.OnCabinLevelUpSucc((MonoIslandBuilding) ntf.body);
            }
            return ((ntf.type == NotifyTypes.OnIslandScoinBtnClick) && this.OnScoinBtnClick((MonoIslandBuilding) ntf.body));
        }

        public override bool OnPacket(NetPacketV1 pkt)
        {
            switch (pkt.getCmdId())
            {
                case 0xb6:
                    this.OnIslandCollectRsp(pkt.getData<IslandCollectRsp>());
                    break;

                case 0xa9:
                    return this.OnGetIslandVentureRsp(pkt.getData<GetIslandVentureRsp>());
            }
            return false;
        }

        private bool OnScoinBtnClick(MonoIslandBuilding building)
        {
            Singleton<NetworkManager>.Instance.RequestIslandCollect();
            return false;
        }

        private void PlayEffect(Transform effectTrans, MonoIslandBuilding building, bool toUIPos = false)
        {
            effectTrans.position = !toUIPos ? building.transform.position : this.GetWorldToUIPosition(building.transform.position + this._offset);
            foreach (ParticleSystem system in effectTrans.GetComponentsInChildren<ParticleSystem>())
            {
                system.Play();
            }
        }

        public override void SetActive(bool enabled)
        {
            base._notifyQueue.Clear();
            base.SetActive(enabled);
        }

        protected override bool SetupView()
        {
            this._mainCamera = GameObject.Find("IslandCameraGroup/MainCamera").GetComponent<Camera>();
            this._uiCamera = GameObject.Find("UICamera").GetComponent<Camera>();
            IEnumerator enumerator = base.view.transform.Find("EffectContainer").GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    foreach (ParticleSystem system in current.GetComponentsInChildren<ParticleSystem>())
                    {
                        system.Stop();
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
            Singleton<NetworkManager>.Instance.RequestGetCollectCabin();
            Transform trans = base.view.transform.Find("CabinBaseInfoPanel");
            trans.DestroyChildren();
            foreach (KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair in this._buildingDataDict)
            {
                Transform transform = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("UI/Menus/Widget/Island/CabinInfoUI")).transform;
                transform.SetParent(trans, false);
                transform.GetComponent<MonoCabinMainInfo>().BindingTargetBuilding(pair.Key, pair.Value);
                transform.gameObject.name = string.Format("CabinInfoUI_{0}", (int) pair.Value.cabinType);
            }
            <SetupView>c__AnonStoreyFC yfc = new <SetupView>c__AnonStoreyFC {
                <>f__this = this
            };
            using (Dictionary<MonoIslandBuilding, CabinDataItemBase>.Enumerator enumerator3 = this._buildingDataDict.GetEnumerator())
            {
                while (enumerator3.MoveNext())
                {
                    yfc.pair = enumerator3.Current;
                    if (yfc.pair.Value.NeedToShowLevelUpComplete())
                    {
                        this._cabinLevelUpEffectDelayTimer = Singleton<MainUIManager>.Instance.SceneCanvas.CreateTimer(0.5f, 0f);
                        this._cabinLevelUpEffectDelayTimer.timeUpCallback = new Action(yfc.<>m__17D);
                        this._cabinLevelUpEffectDelayTimer.StartRun(false);
                        goto Label_0212;
                    }
                }
            }
        Label_0212:
            return false;
        }

        private void ShowCabinLevelUpCompleteEffect(MonoIslandBuilding target, CabinDataItemBase cabinData)
        {
            Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.OnCabinLevelUpSucc, target));
            Singleton<MiHoYoGameData>.Instance.LocalData.CabinNeedToShowLevelUpCompleteSet[cabinData.cabinType] = false;
            Singleton<MiHoYoGameData>.Instance.Save();
            this._cabinLevelUpEffectDelayTimer.Destroy();
        }

        private void ShowGetScoinHintDialog(int scoinNum, float burstRate, List<DropItem> dropItems)
        {
            if (Singleton<MainUIManager>.Instance.SceneCanvas != null)
            {
                Singleton<MainUIManager>.Instance.ShowDialog(new IslandCollectGotDialogContext(scoinNum, burstRate, dropItems), UIType.Any);
            }
        }

        public override void StartUp(Transform canvasTrans, Transform viewParent = null)
        {
            base.StartUp(canvasTrans, viewParent);
        }

        [CompilerGenerated]
        private sealed class <OnIslandCollectRsp>c__AnonStoreyFD
        {
            internal IslandMainPageContext.<OnIslandCollectRsp>c__AnonStoreyFE <>f__ref$254;
            internal IslandMainPageContext <>f__this;
            internal float burstRate;
            internal int fetchScoin;

            internal void <>m__17E()
            {
                this.<>f__this.ShowGetScoinHintDialog(this.fetchScoin, this.burstRate, this.<>f__ref$254.rsp.get_drop_item_list());
            }
        }

        [CompilerGenerated]
        private sealed class <OnIslandCollectRsp>c__AnonStoreyFE
        {
            internal IslandMainPageContext <>f__this;
            internal IslandCollectRsp rsp;
        }

        [CompilerGenerated]
        private sealed class <SetupView>c__AnonStoreyFC
        {
            internal IslandMainPageContext <>f__this;
            internal KeyValuePair<MonoIslandBuilding, CabinDataItemBase> pair;

            internal void <>m__17D()
            {
                this.<>f__this.ShowCabinLevelUpCompleteEffect(this.pair.Key, this.pair.Value);
            }
        }
    }
}

