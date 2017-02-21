namespace UnityEngine.UI
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Events;

    [AddComponentMenu("UI/RectMask"), DisallowMultipleComponent, RequireComponent(typeof(RectTransform))]
    public class RectMask : MonoBehaviour
    {
        [SerializeField]
        private float _bottomTransition;
        private HashSet<MaskableGraphic> _graphicSet;
        private bool _isGraphicDirty;
        private bool _isMaterialDirty;
        [SerializeField]
        private float _leftTransition;
        private List<MatEntry> _matList = new List<MatEntry>();
        private int _propertyIdRect;
        private int _propertyIdToggle;
        private int _propertyIdTransitionWidth;
        [SerializeField]
        private float _rightTransition;
        private Canvas _rootCanvas;
        [SerializeField]
        private float _topTransition;
        private RectTransform _transform;
        private static readonly string DEFAULT_SHADER_NAME = "UI/Default";
        private static readonly string KEYWORD_TOGGLE = "RECT_MASK";
        private static readonly string PROPERTY_RECT = "_RMRect";
        private static readonly string PROPERTY_TOGGLE = "_RectMask";
        private static readonly string PROPERTY_TRANSITION_WIDTH = "_RMTransitWidth";
        private static Shader RECTMASK_DEFAULT_SHADER;
        private static readonly string RECTMASK_DEFAULT_SHADER_NAME = "miHoYo/UI/Default";

        public void AddGraphic(MaskableGraphic graphic)
        {
            if (!this._graphicSet.Contains(graphic))
            {
                bool flag = this._isMaterialDirty;
                this._graphicSet.Add(graphic);
                this.SetupGraphic(graphic);
                this._isMaterialDirty = flag;
            }
        }

        private Material AddMaterial(Material baseMat)
        {
            if (baseMat.shader.name != DEFAULT_SHADER_NAME)
            {
                if (!baseMat.HasProperty(this._propertyIdToggle))
                {
                    Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_TOGGLE + " property", baseMat);
                    return baseMat;
                }
                if (!baseMat.HasProperty(this._propertyIdRect))
                {
                    Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_RECT + " property", baseMat);
                    return baseMat;
                }
                if (!baseMat.HasProperty(this._propertyIdTransitionWidth))
                {
                    Debug.LogWarning("Material " + baseMat.name + " doesn't have " + PROPERTY_TRANSITION_WIDTH + " property", baseMat);
                    return baseMat;
                }
            }
            for (int i = 0; i < this._matList.Count; i++)
            {
                MatEntry entry = this._matList[i];
                if (entry.baseMat == baseMat)
                {
                    entry.count++;
                    return entry.customMat;
                }
            }
            MatEntry item = new MatEntry {
                count = 1,
                baseMat = baseMat,
                customMat = new Material(baseMat)
            };
            item.customMat.name = string.Format("{0} (RectMask Instance)", baseMat.name);
            if (item.customMat.shader.name == DEFAULT_SHADER_NAME)
            {
                item.customMat.shader = this._rectmaskDefaultShader;
            }
            item.customMat.SetInt(this._propertyIdToggle, 1);
            item.customMat.EnableKeyword(KEYWORD_TOGGLE);
            item.customMat.SetVector(this._propertyIdRect, this._rect);
            item.customMat.SetVector(this._propertyIdTransitionWidth, this._transitions);
            this._matList.Add(item);
            return item.customMat;
        }

        private void Awake()
        {
            this._propertyIdToggle = Shader.PropertyToID(PROPERTY_TOGGLE);
            this._propertyIdRect = Shader.PropertyToID(PROPERTY_RECT);
            this._propertyIdTransitionWidth = Shader.PropertyToID(PROPERTY_TRANSITION_WIDTH);
            this._graphicSet = new HashSet<MaskableGraphic>();
            this._transform = base.GetComponent<RectTransform>();
            RectTransform.reapplyDrivenProperties += new RectTransform.ReapplyDrivenProperties(this.OnTransformReapplyDrivenProperties);
            this._rootCanvas = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
            if (this._rootCanvas == null)
            {
                Debug.LogError("Cannot find root Canvas.", this);
                base.enabled = false;
            }
        }

        private void ClearAllGraphics()
        {
            foreach (MaskableGraphic graphic in this._graphicSet)
            {
                if (graphic != null)
                {
                    this.RestoreGraphic(graphic);
                }
            }
            this._graphicSet.Clear();
            this._matList.Clear();
        }

        private void ClearAllMaterial()
        {
            for (int i = 0; i < this._matList.Count; i++)
            {
                MatEntry entry = this._matList[i];
                UnityEngine.Object.Destroy(entry.customMat);
                entry.baseMat = null;
            }
            this._matList.Clear();
        }

        public void DeleteGraphic(MaskableGraphic graphic)
        {
            if (this._graphicSet.Contains(graphic))
            {
                this.RestoreGraphic(graphic);
                this._graphicSet.Remove(graphic);
            }
        }

        private bool HasMaterial(Material customMat)
        {
            for (int i = 0; i < this._matList.Count; i++)
            {
                MatEntry entry = this._matList[i];
                if (entry.customMat == customMat)
                {
                    return true;
                }
            }
            return false;
        }

        private void LateUpdate()
        {
            this.UpdateMask();
        }

        private void OnDisable()
        {
            this.ClearAllGraphics();
        }

        private void OnEnable()
        {
            this.SetGraphicDirty();
        }

        public void OnTransformReapplyDrivenProperties(RectTransform driven)
        {
            if (driven == this._transform)
            {
                this.SetMaterialDirty();
            }
        }

        private Material RemoveMaterial(Material customMat)
        {
            if (customMat != null)
            {
                for (int i = 0; i < this._matList.Count; i++)
                {
                    MatEntry entry = this._matList[i];
                    if (entry.customMat == customMat)
                    {
                        Material baseMat = null;
                        if (--entry.count == 0)
                        {
                            UnityEngine.Object.Destroy(entry.customMat);
                            baseMat = entry.baseMat;
                            entry.baseMat = null;
                            this._matList.RemoveAt(i);
                        }
                        return baseMat;
                    }
                }
            }
            return null;
        }

        public void RestoreGraphic(MaskableGraphic graphic)
        {
            graphic.UnregisterDirtyMaterialCallback(new UnityAction(this.SetAllGraphicMaterials));
            this.RestoreGraphicMaterial(graphic);
        }

        private void RestoreGraphicMaterial(MaskableGraphic graphic)
        {
            graphic.material = this.RemoveMaterial(graphic.material);
        }

        private void SetAllGraphicMaterials()
        {
            foreach (MaskableGraphic graphic in this._graphicSet)
            {
                if (graphic != null)
                {
                    this.SetGraphicMaterial(graphic);
                }
            }
        }

        public void SetGraphicDirty()
        {
            this._isGraphicDirty = true;
        }

        private void SetGraphicMaterial(MaskableGraphic graphic)
        {
            Material customMat = graphic.material;
            if (!this.HasMaterial(customMat))
            {
                Material material2 = this.AddMaterial(customMat);
                if (material2 != customMat)
                {
                    graphic.material = material2;
                }
            }
        }

        public void SetMaterialDirty()
        {
            this._isMaterialDirty = true;
        }

        private void SetupAllGraphics()
        {
            List<MaskableGraphic> toRelease = ListPool<MaskableGraphic>.Get();
            List<MaskableGraphic> result = toRelease;
            base.transform.GetComponentsInChildren<MaskableGraphic>(true, result);
            for (int i = 0; i < toRelease.Count; i++)
            {
                this.AddGraphic(toRelease[i]);
            }
            ListPool<MaskableGraphic>.Release(toRelease);
        }

        private void SetupGraphic(MaskableGraphic graphic)
        {
            graphic.RegisterDirtyMaterialCallback(new UnityAction(this.SetAllGraphicMaterials));
            this.SetGraphicMaterial(graphic);
        }

        private void Update()
        {
            this.UpdateMask();
        }

        private void UpdateAllMaterial()
        {
            for (int i = 0; i < this._matList.Count; i++)
            {
                MatEntry entry = this._matList[i];
                entry.customMat.SetVector(this._propertyIdRect, this._rect);
                entry.customMat.SetVector(this._propertyIdTransitionWidth, this._transitions);
            }
        }

        private void UpdateMask()
        {
            if (this._isGraphicDirty)
            {
                this.ClearAllGraphics();
                this.SetupAllGraphics();
                this._isGraphicDirty = false;
                this._isMaterialDirty = false;
            }
            else if (this._isMaterialDirty)
            {
                this.UpdateAllMaterial();
                this.SetAllGraphicMaterials();
                this._isMaterialDirty = false;
            }
        }

        private Vector4 _rect
        {
            get
            {
                Vector3[] fourCornersArray = new Vector3[4];
                this._transform.GetWorldCorners(fourCornersArray);
                if (this._rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    Camera worldCamera = this._rootCanvas.worldCamera;
                    fourCornersArray[0] = worldCamera.WorldToViewportPoint(fourCornersArray[0]);
                    fourCornersArray[2] = worldCamera.WorldToViewportPoint(fourCornersArray[2]);
                }
                return new Vector4(fourCornersArray[0].x, fourCornersArray[0].y, fourCornersArray[2].x, fourCornersArray[2].y);
            }
        }

        private Shader _rectmaskDefaultShader
        {
            get
            {
                if (RECTMASK_DEFAULT_SHADER == null)
                {
                    RECTMASK_DEFAULT_SHADER = Shader.Find(RECTMASK_DEFAULT_SHADER_NAME);
                    if ((RECTMASK_DEFAULT_SHADER == null) || !RECTMASK_DEFAULT_SHADER.isSupported)
                    {
                        Debug.LogError(string.Format("Shader '{0}' fail to load", RECTMASK_DEFAULT_SHADER_NAME));
                        base.enabled = false;
                    }
                }
                return RECTMASK_DEFAULT_SHADER;
            }
        }

        private Vector4 _transitions
        {
            get
            {
                Vector4 vector = new Vector4(this._leftTransition, this._bottomTransition, this._rightTransition, this._topTransition);
                if (this._rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
                {
                    Camera worldCamera = this._rootCanvas.worldCamera;
                    vector.x /= (float) worldCamera.pixelWidth;
                    vector.z /= (float) worldCamera.pixelWidth;
                    vector.y /= (float) worldCamera.pixelHeight;
                    vector.w /= (float) worldCamera.pixelHeight;
                }
                return vector;
            }
        }

        public float bottomTransition
        {
            get
            {
                return this._bottomTransition;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetStruct<float>(ref this._bottomTransition, value))
                {
                    this.SetMaterialDirty();
                }
            }
        }

        public float leftTransition
        {
            get
            {
                return this._leftTransition;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetStruct<float>(ref this._leftTransition, value))
                {
                    this.SetMaterialDirty();
                }
            }
        }

        public float rightTransition
        {
            get
            {
                return this._rightTransition;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetStruct<float>(ref this._rightTransition, value))
                {
                    this.SetMaterialDirty();
                }
            }
        }

        public float topTransition
        {
            get
            {
                return this._topTransition;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetStruct<float>(ref this._topTransition, value))
                {
                    this.SetMaterialDirty();
                }
            }
        }

        private class MatEntry
        {
            public Material baseMat;
            public int count;
            public Material customMat;
        }
    }
}

