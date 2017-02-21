namespace UnityEngine.UI
{
    using MoleMole;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.Sprites;

    [AddComponentMenu("UI/MultiLayerImage")]
    public class MultiLayerImage : MaskableGraphic, ISerializationCallbackReceiver, ILayoutElement, ICanvasRaycastFilter
    {
        private static Material[] DEFAULT_MATERIALS = new Material[4];
        private float m_EventAlphaThreshold = 1f;
        private Material m_InstancedMaterial;
        private bool m_IsMatIdChange;
        private int m_matId = -1;
        private Material m_SourceMaterial;
        [SerializeField, FormerlySerializedAs("m_Frame")]
        private Sprite m_Sprite;
        [SerializeField]
        private Sprite m_Sprite2;
        [SerializeField]
        private Sprite m_Sprite3;
        [SerializeField]
        private Sprite m_Sprite4;
        [SerializeField]
        private Color m_SpriteColor = Color.white;
        [SerializeField]
        private Color m_SpriteColor2 = Color.white;
        [SerializeField]
        private Color m_SpriteColor3 = Color.white;
        [SerializeField]
        private Color m_SpriteColor4 = Color.white;
        [SerializeField]
        private Vector2 m_SpriteOffset;
        [SerializeField]
        private Vector2 m_SpriteOffset2;
        [SerializeField]
        private Vector2 m_SpriteOffset3;
        [SerializeField]
        private Vector2 m_SpriteOffset4;
        [SerializeField]
        private Vector2 m_SpriteSize;
        [SerializeField]
        private Vector2 m_SpriteSize2;
        [SerializeField]
        private Vector2 m_SpriteSize3;
        [SerializeField]
        private Vector2 m_SpriteSize4;
        private static readonly string[] SHADER_NAMES = new string[] { "miHoYo/UI/Image Multi-Layer 1", "miHoYo/UI/Image Multi-Layer 2", "miHoYo/UI/Image Multi-Layer 3", "miHoYo/UI/Image Multi-Layer 4" };

        protected MultiLayerImage()
        {
            base.set_useLegacyMeshGeneration(false);
        }

        private List<UIQuad> AddLayer(Sprite sprite, int spriteId, Rect trsfRect, Vector2 offset, Vector2 size, List<UIQuad> srcQuads)
        {
            Vector4 vector = (sprite == null) ? Vector4.zero : DataUtility.GetOuterUV(sprite);
            size.x = (size.x >= float.Epsilon) ? size.x : trsfRect.size.x;
            size.y = (size.y >= float.Epsilon) ? size.y : trsfRect.size.y;
            Vector4 drawingDimensions = this.GetDrawingDimensions(this.GetSpriteRect(trsfRect.center, offset, size));
            UIQuad another = new UIQuad {
                min = new Vector2(drawingDimensions.x, drawingDimensions.y),
                max = new Vector2(drawingDimensions.z, drawingDimensions.w)
            };
            another.uvMin[spriteId] = new Vector2(vector.x, vector.y);
            another.uvMax[spriteId] = new Vector2(vector.z, vector.w);
            List<UIQuad> list = new List<UIQuad>();
            List<UIQuad> collection = new List<UIQuad> {
                another
            };
            for (int i = 0; i < srcQuads.Count; i++)
            {
                List<UIQuad> unOverlappedList = new List<UIQuad>();
                UIQuad item = srcQuads[i].Split(another, spriteId, unOverlappedList, false);
                if (item != null)
                {
                    list.Add(item);
                }
                for (int j = 0; j < unOverlappedList.Count; j++)
                {
                    unOverlappedList[j].uvMin[spriteId] = -Vector2.one;
                    unOverlappedList[j].uvMax[spriteId] = -Vector2.one;
                }
                list.AddRange(unOverlappedList);
                List<UIQuad> list4 = new List<UIQuad>();
                for (int k = 0; k < collection.Count; k++)
                {
                    collection[k].Split(srcQuads[i], spriteId, list4, true);
                }
                collection = list4;
            }
            list.AddRange(collection);
            return list;
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        private void GenerateSimpleSprite(VertexHelper vh)
        {
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            List<UIQuad> srcQuads = new List<UIQuad>();
            if (this.sprite != null)
            {
                srcQuads = this.AddLayer(this.sprite, 0, pixelAdjustedRect, this.spriteOffset, this.spriteSize, srcQuads);
            }
            if (this.sprite2 != null)
            {
                srcQuads = this.AddLayer(this.sprite2, 1, pixelAdjustedRect, this.spriteOffset2, this.spriteSize2, srcQuads);
            }
            if (this.sprite3 != null)
            {
                srcQuads = this.AddLayer(this.sprite3, 2, pixelAdjustedRect, this.spriteOffset3, this.spriteSize3, srcQuads);
            }
            if (this.sprite4 != null)
            {
                srcQuads = this.AddLayer(this.sprite4, 3, pixelAdjustedRect, this.spriteOffset4, this.spriteSize4, srcQuads);
            }
            Color color = base.color;
            vh.Clear();
            UIVertex template = new UIVertex {
                color = color
            };
            for (int i = 0; i < srcQuads.Count; i++)
            {
                vh.AddUIVertexQuad(srcQuads[i].ToUIQuad(template));
            }
        }

        private unsafe Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int i = 0; i <= 1; i++)
            {
                float num2 = border[i] + border[i + 2];
                if ((rect.size[i] < num2) && (num2 != 0f))
                {
                    ref Vector4 vectorRef;
                    int num4;
                    ref Vector4 vectorRef2;
                    float num3 = rect.size[i] / num2;
                    float num5 = vectorRef[num4];
                    (vectorRef = (Vector4) &border)[num4 = i] = num5 * num3;
                    num5 = vectorRef2[num4];
                    (vectorRef2 = (Vector4) &border)[num4 = i + 2] = num5 * num3;
                }
            }
            return border;
        }

        private Vector4 GetDrawingDimensions(Rect r)
        {
            Vector4 vector = (this.sprite != null) ? DataUtility.GetPadding(this.sprite) : Vector4.zero;
            Vector2 vector2 = (this.sprite != null) ? new Vector2(this.sprite.rect.width, this.sprite.rect.height) : Vector2.zero;
            int num = Mathf.RoundToInt(vector2.x);
            int num2 = Mathf.RoundToInt(vector2.y);
            Vector4 vector3 = new Vector4(vector.x / ((float) num), vector.y / ((float) num2), (num - vector.z) / ((float) num), (num2 - vector.w) / ((float) num2));
            return new Vector4(r.x + (r.width * vector3.x), r.y + (r.height * vector3.y), r.x + (r.width * vector3.z), r.y + (r.height * vector3.w));
        }

        private Rect GetSpriteRect(Vector2 center, Vector2 offset, Vector2 size)
        {
            return new Rect { size = size, center = center + offset };
        }

        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 vector;
            if (this.m_EventAlphaThreshold >= 1f)
            {
                return true;
            }
            Sprite sprite = this.sprite;
            if (sprite == null)
            {
                return true;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
            vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
            vector = this.MapCoordinate(vector, pixelAdjustedRect);
            Rect textureRect = sprite.textureRect;
            Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
            float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / ((float) sprite.texture.width);
            float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / ((float) sprite.texture.height);
            try
            {
                return (sprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold);
            }
            catch (UnityException exception)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + exception.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect rect2 = this.sprite.rect;
            return new Vector2((local.x * rect2.width) / rect.width, (local.y * rect2.height) / rect.height);
        }

        public virtual void OnAfterDeserialize()
        {
        }

        public virtual void OnBeforeSerialize()
        {
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            if (this.sprite == null)
            {
                base.OnPopulateMesh(toFill);
            }
            else
            {
                this.GenerateSimpleSprite(toFill);
            }
        }

        public override void SetNativeSize()
        {
            if (this.sprite != null)
            {
                float x = this.sprite.rect.width / this.pixelsPerUnit;
                float y = this.sprite.rect.height / this.pixelsPerUnit;
                base.rectTransform.anchorMax = base.rectTransform.anchorMin;
                base.rectTransform.sizeDelta = new Vector2(x, y);
                this.SetAllDirty();
            }
        }

        protected override void UpdateMaterial()
        {
            if (this.IsActive())
            {
                Material materialForRendering = this.materialForRendering;
                if (materialForRendering.HasProperty("_Color0"))
                {
                    materialForRendering.SetColor("_Color0", this.spriteColor);
                }
                if (materialForRendering.HasProperty("_Color1"))
                {
                    materialForRendering.SetColor("_Color1", this.spriteColor2);
                }
                if (materialForRendering.HasProperty("_Color2"))
                {
                    materialForRendering.SetColor("_Color2", this.spriteColor3);
                }
                if (materialForRendering.HasProperty("_Color3"))
                {
                    materialForRendering.SetColor("_Color3", this.spriteColor4);
                }
                if ((this.sprite != null) && materialForRendering.HasProperty("_Tex0"))
                {
                    materialForRendering.SetTexture("_Tex0", this.sprite.texture);
                }
                if ((this.sprite2 != null) && materialForRendering.HasProperty("_Tex1"))
                {
                    materialForRendering.SetTexture("_Tex1", this.sprite2.texture);
                }
                if ((this.sprite3 != null) && materialForRendering.HasProperty("_Tex2"))
                {
                    materialForRendering.SetTexture("_Tex2", this.sprite3.texture);
                }
                if ((this.sprite4 != null) && materialForRendering.HasProperty("_Tex3"))
                {
                    materialForRendering.SetTexture("_Tex3", this.sprite4.texture);
                }
                base.canvasRenderer.materialCount = 1;
                base.canvasRenderer.SetMaterial(materialForRendering, 0);
                base.canvasRenderer.SetTexture(this.mainTexture);
            }
        }

        public override Material defaultMaterial
        {
            get
            {
                return this.defaultMultiLayerMaterial;
            }
        }

        private Material defaultMultiLayerMaterial
        {
            get
            {
                int index = 0;
                if (this.sprite4 != null)
                {
                    index = 3;
                }
                else if (this.sprite3 != null)
                {
                    index = 2;
                }
                else if (this.sprite2 != null)
                {
                    index = 1;
                }
                if (index != this.m_matId)
                {
                    this.m_matId = index;
                    this.m_IsMatIdChange = true;
                }
                if (DEFAULT_MATERIALS[index] == null)
                {
                    Shader shader = Shader.Find(SHADER_NAMES[index]);
                    if ((shader == null) || !shader.isSupported)
                    {
                        Debug.LogError(string.Format("Shader '{0}' fail to load", SHADER_NAMES[index]));
                        base.enabled = false;
                    }
                    DEFAULT_MATERIALS[index] = new Material(shader);
                }
                return DEFAULT_MATERIALS[index];
            }
        }

        public float eventAlphaThreshold
        {
            get
            {
                return this.m_EventAlphaThreshold;
            }
            set
            {
                this.m_EventAlphaThreshold = value;
            }
        }

        public virtual float flexibleHeight
        {
            get
            {
                return -1f;
            }
        }

        public virtual float flexibleWidth
        {
            get
            {
                return -1f;
            }
        }

        public bool hasBorder
        {
            get
            {
                return ((this.sprite != null) && (this.sprite.border.sqrMagnitude > 0f));
            }
        }

        public virtual int layoutPriority
        {
            get
            {
                return 0;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                if (this.sprite != null)
                {
                    return this.sprite.texture;
                }
                if ((this.material != null) && (this.material.mainTexture != null))
                {
                    return this.material.mainTexture;
                }
                return Graphic.s_WhiteTexture;
            }
        }

        public override Material material
        {
            get
            {
                Material source = (base.m_Material == null) ? this.defaultMaterial : base.m_Material;
                if (!Application.isPlaying)
                {
                    if ((this.m_InstancedMaterial == null) || this.m_IsMatIdChange)
                    {
                        this.m_IsMatIdChange = false;
                        if (this.m_InstancedMaterial != null)
                        {
                            if (Application.isEditor)
                            {
                                UnityEngine.Object.DestroyImmediate(this.m_InstancedMaterial);
                            }
                            else
                            {
                                UnityEngine.Object.Destroy(this.m_InstancedMaterial);
                            }
                        }
                        this.m_InstancedMaterial = new Material(source);
                        this.m_InstancedMaterial.shaderKeywords = source.shaderKeywords;
                    }
                    return this.m_InstancedMaterial;
                }
                if ((this.m_SourceMaterial == null) || this.m_IsMatIdChange)
                {
                    this.m_IsMatIdChange = false;
                    this.m_SourceMaterial = source;
                    base.m_Material = null;
                }
                if (base.m_Material == null)
                {
                    base.m_Material = new Material(this.m_SourceMaterial);
                    base.m_Material.shaderKeywords = this.m_SourceMaterial.shaderKeywords;
                }
                return base.m_Material;
            }
            set
            {
                if (base.m_Material != value)
                {
                    base.m_Material = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public virtual float minHeight
        {
            get
            {
                return 0f;
            }
        }

        public virtual float minWidth
        {
            get
            {
                return 0f;
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float pixelsPerUnit = 100f;
                if (this.sprite != null)
                {
                    pixelsPerUnit = this.sprite.pixelsPerUnit;
                }
                float referencePixelsPerUnit = 100f;
                if (base.canvas != null)
                {
                    referencePixelsPerUnit = base.canvas.referencePixelsPerUnit;
                }
                return (pixelsPerUnit / referencePixelsPerUnit);
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                if (this.sprite == null)
                {
                    return 0f;
                }
                return (this.sprite.rect.size.y / this.pixelsPerUnit);
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                if (this.sprite == null)
                {
                    return 0f;
                }
                return (this.sprite.rect.size.x / this.pixelsPerUnit);
            }
        }

        public Sprite sprite
        {
            get
            {
                return this.m_Sprite;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public Sprite sprite2
        {
            get
            {
                return this.m_Sprite2;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite2, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public Sprite sprite3
        {
            get
            {
                return this.m_Sprite3;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite3, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public Sprite sprite4
        {
            get
            {
                return this.m_Sprite4;
            }
            set
            {
                if (UnityEngine.UI.SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite4, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public Color spriteColor
        {
            get
            {
                return this.m_SpriteColor;
            }
            set
            {
                if (this.m_SpriteColor != value)
                {
                    this.m_SpriteColor = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public Color spriteColor2
        {
            get
            {
                return this.m_SpriteColor2;
            }
            set
            {
                if (this.m_SpriteColor2 != value)
                {
                    this.m_SpriteColor2 = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public Color spriteColor3
        {
            get
            {
                return this.m_SpriteColor3;
            }
            set
            {
                if (this.m_SpriteColor3 != value)
                {
                    this.m_SpriteColor3 = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public Color spriteColor4
        {
            get
            {
                return this.m_SpriteColor4;
            }
            set
            {
                if (this.m_SpriteColor4 != value)
                {
                    this.m_SpriteColor4 = value;
                    this.SetMaterialDirty();
                }
            }
        }

        public Vector2 spriteOffset
        {
            get
            {
                return this.m_SpriteOffset;
            }
            set
            {
                if (this.m_SpriteOffset != value)
                {
                    this.m_SpriteOffset = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteOffset2
        {
            get
            {
                return this.m_SpriteOffset2;
            }
            set
            {
                if (this.m_SpriteOffset2 != value)
                {
                    this.m_SpriteOffset2 = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteOffset3
        {
            get
            {
                return this.m_SpriteOffset3;
            }
            set
            {
                if (this.m_SpriteOffset3 != value)
                {
                    this.m_SpriteOffset3 = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteOffset4
        {
            get
            {
                return this.m_SpriteOffset4;
            }
            set
            {
                if (this.m_SpriteOffset4 != value)
                {
                    this.m_SpriteOffset4 = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteSize
        {
            get
            {
                return this.m_SpriteSize;
            }
            set
            {
                if (this.m_SpriteSize != value)
                {
                    this.m_SpriteSize = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteSize2
        {
            get
            {
                return this.m_SpriteSize2;
            }
            set
            {
                if (this.m_SpriteSize2 != value)
                {
                    this.m_SpriteSize2 = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteSize3
        {
            get
            {
                return this.m_SpriteSize3;
            }
            set
            {
                if (this.m_SpriteSize3 != value)
                {
                    this.m_SpriteSize3 = value;
                    this.SetAllDirty();
                }
            }
        }

        public Vector2 spriteSize4
        {
            get
            {
                return this.m_SpriteSize4;
            }
            set
            {
                if (this.m_SpriteSize4 != value)
                {
                    this.m_SpriteSize4 = value;
                    this.SetAllDirty();
                }
            }
        }
    }
}

