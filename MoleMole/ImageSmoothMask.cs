namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(CanvasRenderer))]
    public class ImageSmoothMask : MonoBehaviour
    {
        private ImageForSmoothMask _image;
        private Material _material;
        [HideInInspector]
        public float coverRatio = 0.5f;
        public Image maskImage;
        public Shader maskShader;
        public MonoMaskSlider maskSlider;

        private void Awake()
        {
            this.Init();
        }

        private void CheckImage(Image image)
        {
            if ((image.type != Image.Type.Simple) && (image.type == Image.Type.Sliced))
            {
            }
        }

        private void CheckValid()
        {
            this.CheckImage(this._image);
            this.CheckImage(this.maskImage);
        }

        private void CreateMaterial()
        {
            this._material = new Material(this.maskShader);
        }

        public List<UIVertex[]> GenerateUIQuads()
        {
            List<UIVertex[]> list = new List<UIVertex[]>();
            List<Quad> imageQuades = GetImageQuades(this._image, base.transform.worldToLocalMatrix);
            List<Quad> list3 = GetImageQuades(this.maskImage, base.transform.worldToLocalMatrix);
            List<Quad> list4 = new List<Quad>();
            foreach (Quad quad in list3)
            {
                Quad item = null;
                foreach (Quad quad3 in imageQuades)
                {
                    item = quad3.Split(quad, null);
                    if (item != null)
                    {
                        list4.Add(item);
                    }
                }
            }
            UIVertex simpleVert = UIVertex.simpleVert;
            simpleVert.color = this._image.color;
            simpleVert.normal.x = 1f;
            foreach (Quad quad4 in list4)
            {
                list.Add(quad4.ToUIQuad(simpleVert));
            }
            return list;
        }

        private static List<Quad> GetImageQuades(Image image, Matrix4x4 mat)
        {
            List<Quad> list = new List<Quad>();
            Vector3[] fourCornersArray = new Vector3[4];
            image.rectTransform.GetWorldCorners(fourCornersArray);
            fourCornersArray[0] = mat.MultiplyPoint(fourCornersArray[0]);
            fourCornersArray[2] = mat.MultiplyPoint(fourCornersArray[2]);
            Vector2[] minMaxUV = GetMinMaxUV(image.sprite);
            Quad item = new Quad(fourCornersArray[0], fourCornersArray[2], minMaxUV[0], minMaxUV[1], minMaxUV[0], minMaxUV[1]);
            if (image.type == Image.Type.Simple)
            {
                list.Add(item);
                return list;
            }
            if (image.type == Image.Type.Sliced)
            {
                Quad quad2;
                Vector4 border = image.sprite.border;
                Rect rect = image.sprite.rect;
                quad2 = new Quad(item) {
                    min = quad2.min + new Vector2(border.x, border.y),
                    max = quad2.max - new Vector2(border.z, border.w),
                    uvMin0 = new Vector2(Mathf.Lerp(minMaxUV[0].x, minMaxUV[1].x, border.x / rect.width), Mathf.Lerp(minMaxUV[0].y, minMaxUV[1].y, border.y / rect.height)),
                    uvMin1 = quad2.uvMin0,
                    uvMax0 = new Vector2(Mathf.Lerp(minMaxUV[1].x, minMaxUV[0].x, border.z / rect.width), Mathf.Lerp(minMaxUV[1].y, minMaxUV[0].y, border.w / rect.height)),
                    uvMax1 = quad2.uvMax0
                };
                if (image.fillCenter)
                {
                    list.Add(quad2);
                }
                List<Quad> unOverlappedList = new List<Quad>();
                item.GridSplit(quad2, unOverlappedList);
                foreach (Quad quad3 in unOverlappedList)
                {
                    quad3.uvMin0 = quad3.uvMin1;
                    quad3.uvMax0 = quad3.uvMax1;
                }
                list.AddRange(unOverlappedList);
            }
            return list;
        }

        private static Vector2[] GetMinMaxUV(Sprite sprite)
        {
            Vector2[] vectorArray = new Vector2[2];
            Vector2[] uv = sprite.uv;
            vectorArray[0] = (Vector2) (Vector2.one * (float) 1.0 / (float) 0.0);
            vectorArray[1] = (Vector2) (Vector2.one * (float) -1.0 / (float) 0.0);
            foreach (Vector2 vector in uv)
            {
                vectorArray[0] = Vector2.Min(vectorArray[0], vector);
                vectorArray[1] = Vector2.Max(vectorArray[1], vector);
            }
            return vectorArray;
        }

        private void Init()
        {
            this.maskImage.enabled = false;
            this._image = base.GetComponent<ImageForSmoothMask>();
            this.CreateMaterial();
            this._material.SetTexture("_MaskTex", this.maskImage.mainTexture);
            this._image.material = this._material;
            this.CheckValid();
            if (this.maskSlider != null)
            {
                this.maskSlider.onValueChanged = (Action<float, float>) Delegate.Combine(this.maskSlider.onValueChanged, new Action<float, float>(this.OnMaskSliderValueChanged));
            }
            this.ResetRender();
        }

        private void OnDestroy()
        {
            if (this._material != null)
            {
                UnityEngine.Object.DestroyImmediate(this._material);
            }
            this.maskShader = null;
            if (this.maskImage != null)
            {
                this.maskImage.sprite = null;
            }
            this.maskImage = null;
            this.maskSlider = null;
        }

        private void OnMaskSliderValueChanged(float fromRatio, float toRatio)
        {
            if (fromRatio != toRatio)
            {
                this.coverRatio = toRatio;
                this.ResetRender();
            }
        }

        private void ResetRender()
        {
            this._image.SetAllDirty();
        }

        private class Quad
        {
            public Vector2 max;
            public Vector2 min;
            public Vector2 uvMax0;
            public Vector2 uvMax1;
            public Vector2 uvMin0;
            public Vector2 uvMin1;

            public Quad(ImageSmoothMask.Quad quad)
            {
                this.min = quad.min;
                this.max = quad.max;
                this.uvMin0 = quad.uvMin0;
                this.uvMax0 = quad.uvMax0;
                this.uvMin1 = quad.uvMin1;
                this.uvMax1 = quad.uvMax1;
            }

            public Quad(Vector2 min, Vector2 max, Vector2 uvMin0, Vector2 uvMax0, Vector2 uvMin1, Vector2 uvMax1)
            {
                this.min = min;
                this.max = max;
                this.uvMin0 = uvMin0;
                this.uvMax0 = uvMax0;
                this.uvMin1 = uvMin1;
                this.uvMax1 = uvMax1;
            }

            private ImageSmoothMask.Quad GetHigherPart(float splitPoint, float uv1, int dir)
            {
                splitPoint = Mathf.Max(this.min[dir], splitPoint);
                ImageSmoothMask.Quad quad = new ImageSmoothMask.Quad(this);
                quad.min[dir] = splitPoint;
                float t = (splitPoint - this.min[dir]) / (this.max[dir] - this.min[dir]);
                quad.uvMin0[dir] = Mathf.Lerp(this.uvMin0[dir], this.uvMax0[dir], t);
                quad.uvMin1[dir] = uv1;
                return quad;
            }

            private ImageSmoothMask.Quad GetLowerPart(float splitPoint, float uv1, int dir)
            {
                splitPoint = Mathf.Min(this.max[dir], splitPoint);
                ImageSmoothMask.Quad quad = new ImageSmoothMask.Quad(this);
                quad.max[dir] = splitPoint;
                float t = (splitPoint - this.min[dir]) / (this.max[dir] - this.min[dir]);
                quad.uvMax0[dir] = Mathf.Lerp(this.uvMin0[dir], this.uvMax0[dir], t);
                quad.uvMax1[dir] = uv1;
                return quad;
            }

            private ImageSmoothMask.Quad GetMiddlePart(float splitPoint1, float splitPoint2, int dir)
            {
                ImageSmoothMask.Quad quad = new ImageSmoothMask.Quad(this);
                quad.min[dir] = splitPoint1;
                quad.max[dir] = splitPoint2;
                float t = (splitPoint1 - this.min[dir]) / (this.max[dir] - this.min[dir]);
                float num2 = (splitPoint2 - this.min[dir]) / (this.max[dir] - this.min[dir]);
                quad.uvMin0[dir] = Mathf.Lerp(this.uvMin0[dir], this.uvMax0[dir], t);
                quad.uvMin1[dir] = Mathf.Lerp(this.uvMin1[dir], this.uvMax1[dir], t);
                quad.uvMax0[dir] = Mathf.Lerp(this.uvMin0[dir], this.uvMax0[dir], num2);
                quad.uvMax1[dir] = Mathf.Lerp(this.uvMin1[dir], this.uvMax1[dir], num2);
                return quad;
            }

            public ImageSmoothMask.Quad GridSplit(ImageSmoothMask.Quad another, List<ImageSmoothMask.Quad> unOverlappedList)
            {
                ImageSmoothMask.Quad quad = null;
                List<ImageSmoothMask.Quad> list = new List<ImageSmoothMask.Quad>();
                ImageSmoothMask.Quad item = this.Split(another, list, 0);
                if (item != null)
                {
                    quad = item.Split(another, unOverlappedList, 1);
                }
                foreach (ImageSmoothMask.Quad quad3 in list)
                {
                    item = quad3.Split(another, unOverlappedList, 1);
                    if (item != null)
                    {
                        unOverlappedList.Add(item);
                    }
                }
                return quad;
            }

            public ImageSmoothMask.Quad Split(ImageSmoothMask.Quad another, List<ImageSmoothMask.Quad> unOverlappedList = null)
            {
                ImageSmoothMask.Quad quad = null;
                ImageSmoothMask.Quad quad2 = this.Split(another, unOverlappedList, 0);
                if (quad2 != null)
                {
                    quad = quad2.Split(another, unOverlappedList, 1);
                }
                return quad;
            }

            private ImageSmoothMask.Quad Split(ImageSmoothMask.Quad another, List<ImageSmoothMask.Quad> unOverlappedList, int dir)
            {
                float splitPoint = Mathf.Max(this.min[dir], another.min[dir]);
                float num2 = Mathf.Min(this.max[dir], another.max[dir]);
                if ((this.min[dir] < splitPoint) && (unOverlappedList != null))
                {
                    unOverlappedList.Add(this.GetLowerPart(splitPoint, another.uvMin1[dir], dir));
                }
                if ((num2 < this.max[dir]) && (unOverlappedList != null))
                {
                    unOverlappedList.Add(this.GetHigherPart(num2, another.uvMax1[dir], dir));
                }
                ImageSmoothMask.Quad quad = null;
                if (splitPoint < num2)
                {
                    quad = this.GetMiddlePart(splitPoint, num2, dir);
                    quad.uvMin1[dir] = Mathf.Lerp(another.uvMin1[dir], another.uvMax1[dir], (splitPoint - another.min[dir]) / (another.max[dir] - another.min[dir]));
                    quad.uvMax1[dir] = Mathf.Lerp(another.uvMin1[dir], another.uvMax1[dir], (num2 - another.min[dir]) / (another.max[dir] - another.min[dir]));
                }
                return quad;
            }

            public UIVertex[] ToUIQuad(UIVertex template)
            {
                UIVertex[] vertexArray = new UIVertex[] { template, template, template, template };
                vertexArray[0].position = new Vector3(this.min.x, this.min.y, 0f);
                vertexArray[1].position = new Vector3(this.min.x, this.max.y, 0f);
                vertexArray[2].position = new Vector3(this.max.x, this.max.y, 0f);
                vertexArray[3].position = new Vector3(this.max.x, this.min.y, 0f);
                vertexArray[0].uv0 = new Vector3(this.uvMin0.x, this.uvMin0.y, 0f);
                vertexArray[1].uv0 = new Vector3(this.uvMin0.x, this.uvMax0.y, 0f);
                vertexArray[2].uv0 = new Vector3(this.uvMax0.x, this.uvMax0.y, 0f);
                vertexArray[3].uv0 = new Vector3(this.uvMax0.x, this.uvMin0.y, 0f);
                vertexArray[0].uv1 = new Vector3(this.uvMin1.x, this.uvMin1.y, 0f);
                vertexArray[1].uv1 = new Vector3(this.uvMin1.x, this.uvMax1.y, 0f);
                vertexArray[2].uv1 = new Vector3(this.uvMax1.x, this.uvMax1.y, 0f);
                vertexArray[3].uv1 = new Vector3(this.uvMax1.x, this.uvMin1.y, 0f);
                return vertexArray;
            }
        }
    }
}

