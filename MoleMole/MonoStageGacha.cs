namespace MoleMole
{
    using MoleMole.MainMenu;
    using proto;
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine;

    public class MonoStageGacha : MonoBehaviour
    {
        private GachaMainPageContext.GachaAmountType _amountType;
        private bool _audioEventPosted;
        private GameObject _box;
        [SerializeField]
        private GameObject _boxFPBigPrefab;
        [SerializeField]
        private GameObject _boxFPPrefab;
        [SerializeField]
        private GameObject _boxHCBigPrefab;
        [SerializeField]
        private GameObject _boxHCPrefab;
        [SerializeField]
        private float _boxScale = 1f;
        [SerializeField]
        private GameObject _boxSPBigPrefab;
        [SerializeField]
        private GameObject _boxSPPrefab;
        private bool _bUIFadeEffect;
        [SerializeField]
        private float _duration = 3.5f;
        private Action _endCallBack;
        [SerializeField]
        private float _FadeDuration;
        [SerializeField]
        private float _FadeStart;
        private Animator _gachaCameraAnimator;
        private GameObject _gachaEffect;
        private MainMenuStage _mainMenuStage;
        [SerializeField]
        private Material _matAvatar;
        [SerializeField]
        private Material _matEquipment;
        [SerializeField]
        private Material _matStageFP;
        [SerializeField]
        private Material _matStageHC;
        [SerializeField]
        private Material _matStageSP;
        private GameObject _spaceShip;
        [SerializeField]
        private float _tenBoxScale = 1f;
        private float _time;
        private GachaType _type;
        private GameObject _uiCamera;
        [SerializeField]
        private GameObject _uiFadeEffect;
        private GameObject _warShip;
        private static readonly string ATMOSPHERE_PATH = "Rendering/MainMenuAtmosphereConfig/GachaSky";
        public float audioEventDelay;
        public string audioEventName;

        private void ChangeAtmosphere()
        {
            float time = 0f;
            if (this._type == 2)
            {
                time = 18f;
            }
            else if (this._type == 1)
            {
                time = 12f;
            }
            else if (this._type == 3)
            {
                time = 18f;
            }
            ConfigAtmosphereSeries series = ConfigAtmosphereSeries.LoadFromFileAndDetach(ATMOSPHERE_PATH);
            int key = series.KeyBeforeTime(time);
            ConfigAtmosphere config = series.Value(key);
            if (this._mainMenuStage != null)
            {
                this._mainMenuStage.SetupAtmosphere(series.Common, config);
                this._mainMenuStage.IsUpdateAtmosphereAuto = false;
                this._mainMenuStage.ForceUpdateAtmosphere = false;
            }
        }

        public void Init(GameObject spaceShip, GameObject uiCamera, GachaType type, GachaMainPageContext.GachaAmountType amountType, Action endCallBack = null)
        {
            this._spaceShip = spaceShip;
            this._uiCamera = uiCamera;
            this._type = type;
            this._endCallBack = endCallBack;
            this._amountType = amountType;
            this._mainMenuStage = (spaceShip != null) ? spaceShip.GetComponent<MainMenuStage>() : null;
        }

        private void RestoreAtmosphere()
        {
            if (this._mainMenuStage != null)
            {
                this._mainMenuStage.IsUpdateAtmosphereAuto = true;
                this._mainMenuStage.ForceUpdateAtmosphere = true;
            }
        }

        private void Start()
        {
            this.ChangeAtmosphere();
            this._uiCamera.SetActive(false);
            if (this._spaceShip != null)
            {
                this._spaceShip.SetActive(true);
                this._warShip = this._spaceShip.transform.Find("Warship").gameObject;
                if (this._warShip != null)
                {
                    this._warShip.SetActive(false);
                }
            }
            Transform transform = base.transform.Find("GachaStage/boxInitPositon");
            if (this._type == 2)
            {
                this._box = UnityEngine.Object.Instantiate<GameObject>((this._amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? this._boxHCBigPrefab : this._boxHCPrefab);
            }
            else if (this._type == 1)
            {
                this._box = UnityEngine.Object.Instantiate<GameObject>((this._amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? this._boxFPBigPrefab : this._boxFPPrefab);
            }
            else if (this._type == 3)
            {
                this._box = UnityEngine.Object.Instantiate<GameObject>((this._amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? this._boxSPBigPrefab : this._boxSPPrefab);
            }
            else
            {
                this._box = null;
            }
            this._box.transform.position = transform.position;
            this._box.transform.rotation = transform.rotation;
            float x = (this._amountType != GachaMainPageContext.GachaAmountType.GachaOne) ? this._tenBoxScale : this._boxScale;
            this._box.transform.localScale = new Vector3(x, x, x);
            base.transform.Find("background").GetComponent<MeshRenderer>().material = (this._type != 1) ? this._matAvatar : this._matEquipment;
            foreach (MeshRenderer renderer in base.transform.Find("GachaStage").GetComponentsInChildren<MeshRenderer>())
            {
                if (this._type == 2)
                {
                    renderer.material = this._matStageHC;
                }
                else if (this._type == 1)
                {
                    renderer.material = this._matStageFP;
                }
                else if (this._type == 3)
                {
                    renderer.material = this._matStageSP;
                }
            }
            this._gachaEffect = base.transform.Find("GachaEffect/GachaEffectUI_Colored").gameObject;
            this._gachaEffect.SetActive(this._type != 1);
            this._gachaCameraAnimator = base.transform.Find("3DCamera").GetComponent<Animator>();
            if (this._amountType == GachaMainPageContext.GachaAmountType.GachaOne)
            {
                this._gachaCameraAnimator.SetTrigger("GachaOneTrigger");
            }
            else if (this._amountType == GachaMainPageContext.GachaAmountType.GachaTen)
            {
                this._gachaCameraAnimator.SetTrigger("GachaTenTrigger");
            }
            if (this.audioEventDelay == 0f)
            {
                Singleton<WwiseAudioManager>.Instance.Post(this.audioEventName, null, null, null);
                this._audioEventPosted = true;
            }
            this._bUIFadeEffect = false;
        }

        private void Update()
        {
            this._time += Time.deltaTime;
            if (this._time >= this._duration)
            {
                this._uiCamera.SetActive(true);
                if (this._warShip != null)
                {
                    this._warShip.SetActive(true);
                }
                if (this._spaceShip != null)
                {
                    this._spaceShip.SetActive(false);
                }
                if (this._endCallBack != null)
                {
                    this._endCallBack();
                }
                UnityEngine.Object.Destroy(this._box);
                UnityEngine.Object.Destroy(base.gameObject);
                this.RestoreAtmosphere();
            }
            if ((this._time > this._FadeStart) && !this._bUIFadeEffect)
            {
                this._bUIFadeEffect = true;
                UnityEngine.Object.Instantiate<GameObject>(this._uiFadeEffect).GetComponent<MonoUIFadeEffect>().Init(this._FadeDuration);
            }
            if ((this._time > this.audioEventDelay) && !this._audioEventPosted)
            {
                this._audioEventPosted = true;
                Singleton<WwiseAudioManager>.Instance.Post(this.audioEventName, null, null, null);
            }
        }
    }
}

