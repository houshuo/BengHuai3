namespace MoleMole
{
    using MoleMole.Config;
    using proto;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Events;

    public abstract class BaseMonoUIAvatar : MonoBehaviour, IWeaponAttacher, IBodyPartTouchable
    {
        private Animator _animator;
        private GameObject _buffEffect;
        private Vector3 _cameraOrigionPostion;
        private float _camShakeTimer = -1f;
        private float _fadeSpeed;
        private bool _fading;
        private RuntimeAnimatorController _galtouchAnimController;
        private bool _galTouchInited;
        private GameObject _goodFeelPrototype;
        private float _idleEffectTimer;
        private bool _isAppliedLightColor;
        private bool _isInGalTouch;
        private bool _isShadowColorAdjusted;
        private List<ColorAdjuster> _mainColorAdjusterList;
        private GameObject _ordinaryFeelPrototype;
        private RuntimeAnimatorController _originAnimatorController;
        private Vector3 _originPos;
        private AtlasMatInfoProvider _providerL;
        private AtlasMatInfoProvider _providerM;
        private AtlasMatInfoProvider _providerR;
        private bool _readyToRestart;
        private List<ColorAdjuster> _shadowColorAdjusterList;
        private int _weaponMetaID;
        [HideInInspector]
        public AttachPoint[] attachPoints = new AttachPoint[0];
        public AvatarDataItem avatarData;
        public int avatarID;
        private const string BUFF_EFFECT_FOLDER = "Effect/GalTouchBuff/";
        public float effectYOffset;
        public string galTouchControllerPath;
        public GameObject[] galTouchHideObjects;
        private GalTouchSystem galTouchSystem;
        private const string GOOD_FEEL_PATH = "UI/Menus/Widget/Storage/GoodFeeling";
        public Transform headRoot;
        private const float IDLE_EFFECT_INTERVAL = 15f;
        public Renderer leftEyeRenderer;
        public Renderer mouthRenderer;
        private const string ORDINARY_FEEL_PATH = "UI/Menus/Widget/Storage/OrdinaryFeeling";
        public Renderer[] renderers;
        public Renderer rightEyeRenderer;
        private const string SILENT_EFFECT_NAME = "EmotionSilent";
        public GameObject[] switchObjects;
        public bool tattooVisible;

        protected BaseMonoUIAvatar()
        {
        }

        private void AdjustShadowColors()
        {
            Camera main = Camera.main;
            if (main != null)
            {
                PostFXBase component = main.GetComponent<PostFXBase>();
                if (component != null)
                {
                    float avatarShadowAdjust = component.AvatarShadowAdjust;
                    foreach (ColorAdjuster adjuster in this._shadowColorAdjusterList)
                    {
                        adjuster.ApplyLerp(avatarShadowAdjust);
                    }
                    this._isShadowColorAdjusted = true;
                }
            }
        }

        private void ApplyLightColor()
        {
            Light light = UnityEngine.Object.FindObjectOfType<Light>();
            if (light != null)
            {
                Color tintColor = (Color) (light.color * light.intensity);
                foreach (ColorAdjuster adjuster in this._mainColorAdjusterList)
                {
                    adjuster.ApplyMultiply(tintColor);
                }
                this._isAppliedLightColor = true;
            }
        }

        private void AttachBuffEffect(string name)
        {
            GameObject original = Resources.Load<GameObject>("Effect/GalTouchBuff/" + name);
            if (original != null)
            {
                if (this._buffEffect != null)
                {
                    UnityEngine.Object.Destroy(this._buffEffect);
                }
                this._buffEffect = UnityEngine.Object.Instantiate<GameObject>(original);
                if (this._buffEffect != null)
                {
                    this._buffEffect.transform.SetParent(base.transform, false);
                }
            }
        }

        public void AttachWeapon(int weaponID, string avatarType)
        {
            if (this._weaponMetaID != weaponID)
            {
                this._weaponMetaID = weaponID;
                ConfigWeapon weaponConfig = WeaponData.GetWeaponConfig(weaponID);
                Transform weaponProtoTrans = Miscs.LoadResource<GameObject>(weaponConfig.Attach.PrefabPath, BundleType.RESOURCE_FILE).transform;
                WeaponAttach.AttachWeaponMesh(weaponConfig, this, weaponProtoTrans, avatarType);
                int layer = 8;
                base.gameObject.SetLayer(layer, true);
            }
        }

        public void BodyPartTouched(BodyPartType type, Vector3 point)
        {
            if (!Singleton<NetworkManager>.Instance.DispatchSeverData.isReview)
            {
                BasePageContext currentPageContext = Singleton<MainUIManager>.Instance.CurrentPageContext;
                if ((currentPageContext == null) || (currentPageContext.dialogContextList.Count <= 0))
                {
                    int heartLevel = this.galTouchSystem.heartLevel;
                    if (this.galTouchSystem.BodyPartTouched(type))
                    {
                        GameObject original = null;
                        if (heartLevel >= 4)
                        {
                            original = this._goodFeelPrototype;
                        }
                        else
                        {
                            original = this._ordinaryFeelPrototype;
                        }
                        if (original != null)
                        {
                            GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                            if (obj3 != null)
                            {
                                obj3.transform.position = point;
                            }
                        }
                        this._idleEffectTimer = 15f;
                        GameObject obj4 = GameObject.Find("EmotionSilent");
                        if (obj4 != null)
                        {
                            UnityEngine.Object.Destroy(obj4);
                        }
                    }
                }
            }
        }

        public void CameraShake(float time)
        {
            this._cameraOrigionPostion = Camera.main.transform.localPosition;
            this._camShakeTimer = time;
        }

        public void ChangeStigmata(StigmataDataItem from, StigmataDataItem to, EquipmentSlot slot)
        {
            base.StartCoroutine(this.CorrutineChangeStigmata(from, to, slot));
        }

        [DebuggerHidden]
        private IEnumerator CorrutineChangeStigmata(StigmataDataItem from, StigmataDataItem to, EquipmentSlot slot)
        {
            return new <CorrutineChangeStigmata>c__Iterator1D { slot = slot, from = from, to = to, <$>slot = slot, <$>from = from, <$>to = to, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator CorrutineStigmataFadeIn(EquipmentSlot slot)
        {
            return new <CorrutineStigmataFadeIn>c__Iterator1E { slot = slot, <$>slot = slot, <>f__this = this };
        }

        [DebuggerHidden]
        private IEnumerator CorrutineStigmataFadeOut(EquipmentSlot slot)
        {
            return new <CorrutineStigmataFadeOut>c__Iterator1F { slot = slot, <$>slot = slot, <>f__this = this };
        }

        private void DetachBuffEffect()
        {
            if (this._buffEffect != null)
            {
                UnityEngine.Object.Destroy(this._buffEffect);
                this._buffEffect = null;
            }
        }

        public void EnterGalTouch()
        {
            if (!this._galTouchInited)
            {
                this.InitGalTouch();
            }
            if (!this._isInGalTouch)
            {
                if (this.galTouchSystem != null)
                {
                    this.galTouchSystem.enable = true;
                    if (this._animator != null)
                    {
                        this._galtouchAnimController = this.GetRuntimeAnimatorControllerByID();
                        this._animator.runtimeAnimatorController = this._galtouchAnimController;
                    }
                    int characterHeartLevel = Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel();
                    if (Singleton<GalTouchModule>.Instance != null)
                    {
                        Singleton<GalTouchModule>.Instance.GalTouchInfoChanged += new GalTouchModule.GalTouchInfoChangedHandler(this.OnGalTouchInfoChanged);
                        Singleton<GalTouchModule>.Instance.GalAddBuff += new Action<int, int>(this.OnGalTouchAddBuff);
                    }
                    this.galTouchSystem.heartLevel = (characterHeartLevel <= 4) ? characterHeartLevel : 4;
                }
                int index = 0;
                int length = this.galTouchHideObjects.Length;
                while (index < length)
                {
                    this.galTouchHideObjects[index].SetActive(false);
                    index++;
                }
                int avatarGalTouchBuffId = Singleton<GalTouchModule>.Instance.GetAvatarGalTouchBuffId(this.avatarID);
                this.ProcessBuffEffectOfBuffId(avatarGalTouchBuffId);
                this._idleEffectTimer = 15f;
                this._isInGalTouch = true;
            }
        }

        public void ExitGalTouch()
        {
            if (this._readyToRestart)
            {
                GeneralLogicManager.RestartGame();
            }
            if (this._isInGalTouch)
            {
                if (this.galTouchSystem != null)
                {
                    this.galTouchSystem.enable = false;
                    this.galTouchSystem.StopFaceAnimation();
                    this.galTouchSystem.StopVoice();
                    if (this._animator != null)
                    {
                        this._animator.runtimeAnimatorController = this._originAnimatorController;
                        Resources.UnloadAsset(this._galtouchAnimController);
                        this._galtouchAnimController = null;
                    }
                    if (Singleton<GalTouchModule>.Instance != null)
                    {
                        Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= new GalTouchModule.GalTouchInfoChangedHandler(this.OnGalTouchInfoChanged);
                        Singleton<GalTouchModule>.Instance.GalAddBuff -= new Action<int, int>(this.OnGalTouchAddBuff);
                    }
                }
                int index = 0;
                int length = this.galTouchHideObjects.Length;
                while (index < length)
                {
                    if (this.galTouchHideObjects[index] != null)
                    {
                        this.galTouchHideObjects[index].SetActive(true);
                    }
                    index++;
                }
                this.DetachBuffEffect();
                this._isInGalTouch = false;
            }
        }

        public void FadeBlack(float speed = 60f)
        {
            int index = 0;
            int allCamerasCount = Camera.allCamerasCount;
            while (index < allCamerasCount)
            {
                Camera camera = Camera.allCameras[index];
                if (camera != Camera.main)
                {
                    camera.enabled = false;
                }
                this._fading = true;
                this._fadeSpeed = speed;
                index++;
            }
        }

        public void GalTouchEffect(string name)
        {
            char[] separator = new char[] { ',' };
            string[] strArray = name.Split(separator);
            string str = strArray[0];
            string s = (strArray.Length < 2) ? null : strArray[1];
            string str3 = (strArray.Length < 3) ? null : strArray[2];
            GameObject original = Resources.Load<GameObject>("UI/Menus/Widget/Storage/" + str);
            if (original != null)
            {
                GameObject obj3 = UnityEngine.Object.Instantiate<GameObject>(original);
                float result = 0f;
                if (s != null)
                {
                    float.TryParse(s, out result);
                }
                float num2 = 0f;
                if (str3 != null)
                {
                    float.TryParse(str3, out num2);
                }
                obj3.transform.position += new Vector3(result, num2 + this.effectYOffset, 0f);
                obj3.name = name;
            }
        }

        private void GalTouchRollBuff(int partIndex)
        {
            TouchLevelItem touchLevelItem = GalTouchData.GetTouchLevelItem(this.galTouchSystem.heartLevel);
            if (touchLevelItem != null)
            {
                float prop = touchLevelItem.prop;
                if (UnityEngine.Random.value < prop)
                {
                    int[] numArray = GalTouchData.QueryTouchBuff(this.avatarID, partIndex, this.galTouchSystem.heartLevel);
                    if (numArray != null)
                    {
                        int index = (int) (UnityEngine.Random.value * numArray.Length);
                        Singleton<GalTouchModule>.Instance.AddBuff(this.avatarID, numArray[index]);
                    }
                }
            }
        }

        public Transform GetAttachPoint(string name)
        {
            for (int i = 0; i < this.attachPoints.Length; i++)
            {
                if (this.attachPoints[i].name == name)
                {
                    return this.attachPoints[i].pointTransform;
                }
            }
            return base.transform;
        }

        private RuntimeAnimatorController GetRuntimeAnimatorControllerByID()
        {
            string str = "Entities/Avatar/";
            string galTouchControllerPath = this.galTouchControllerPath;
            return Resources.Load<RuntimeAnimatorController>(str + galTouchControllerPath);
        }

        public bool HasAttachPoint(string name)
        {
            for (int i = 0; i < this.attachPoints.Length; i++)
            {
                if (this.attachPoints[i].name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual void Init(int avatarID)
        {
            this.avatarID = avatarID;
            this.InitMaterials();
            this.InitDynamicBone();
            this.UploadFaceTexture();
        }

        private void InitColorAdjusterList()
        {
            bool flag = false;
            bool flag2 = false;
            if (this._shadowColorAdjusterList == null)
            {
                flag = true;
                this._shadowColorAdjusterList = new List<ColorAdjuster>();
            }
            if (this._mainColorAdjusterList == null)
            {
                flag2 = true;
                this._mainColorAdjusterList = new List<ColorAdjuster>();
            }
            if (flag || flag2)
            {
                string[] colorNames = new string[] { "_FirstShadowMultColor", "_SecondShadowMultColor" };
                string[] strArray2 = new string[] { "_Color" };
                foreach (Renderer renderer in this.renderers)
                {
                    foreach (Material material in renderer.materials)
                    {
                        if (flag && material.HasProperty("_FirstShadowMultColor"))
                        {
                            this._shadowColorAdjusterList.Add(new ColorAdjuster(material, colorNames));
                        }
                        if (flag2 && material.HasProperty("_Color"))
                        {
                            this._mainColorAdjusterList.Add(new ColorAdjuster(material, strArray2));
                        }
                    }
                }
            }
        }

        private void InitDynamicBone()
        {
            bool flag = GlobalVars.UI_AVATAR_USE_DYNAMIC_BONE;
            foreach (DynamicBone bone in base.gameObject.GetComponentsInChildren<DynamicBone>())
            {
                bone.enabled = flag;
            }
        }

        private void InitGalTouch()
        {
            if (((this.leftEyeRenderer != null) && (this.rightEyeRenderer != null)) && ((this.mouthRenderer != null) && (this._originAnimatorController == null)))
            {
                this._animator = base.GetComponent<Animator>();
                this.galTouchSystem = new GalTouchSystem();
                string str = null;
                if ((this.avatarID / 100) == 1)
                {
                    str = "Kiana";
                }
                else if ((this.avatarID / 100) == 2)
                {
                    str = "Mei";
                }
                else if ((this.avatarID / 100) == 3)
                {
                    str = "Bronya";
                }
                string path = "FaceAtlas/" + str + "/Eye/Atlas";
                string str3 = "FaceAtlas/" + str + "/Mouth/Atlas";
                if (this._providerL == null)
                {
                    this._providerL = Resources.Load<AtlasMatInfoProvider>(path);
                    this._providerL.RetainReference();
                }
                if (this._providerR == null)
                {
                    this._providerR = Resources.Load<AtlasMatInfoProvider>(path);
                    this._providerR.RetainReference();
                }
                if (this._providerM == null)
                {
                    this._providerM = Resources.Load<AtlasMatInfoProvider>(str3);
                    this._providerM.RetainReference();
                }
                this.galTouchSystem.Init(base.GetComponent<Animator>(), this.avatarData.avatarID, 1, this.leftEyeRenderer, this.rightEyeRenderer, this.mouthRenderer, this._providerL, this._providerR, this._providerM, this.headRoot);
                this._originAnimatorController = base.GetComponent<Animator>().runtimeAnimatorController;
                MonoBodyPart[] componentsInChildren = base.gameObject.GetComponentsInChildren<MonoBodyPart>();
                int index = 0;
                int length = componentsInChildren.Length;
                while (index < length)
                {
                    componentsInChildren[index].SetBodyPartTouchable(this);
                    index++;
                }
                int num3 = 0;
                int num4 = this.switchObjects.Length;
                while (num3 < num4)
                {
                    this.switchObjects[num3].SetActive(false);
                    num3++;
                }
                this.galTouchSystem.IdleChanged += new UnityAction<bool>(this.OnGalTouchSystemIdleChanged);
                this.galTouchSystem.TouchPatternTriggered += new UnityAction<int>(this.OnTouchPatternTriggered);
                this._ordinaryFeelPrototype = Resources.Load<GameObject>("UI/Menus/Widget/Storage/OrdinaryFeeling");
                this._goodFeelPrototype = Resources.Load<GameObject>("UI/Menus/Widget/Storage/GoodFeeling");
                this._galTouchInited = true;
            }
        }

        private void InitMaterials()
        {
            this.InitColorAdjusterList();
            this.AdjustShadowColors();
            this.ApplyLightColor();
        }

        GameObject IWeaponAttacher.get_gameObject()
        {
            return base.gameObject;
        }

        protected void OnDestroy()
        {
            if (this._isInGalTouch && (Singleton<GalTouchModule>.Instance != null))
            {
                Singleton<GalTouchModule>.Instance.GalTouchInfoChanged -= new GalTouchModule.GalTouchInfoChangedHandler(this.OnGalTouchInfoChanged);
                Singleton<GalTouchModule>.Instance.GalAddBuff -= new Action<int, int>(this.OnGalTouchAddBuff);
            }
            if (this._providerL != null)
            {
                if (this._providerL.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerL);
                }
                this._providerL = null;
            }
            if (this._providerR != null)
            {
                if (this._providerR.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerR);
                }
                this._providerR = null;
            }
            if (this._providerM != null)
            {
                if (this._providerM.ReleaseReference())
                {
                    Resources.UnloadAsset(this._providerM);
                }
                this._providerM = null;
            }
        }

        private void OnDisable()
        {
            if (Application.isEditor)
            {
                this.RestoreLightColor();
                this.RestoreShadowColor();
                this._mainColorAdjusterList = null;
                this._shadowColorAdjusterList = null;
            }
        }

        private void OnEnable()
        {
            if (Application.isEditor)
            {
                this.InitMaterials();
            }
        }

        public void OnGalTouchAddBuff(int avatarId, int buffId)
        {
            if (avatarId == this.avatarID)
            {
                this.ProcessBuffEffectOfBuffId(buffId);
            }
        }

        public void OnGalTouchInfoChanged(int oldGoodFeel, int oldHeartLevel, int newGoodFeel, int newHeartLevel, GoodFeelLimitType limitType)
        {
            if (oldHeartLevel != newHeartLevel)
            {
                this.galTouchSystem.heartLevel = (newHeartLevel <= 4) ? newHeartLevel : 4;
            }
        }

        private void OnGalTouchSystemIdleChanged(bool idle)
        {
            if (idle)
            {
                int index = 0;
                int length = this.switchObjects.Length;
                while (index < length)
                {
                    this.switchObjects[index].SetActive(false);
                    index++;
                }
                int num3 = 0;
                int num4 = this.galTouchHideObjects.Length;
                while (num3 < num4)
                {
                    this.galTouchHideObjects[num3].SetActive(false);
                    num3++;
                }
            }
        }

        private void OnTouchPatternTriggered(int partIndex)
        {
            int amount = GalTouchData.QueryTouchFeel(this.avatarID, partIndex, Singleton<GalTouchModule>.Instance.GetCharacterHeartLevel());
            this.GalTouchRollBuff(partIndex);
            Singleton<GalTouchModule>.Instance.IncreaseTouchGoodFeel(amount);
        }

        private void ProcessBuffEffectOfBuffId(int buffId)
        {
            if (buffId != 0)
            {
                TouchBuffItem touchBuffItem = GalTouchData.GetTouchBuffItem(buffId);
                if (touchBuffItem != null)
                {
                    this.AttachBuffEffect(touchBuffItem.effect);
                }
            }
            else
            {
                this.DetachBuffEffect();
            }
        }

        public void RestartGame()
        {
            this._readyToRestart = true;
        }

        private void RestoreLightColor()
        {
            foreach (ColorAdjuster adjuster in this._mainColorAdjusterList)
            {
                adjuster.Restore();
            }
        }

        private void RestoreShadowColor()
        {
            foreach (ColorAdjuster adjuster in this._shadowColorAdjusterList)
            {
                adjuster.Restore();
            }
        }

        public void SetOriginPos(Vector3 originPos)
        {
            this._originPos = originPos;
        }

        public void SetTattooVisible(int visible)
        {
            GameObject gameObject = this.GetAttachPoint("Stigmata").gameObject;
            if (visible == 0)
            {
                gameObject.SetActive(false);
            }
            else if (this.tattooVisible)
            {
                this.GetAttachPoint("Stigmata").gameObject.SetActive(true);
                foreach (KeyValuePair<EquipmentSlot, StigmataDataItem> pair in this.avatarData.GetStigmataDict())
                {
                    Transform attachPoint = this.GetAttachPoint(pair.Key.ToString());
                    if (attachPoint != null)
                    {
                        attachPoint.gameObject.SetActive(pair.Value != null);
                        if (pair.Value != null)
                        {
                            this.StigmataFadeIn(pair.Key);
                        }
                    }
                }
            }
        }

        public void ShowHiden(int show)
        {
            int index = 0;
            int length = this.galTouchHideObjects.Length;
            while (index < length)
            {
                this.galTouchHideObjects[index].SetActive(show != 0);
                index++;
            }
        }

        public void StigmataFadeIn(EquipmentSlot slot)
        {
            base.StartCoroutine(this.CorrutineStigmataFadeIn(slot));
        }

        public void StigmataFadeOut(EquipmentSlot slot)
        {
            base.StartCoroutine(this.CorrutineStigmataFadeOut(slot));
        }

        public void SwitchOff(string name)
        {
            int index = 0;
            int length = this.switchObjects.Length;
            while (index < length)
            {
                if (this.switchObjects[index].name == name)
                {
                    this.switchObjects[index].SetActive(false);
                    return;
                }
                index++;
            }
        }

        public void SwitchOn(string name)
        {
            int index = 0;
            int length = this.switchObjects.Length;
            while (index < length)
            {
                if (this.switchObjects[index].name == name)
                {
                    this.switchObjects[index].SetActive(true);
                    return;
                }
                index++;
            }
        }

        public void TriggerAudioPattern(string name)
        {
            Singleton<WwiseAudioManager>.Instance.Post(name, base.gameObject, null, null);
        }

        protected virtual void Update()
        {
            if (!this._isShadowColorAdjusted || Application.isEditor)
            {
                this.AdjustShadowColors();
            }
            if (!this._isAppliedLightColor || Application.isEditor)
            {
                this.ApplyLightColor();
            }
            this.UpdateStandOnSpaceshipInGameEntry();
            this.UpdateGalTouchSystem();
        }

        public void UpdateGalTouchSystem()
        {
            if (this.galTouchSystem != null)
            {
                this.galTouchSystem.Process(Time.deltaTime);
                if (this.galTouchSystem.enable)
                {
                    this._idleEffectTimer -= Time.deltaTime;
                    if (this._idleEffectTimer <= 0f)
                    {
                        this._idleEffectTimer += 15f;
                        this.GalTouchEffect("EmotionSilent");
                    }
                }
                if (!this.galTouchSystem.idle)
                {
                    this._idleEffectTimer = 15f;
                }
            }
            if (this._readyToRestart && this.galTouchSystem.idle)
            {
                GeneralLogicManager.RestartGame();
            }
            if (this._fading)
            {
                PostFX component = Camera.main.gameObject.GetComponent<PostFX>();
                component.Exposure -= this._fadeSpeed * Time.deltaTime;
                component.Exposure = (component.Exposure < 0f) ? 0f : component.Exposure;
            }
            if (this._camShakeTimer > 0f)
            {
                this._camShakeTimer -= Time.deltaTime;
                if (this._camShakeTimer > 0f)
                {
                    Vector3 vector = new Vector3(0f, Mathf.Sin(this._camShakeTimer * 900f) * 0.2f, 0f);
                    Camera.main.transform.localPosition = this._cameraOrigionPostion + vector;
                }
                else
                {
                    this._camShakeTimer = -1f;
                    Camera.main.transform.localPosition = this._cameraOrigionPostion;
                }
            }
            BasePageContext context = !(Singleton<MainUIManager>.Instance.SceneCanvas is MonoMainCanvas) ? null : Singleton<MainUIManager>.Instance.CurrentPageContext;
            if ((context != null) && (context.dialogContextList.Count > 0))
            {
                this._idleEffectTimer = 15f;
            }
        }

        private void UpdateStandOnSpaceshipInGameEntry()
        {
            if (this.standOnSpaceshipInGameEntry && (((Singleton<MainUIManager>.Instance != null) && (Singleton<MainUIManager>.Instance.SceneCanvas != null)) && (Singleton<MainUIManager>.Instance.SceneCanvas is MonoGameEntry)))
            {
                float y = this._originPos.y;
                MonoGameEntry sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas as MonoGameEntry;
                Transform transform = sceneCanvas.spaceshipGO.transform.Find("Warship");
                base.transform.position = new Vector3(this._originPos.x, (y + transform.position.y) - sceneCanvas.warshipDefaultYPos, this._originPos.z);
            }
        }

        private void UploadFaceTexture()
        {
            if (this.leftEyeRenderer != null)
            {
                this.leftEyeRenderer.sharedMaterial.mainTexture = this.leftEyeRenderer.sharedMaterial.mainTexture;
            }
            if (this.rightEyeRenderer != null)
            {
                this.rightEyeRenderer.sharedMaterial.mainTexture = this.rightEyeRenderer.sharedMaterial.mainTexture;
            }
            if (this.mouthRenderer != null)
            {
                this.mouthRenderer.sharedMaterial.mainTexture = this.mouthRenderer.sharedMaterial.mainTexture;
            }
        }

        public bool standOnSpaceshipInGameEntry { get; set; }

        public int WeaponMetaID
        {
            get
            {
                return this._weaponMetaID;
            }
        }

        [CompilerGenerated]
        private sealed class <CorrutineChangeStigmata>c__Iterator1D : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal StigmataDataItem <$>from;
            internal EquipmentSlot <$>slot;
            internal StigmataDataItem <$>to;
            internal BaseMonoUIAvatar <>f__this;
            internal float <emissionFactor>__4;
            internal float <emissionFactorOrigin>__3;
            internal Material <material>__2;
            internal float <speed>__0;
            internal Transform <stigmataTrans>__1;
            internal StigmataDataItem from;
            internal EquipmentSlot slot;
            internal StigmataDataItem to;

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
                        this.<speed>__0 = 1f;
                        this.<stigmataTrans>__1 = this.<>f__this.GetAttachPoint(this.slot.ToString());
                        this.<stigmataTrans>__1.gameObject.SetActive(true);
                        this.<material>__2 = this.<stigmataTrans>__1.GetComponent<MeshRenderer>().material;
                        this.<emissionFactorOrigin>__3 = this.<material>__2.GetFloat("_EmissionFactor");
                        this.<emissionFactor>__4 = this.<emissionFactorOrigin>__3;
                        if (this.from == null)
                        {
                            goto Label_00FA;
                        }
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_017D;

                    case 3:
                        goto Label_01CE;

                    default:
                        goto Label_01FB;
                }
                if (this.<emissionFactor>__4 < 14f)
                {
                    this.<emissionFactor>__4 += this.<speed>__0;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_01FD;
                }
            Label_00FA:
                if (this.to == null)
                {
                    this.<stigmataTrans>__1.gameObject.SetActive(false);
                    goto Label_01F4;
                }
                this.<material>__2.SetTexture("_MainTex", Miscs.LoadResource<Texture>(this.to.GetTattooPath(), BundleType.RESOURCE_FILE));
            Label_017D:
                while (this.<emissionFactor>__4 < 20f)
                {
                    this.<emissionFactor>__4 += this.<speed>__0;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_01FD;
                }
            Label_01CE:
                while (this.<emissionFactor>__4 > 1.5f)
                {
                    this.<emissionFactor>__4 -= this.<speed>__0;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                    this.$current = null;
                    this.$PC = 3;
                    goto Label_01FD;
                }
                this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
            Label_01F4:
                this.$PC = -1;
            Label_01FB:
                return false;
            Label_01FD:
                return true;
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

        [CompilerGenerated]
        private sealed class <CorrutineStigmataFadeIn>c__Iterator1E : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal EquipmentSlot <$>slot;
            internal BaseMonoUIAvatar <>f__this;
            internal float <emissionFactor>__4;
            internal float <emissionFactorOrigin>__3;
            internal Material <material>__2;
            internal float <speed>__0;
            internal Transform <stigmataTrans>__1;
            internal EquipmentSlot slot;

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
                        this.<speed>__0 = 1f;
                        this.<stigmataTrans>__1 = this.<>f__this.GetAttachPoint(this.slot.ToString());
                        this.<stigmataTrans>__1.gameObject.SetActive(true);
                        this.<material>__2 = this.<stigmataTrans>__1.GetComponent<MeshRenderer>().material;
                        this.<emissionFactorOrigin>__3 = this.<material>__2.GetFloat("_EmissionFactor");
                        this.<emissionFactor>__4 = this.<emissionFactorOrigin>__3;
                        break;

                    case 1:
                        break;

                    case 2:
                        goto Label_012C;

                    default:
                        goto Label_0159;
                }
                if (this.<emissionFactor>__4 < 20f)
                {
                    this.<emissionFactor>__4 += this.<speed>__0;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                    this.$current = null;
                    this.$PC = 1;
                    goto Label_015B;
                }
            Label_012C:
                while (this.<emissionFactor>__4 > 1.5f)
                {
                    this.<emissionFactor>__4 -= this.<speed>__0;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                    this.$current = null;
                    this.$PC = 2;
                    goto Label_015B;
                }
                this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__4);
                this.$PC = -1;
            Label_0159:
                return false;
            Label_015B:
                return true;
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

        [CompilerGenerated]
        private sealed class <CorrutineStigmataFadeOut>c__Iterator1F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal EquipmentSlot <$>slot;
            internal BaseMonoUIAvatar <>f__this;
            internal float <bloomFactor>__5;
            internal float <bloomFactorOriginal>__6;
            internal float <emissionFactor>__3;
            internal float <emissionFactorOriginal>__4;
            internal Material <material>__2;
            internal float <speed>__0;
            internal float <speedRatio>__7;
            internal Transform <stigmataTrans>__1;
            internal EquipmentSlot slot;

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
                        this.<speed>__0 = 1f;
                        this.<stigmataTrans>__1 = this.<>f__this.GetAttachPoint(this.slot.ToString());
                        this.<stigmataTrans>__1.gameObject.SetActive(true);
                        this.<material>__2 = this.<stigmataTrans>__1.GetComponent<MeshRenderer>().material;
                        this.<emissionFactor>__3 = this.<material>__2.GetFloat("_EmissionFactor");
                        this.<emissionFactorOriginal>__4 = this.<emissionFactor>__3;
                        this.<bloomFactor>__5 = this.<material>__2.GetFloat("_BloomFactor");
                        this.<bloomFactorOriginal>__6 = this.<bloomFactor>__5;
                        this.<speedRatio>__7 = 0.3f;
                        break;

                    case 1:
                        break;

                    default:
                        goto Label_018C;
                }
                if (this.<emissionFactor>__3 < 10f)
                {
                    this.<speedRatio>__7 += 0.001f;
                    if (this.<speedRatio>__7 > 5f)
                    {
                        this.<speedRatio>__7 = 5f;
                    }
                    this.<emissionFactor>__3 += this.<speed>__0 * this.<speedRatio>__7;
                    this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactor>__3);
                    this.$current = null;
                    this.$PC = 1;
                    return true;
                }
                this.<material>__2.SetFloat("_EmissionFactor", this.<emissionFactorOriginal>__4);
                this.<material>__2.SetFloat("_BloomFactor", this.<bloomFactorOriginal>__6);
                this.<stigmataTrans>__1.gameObject.SetActive(false);
                this.$PC = -1;
            Label_018C:
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

        private class ColorAdjuster
        {
            private Material _material;
            private string[] colorNames;
            private Color[] colors;

            public ColorAdjuster(Material material, string[] colorNames)
            {
                this._material = material;
                this.colorNames = colorNames;
                this.colors = new Color[this.colorNames.Length];
                for (int i = 0; i < this.colorNames.Length; i++)
                {
                    this.colors[i] = this._material.GetColor(colorNames[i]);
                }
            }

            public void ApplyLerp(float factor)
            {
                for (int i = 0; i < this.colors.Length; i++)
                {
                    this._material.SetColor(this.colorNames[i], Color.Lerp(this.colors[i], Color.white, factor));
                }
            }

            public void ApplyMultiply(Color tintColor)
            {
                for (int i = 0; i < this.colors.Length; i++)
                {
                    this._material.SetColor(this.colorNames[i], this.colors[i] * tintColor);
                }
            }

            public void Restore()
            {
                for (int i = 0; i < this.colors.Length; i++)
                {
                    this._material.SetColor(this.colorNames[i], this.colors[i]);
                }
            }
        }
    }
}

