namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public class MonoWeaponRenderImage : MonoBehaviour
    {
        private GameObject _containerGo;
        private int _index;
        private bool _isStatic;
        private RenderTextureWrapper _renderTexture;
        private int _rtDepth = 0x18;
        private RenderTextureFormat _rtFormat;
        private int _rtHeight = 300;
        private int _rtWidth = 600;
        private Coroutine _setupWeapon3dModelCoroutine;
        private bool _triggerRebindRenderTextureToCamera;
        private WeaponDataItem _weaponData;
        private const string CONTAINER_PREFAB_PATH = "UI/Menus/Widget/Storage/Weapon3dModel";
        private const float EMISSION_BLOOM_FACTOR_SCALE = 0.7f;
        private const float EMISSION_SCALE = 0.7f;
        private const float EVO_OFFSET_Y = 10f;
        private const string TEXTURE_PATH = "UI/RenderTexture/WeaponRenderTexture";
        public const int WEAPON_LAYER = 0x1a;

        public void CleanUp()
        {
            if (this._setupWeapon3dModelCoroutine != null)
            {
                if (Singleton<ApplicationManager>.Instance != null)
                {
                    Singleton<ApplicationManager>.Instance.StopCoroutine(this._setupWeapon3dModelCoroutine);
                }
                this._setupWeapon3dModelCoroutine = null;
            }
            if (this._containerGo != null)
            {
                UnityEngine.Object.Destroy(this._containerGo);
                this._containerGo = null;
            }
            if (this._renderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._renderTexture);
                this._renderTexture = null;
            }
        }

        [DebuggerHidden]
        private IEnumerator DoSetupWeapon3dModel()
        {
            return new <DoSetupWeapon3dModel>c__Iterator79 { <>f__this = this };
        }

        private string GetWeaponPrefaPath()
        {
            return this._weaponData.GetPrefabPath();
        }

        public void OnDestroy()
        {
            this.CleanUp();
        }

        public void OnDisable()
        {
            if (this._containerGo != null)
            {
                this._containerGo.gameObject.SetActive(false);
            }
        }

        public void OnEnable()
        {
            if (this._containerGo != null)
            {
                this._containerGo.gameObject.SetActive(true);
            }
        }

        private void OnRebindToCamera()
        {
            if (this._isStatic)
            {
                this._triggerRebindRenderTextureToCamera = true;
            }
        }

        private void SetMaterial(GameObject obj)
        {
            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    bool flag = false;
                    if (material.HasProperty("_EmissionOverride"))
                    {
                        flag = material.GetInt("_EmissionOverride") == 1;
                    }
                    if (flag)
                    {
                        if (material.HasProperty("_EOEmissionScaler"))
                        {
                            material.SetFloat("_EmissionScaler", material.GetFloat("_EOEmissionScaler"));
                        }
                        if (material.HasProperty("_EOPartialEmissionScaler"))
                        {
                            material.SetFloat("_PartialEmissionScaler", material.GetFloat("_EOPartialEmissionScaler"));
                        }
                        if (material.HasProperty("_EOEmissionBloomFactor"))
                        {
                            material.SetFloat("_EmissionBloomFactor", material.GetFloat("_EOEmissionBloomFactor"));
                        }
                    }
                    else
                    {
                        this.TuneMaterialFloat(material, "_EmissionScaler", 0.7f);
                        this.TuneMaterialFloat(material, "_PartialEmissionScaler", 0.7f);
                        this.TuneMaterialFloat(material, "_EmissionBloomFactor", 0.7f);
                    }
                    if (material.HasProperty("_SPTransition"))
                    {
                        material.SetFloat("_SPTransition", 0f);
                    }
                }
            }
        }

        public void SetupView(WeaponDataItem weaponData, bool isStatic = false, int index = 0)
        {
            this._weaponData = weaponData;
            this._isStatic = isStatic;
            this._index = index;
            this.CleanUp();
            this._setupWeapon3dModelCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoSetupWeapon3dModel());
        }

        private void SetupWeaponView(GameObject weaponGo)
        {
            if (weaponGo.transform.Find("TransformCopy") != null)
            {
                Transform transform = weaponGo.transform.Find("TransformCopy");
                weaponGo.transform.localPosition = transform.localPosition;
                weaponGo.transform.localEulerAngles = transform.localEulerAngles;
                weaponGo.transform.localScale = transform.localScale;
            }
            if ((weaponGo.transform.Find("ShortSword") != null) && (weaponGo.transform.Find("LongSword") != null))
            {
                weaponGo.transform.Find("ShortSword").gameObject.SetActive(false);
                weaponGo.transform.Find("LongSword").gameObject.SetActive(true);
            }
        }

        private void TuneMaterialFloat(Material material, string name, float scale)
        {
            if (material.HasProperty(name))
            {
                material.SetFloat(name, Mathf.Max((float) 1f, (float) (material.GetFloat(name) * scale)));
            }
        }

        public void Update()
        {
            if ((this._isStatic && this._triggerRebindRenderTextureToCamera) && ((this._renderTexture != null) && !this._renderTexture.IsCreated()))
            {
                this._triggerRebindRenderTextureToCamera = false;
                this.CleanUp();
                this._setupWeapon3dModelCoroutine = Singleton<ApplicationManager>.Instance.StartCoroutine(this.DoSetupWeapon3dModel());
            }
        }

        [CompilerGenerated]
        private sealed class <DoSetupWeapon3dModel>c__Iterator79 : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal MonoWeaponRenderImage <>f__this;
            internal Camera <camera>__2;
            internal Canvas <canvas>__1;
            internal float <scalerFactor>__0;
            internal GameObject <weaponGo>__3;

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
                        this.$current = null;
                        this.$PC = 1;
                        return true;

                    case 1:
                        this.<>f__this.transform.GetComponent<RawImage>().enabled = true;
                        this.<scalerFactor>__0 = 1f;
                        this.<canvas>__1 = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
                        if ((this.<canvas>__1 != null) && (this.<canvas>__1.renderMode != RenderMode.WorldSpace))
                        {
                            this.<scalerFactor>__0 = this.<canvas>__1.scaleFactor;
                        }
                        this.<>f__this._containerGo = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>("UI/Menus/Widget/Storage/Weapon3dModel", BundleType.RESOURCE_FILE));
                        this.<camera>__2 = this.<>f__this._containerGo.transform.Find("WeaponCamera").GetComponent<Camera>();
                        this.<>f__this._renderTexture = GraphicsUtils.GetRenderTexture((int) (this.<>f__this._rtWidth * this.<scalerFactor>__0), (int) (this.<>f__this._rtHeight * this.<scalerFactor>__0), this.<>f__this._rtDepth, this.<>f__this._rtFormat);
                        this.<>f__this._renderTexture.onRebindToCameraCallBack = new Action(this.<>f__this.OnRebindToCamera);
                        if (this.<>f__this._renderTexture.IsValid())
                        {
                            this.<>f__this._renderTexture.BindToCamera(this.<camera>__2);
                            this.<>f__this._renderTexture.content.filterMode = FilterMode.Bilinear;
                            this.<>f__this.transform.GetComponent<RawImage>().texture = (Texture) this.<>f__this._renderTexture;
                        }
                        this.<weaponGo>__3 = UnityEngine.Object.Instantiate<GameObject>(Miscs.LoadResource<GameObject>(this.<>f__this.GetWeaponPrefaPath(), BundleType.RESOURCE_FILE));
                        this.<>f__this.SetMaterial(this.<weaponGo>__3);
                        this.<weaponGo>__3.transform.SetParent(this.<>f__this._containerGo.transform.Find("Weapon"), false);
                        this.<weaponGo>__3.SetLayer(0x1a, true);
                        this.<>f__this.SetupWeaponView(this.<weaponGo>__3);
                        this.<>f__this._containerGo.transform.AddLocalPositionY(10f * this.<>f__this._index);
                        if (this.<>f__this._isStatic)
                        {
                            this.<camera>__2.Render();
                            this.<camera>__2.targetTexture = null;
                            this.<>f__this._containerGo.SetActive(false);
                            UnityEngine.Object.Destroy(this.<>f__this._containerGo);
                        }
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

