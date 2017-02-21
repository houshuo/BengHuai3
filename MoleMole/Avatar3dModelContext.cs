namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UniRx;
    using UnityEngine;

    public class Avatar3dModelContext : BaseWidgetContext
    {
        private Dictionary<int, float> _avatarDefaultYDict;
        private Dictionary<int, Transform> _avatarModelDict;
        private Coroutine _createAvatarCoroutine;
        private bool _setAvatarPosFlag = true;

        public Avatar3dModelContext(GameObject view = null)
        {
            ContextPattern pattern = new ContextPattern {
                contextName = "Avatar3dModelContext",
                viewPrefabPath = "UI/Menus/Widget/AvatarContainer",
                cacheType = ViewCacheType.DontCache,
                dontDestroyView = true
            };
            base.config = pattern;
            this._avatarModelDict = new Dictionary<int, Transform>();
            this._avatarDefaultYDict = new Dictionary<int, float>();
            base.view = view;
        }

        public bool ContainUIAvatar(int avatarID)
        {
            return this._avatarModelDict.ContainsKey(avatarID);
        }

        [DebuggerHidden]
        private IEnumerator DoCreateAvatarUIModels(List<Avatar3dModelDataItem> avatarDataList)
        {
            return new <DoCreateAvatarUIModels>c__Iterator68 { avatarDataList = avatarDataList, <$>avatarDataList = avatarDataList, <>f__this = this };
        }

        public List<Transform> GetAllAvatars()
        {
            List<Transform> list = new List<Transform>();
            foreach (Transform transform in this._avatarModelDict.Values)
            {
                list.Add(transform);
            }
            return list;
        }

        public Transform GetAvatarById(int avatarID)
        {
            return this._avatarModelDict[avatarID];
        }

        private string GetAvatarPrefaPath(string avatarRegistryKey)
        {
            return string.Format("Entities/Avatar/{0}/Avatar_{0}_UI", avatarRegistryKey);
        }

        public override bool OnNotify(Notify ntf)
        {
            if (ntf.type == NotifyTypes.CreateAvatarUIModels)
            {
                return this.OnRecvCreateAvatarNotify((List<Avatar3dModelDataItem>) ntf.body);
            }
            if (ntf.type == NotifyTypes.SetSpaceShipActive)
            {
                return this.OnSetSpaceShipActive(((Tuple<bool, bool>) ntf.body).Item1, ((Tuple<bool, bool>) ntf.body).Item2);
            }
            return ((ntf.type == NotifyTypes.PlayAvtarChangeEffect) && this.PlayAvtarChangeEffect());
        }

        private bool OnRecvCreateAvatarNotify(List<Avatar3dModelDataItem> avatarDataList)
        {
            if (this._createAvatarCoroutine != null)
            {
                Singleton<ApplicationManager>.Instance.StopCoroutine(this._createAvatarCoroutine);
            }
            this._createAvatarCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoCreateAvatarUIModels(avatarDataList));
            return false;
        }

        private bool OnSetSpaceShipActive(bool active, bool setCameraComponentOnly = false)
        {
            base.view.SetActive(active);
            return false;
        }

        private bool PlayAvtarChangeEffect()
        {
            base.view.transform.Find("AvatarChangeEffect").gameObject.SetActive(true);
            base.view.transform.Find("AvatarChangeEffect").GetComponent<ParticleSystem>().Play();
            return false;
        }

        private void SetAvatarAttachWeaponView(Transform modelTrans, AvatarDataItem avatarData)
        {
            WeaponDataItem weapon = avatarData.GetWeapon();
            if (weapon != null)
            {
                BaseMonoUIAvatar component = modelTrans.GetComponent<BaseMonoUIAvatar>();
                if (component.WeaponMetaID != weapon.ID)
                {
                    component.AttachWeapon(weapon.ID, avatarData.AvatarRegistryKey);
                }
            }
        }

        private void SetAvatarModelView(Transform modelTrans, AvatarDataItem avatarData, Vector3 pos, Vector3 eulerAngles, bool showLockViewIfLock)
        {
            if (this._setAvatarPosFlag)
            {
                modelTrans.SetLocalPositionX(pos.x);
                modelTrans.SetLocalPositionZ(pos.z);
                modelTrans.SetLocalPositionY(this._avatarDefaultYDict[avatarData.avatarID] + pos.y);
                modelTrans.GetComponent<BaseMonoUIAvatar>().SetOriginPos(modelTrans.position);
                MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                if (sceneCanvas != null)
                {
                    modelTrans.Translate(0f, 0f, 20f);
                }
            }
            modelTrans.eulerAngles = eulerAngles;
            bool active = showLockViewIfLock && !avatarData.UnLocked;
            this.SetLockViewActive(active);
            Transform transform = base.view.transform.Find("Lock");
            transform.position = new Vector3(modelTrans.position.x, transform.position.y, modelTrans.position.z);
            this.SetAvatarAttachWeaponView(modelTrans, avatarData);
        }

        private void SetLockViewActive(bool active)
        {
            base.view.transform.Find("Lock").gameObject.SetActive(active);
        }

        public void SetStandOnSpaceship(int avatarID)
        {
            Transform transform = this._avatarModelDict[avatarID];
            transform.GetComponent<BaseMonoUIAvatar>().standOnSpaceshipInGameEntry = true;
        }

        protected override bool SetupView()
        {
            base.view.name = "AvatarContainer";
            this.SetLockViewActive(false);
            base.view.transform.Find("AvatarChangeEffect").gameObject.SetActive(false);
            return false;
        }

        public void TriggerAvatarTurnAround(int avatarID)
        {
            Transform transform = this._avatarModelDict[avatarID];
            transform.GetComponent<Animator>().SetTrigger("TriggerTurnAround");
            Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim = true;
        }

        public void TriggerStartGalTouch()
        {
            List<Transform> allAvatars = this.GetAllAvatars();
            int num = 0;
            int count = allAvatars.Count;
            while (num < count)
            {
                BaseMonoUIAvatar component = allAvatars[num].GetComponent<BaseMonoUIAvatar>();
                if (component != null)
                {
                    component.EnterGalTouch();
                }
                num++;
            }
        }

        public void TriggerStartGalTouch(int avatarID)
        {
            BaseMonoUIAvatar component = this._avatarModelDict[avatarID].GetComponent<BaseMonoUIAvatar>();
            if (component != null)
            {
                component.EnterGalTouch();
            }
        }

        public void TriggerStopGalTouch()
        {
            foreach (Transform transform in this._avatarModelDict.Values)
            {
                BaseMonoUIAvatar component = transform.GetComponent<BaseMonoUIAvatar>();
                if (component != null)
                {
                    component.ExitGalTouch();
                }
            }
        }

        [CompilerGenerated]
        private sealed class <DoCreateAvatarUIModels>c__Iterator68 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal List<Avatar3dModelDataItem> <$>avatarDataList;
            internal Dictionary<int, Transform>.KeyCollection.Enumerator <$s_1828>__1;
            internal List<int>.Enumerator <$s_1829>__3;
            internal List<Avatar3dModelDataItem>.Enumerator <$s_1830>__5;
            internal Avatar3dModelContext <>f__this;
            internal int <avatarId>__2;
            internal Avatar3dModelDataItem <data>__6;
            internal int <key>__4;
            internal bool <loadFlag>__11;
            internal Transform <modelTrans>__7;
            internal BaseMonoUIAvatar <monoAvatar>__10;
            internal MonoGameEntry <monoGameEntry>__9;
            internal bool <needSetTriggerFlag>__8;
            internal List<int> <removeKeys>__0;
            internal List<Avatar3dModelDataItem> avatarDataList;

            internal bool <>m__1AC(Avatar3dModelDataItem x)
            {
                return (x.avatar.avatarID == this.<avatarId>__2);
            }

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.<removeKeys>__0 = new List<int>();
                        this.<$s_1828>__1 = this.<>f__this._avatarModelDict.Keys.GetEnumerator();
                        try
                        {
                            while (this.<$s_1828>__1.MoveNext())
                            {
                                this.<avatarId>__2 = this.<$s_1828>__1.Current;
                                if (this.avatarDataList.Find(new Predicate<Avatar3dModelDataItem>(this.<>m__1AC)) == null)
                                {
                                    this.<removeKeys>__0.Add(this.<avatarId>__2);
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1828>__1.Dispose();
                        }
                        this.<$s_1829>__3 = this.<removeKeys>__0.GetEnumerator();
                        try
                        {
                            while (this.<$s_1829>__3.MoveNext())
                            {
                                this.<key>__4 = this.<$s_1829>__3.Current;
                                if (this.<>f__this._avatarModelDict[this.<key>__4] != null)
                                {
                                    UnityEngine.Object.Destroy(this.<>f__this._avatarModelDict[this.<key>__4].gameObject);
                                }
                                this.<>f__this._avatarModelDict.Remove(this.<key>__4);
                                this.<>f__this._avatarDefaultYDict.Remove(this.<key>__4);
                            }
                        }
                        finally
                        {
                            this.<$s_1829>__3.Dispose();
                        }
                        this.<$s_1830>__5 = this.avatarDataList.GetEnumerator();
                        try
                        {
                            while (this.<$s_1830>__5.MoveNext())
                            {
                                this.<data>__6 = this.<$s_1830>__5.Current;
                                this.<modelTrans>__7 = null;
                                this.<needSetTriggerFlag>__8 = true;
                                this.<monoGameEntry>__9 = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                                if (this.<>f__this._avatarModelDict.ContainsKey(this.<data>__6.avatar.avatarID))
                                {
                                    this.<modelTrans>__7 = this.<>f__this._avatarModelDict[this.<data>__6.avatar.avatarID];
                                    this.<needSetTriggerFlag>__8 = false;
                                }
                                else
                                {
                                    this.<monoAvatar>__10 = this.<>f__this.view.GetComponentInChildren<BaseMonoUIAvatar>();
                                    this.<loadFlag>__11 = true;
                                    if (this.<monoAvatar>__10 != null)
                                    {
                                        if (this.<monoAvatar>__10.avatarID == this.<data>__6.avatar.avatarID)
                                        {
                                            this.<modelTrans>__7 = this.<monoAvatar>__10.gameObject.transform;
                                            this.<>f__this._setAvatarPosFlag = false;
                                            this.<loadFlag>__11 = false;
                                        }
                                        else
                                        {
                                            UnityEngine.Object.DestroyImmediate(this.<monoAvatar>__10.gameObject);
                                            this.<>f__this._setAvatarPosFlag = true;
                                        }
                                    }
                                    if (this.<loadFlag>__11)
                                    {
                                        this.<modelTrans>__7 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(this.<>f__this.GetAvatarPrefaPath(this.<data>__6.avatar.AvatarRegistryKey), BundleType.RESOURCE_FILE)).transform;
                                        this.<modelTrans>__7.SetParent(this.<>f__this.view.transform);
                                        this.<monoAvatar>__10 = this.<modelTrans>__7.GetComponent<BaseMonoUIAvatar>();
                                        if (this.<monoGameEntry>__9 != null)
                                        {
                                            UnityEngine.Object.DontDestroyOnLoad(this.<>f__this.view);
                                        }
                                    }
                                    this.<monoAvatar>__10.avatarData = this.<data>__6.avatar;
                                    this.<monoAvatar>__10.tattooVisible = false;
                                    this.<monoAvatar>__10.SetTattooVisible(0);
                                    this.<monoAvatar>__10.Init(this.<data>__6.avatar.avatarID);
                                    this.<>f__this._avatarModelDict.Add(this.<data>__6.avatar.avatarID, this.<modelTrans>__7);
                                    this.<>f__this._avatarDefaultYDict.Add(this.<data>__6.avatar.avatarID, this.<modelTrans>__7.localPosition.y);
                                    if (this.<monoGameEntry>__9 == null)
                                    {
                                        this.<modelTrans>__7.GetComponent<BaseMonoUIAvatar>().standOnSpaceshipInGameEntry = false;
                                    }
                                }
                                this.<>f__this.SetAvatarModelView(this.<modelTrans>__7, this.<data>__6.avatar, this.<data>__6.pos, this.<data>__6.eulerAngles, this.<data>__6.showLockViewIfLock);
                                if (this.<needSetTriggerFlag>__8)
                                {
                                    if (this.<monoGameEntry>__9 != null)
                                    {
                                        this.<modelTrans>__7.GetComponent<Animator>().SetTrigger("TriggerStandByBack");
                                    }
                                    else
                                    {
                                        this.<modelTrans>__7.GetComponent<Animator>().SetTrigger("TriggerStandBy");
                                    }
                                    if ((this.<monoGameEntry>__9 == null) && !Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim)
                                    {
                                        this.<modelTrans>__7.GetComponent<Animator>().SetTrigger("TriggerTurnAround");
                                        Singleton<PlayerModule>.Instance.playerData.uiTempSaveData.hasShowAvatarTurnAroundAnim = true;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            this.<$s_1830>__5.Dispose();
                        }
                        Singleton<NotifyManager>.Instance.FireNotify(new Notify(NotifyTypes.FinishCreateAvatarUIModels, null));
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.$PC = -1;
                        break;
                }
                return false;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

