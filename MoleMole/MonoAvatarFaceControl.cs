namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoAvatarFaceControl : MonoBehaviour
    {
        private int _currentFaceIndex = -1;
        private Texture2D _currentTexture;
        public int defaultFaceFrameIndex;
        public FaceFrame[] faceFrames;
        public Renderer faceRenderer;
        public FaceTextureItem[] faceTextureItems;
        public float targetFaceIndex;
        public bool useUpdateFaceIndex;

        public int GetFaceCount()
        {
            return this.faceFrames.Length;
        }

        public bool SetFace(int frameIndex)
        {
            if (frameIndex != this._currentFaceIndex)
            {
                if (this.faceRenderer == null)
                {
                    Debug.LogError("[GalTouch] face renderer not set : " + base.gameObject.name);
                    return false;
                }
                if ((frameIndex < 0) || (frameIndex >= this.faceFrames.Length))
                {
                    Debug.LogError(string.Format("[GalTouch] face frame index({0}) out of range : {1}", frameIndex.ToString(), base.gameObject.name));
                    return false;
                }
                FaceFrame frame = this.faceFrames[frameIndex];
                if ((frame.textureItemIndex < 0) || (frame.textureItemIndex >= this.faceTextureItems.Length))
                {
                    Debug.LogError(string.Format("[GalTouch] face texture item index({0}) out of range : {1}", frame.textureItemIndex.ToString(), base.gameObject.name));
                    return false;
                }
                FaceTextureItem item = this.faceTextureItems[frame.textureItemIndex];
                if ((frame.frameIndex < 0) || (frame.frameIndex >= (item.row * item.row)))
                {
                    Debug.LogError("[GalTouch] face texture frame index out of range : " + base.gameObject.name);
                    return false;
                }
                if (item.row <= 0)
                {
                    Debug.LogError("[GalTouch] texture item row illegal : " + base.gameObject.name);
                    return false;
                }
                this.SetFaceTextureAndUV(item.texture, item.row, frame.frameIndex);
                this._currentFaceIndex = frameIndex;
            }
            return true;
        }

        private void SetFaceTextureAndUV(Texture2D texture, int row, int index)
        {
            Material material = this.faceRenderer.material;
            if (this._currentTexture != texture)
            {
                this._currentTexture = texture;
                material.mainTexture = texture;
            }
            int num = index / row;
            int num2 = index % row;
            float x = 1f / ((float) row);
            material.mainTextureScale = new Vector2(x, x);
            material.mainTextureOffset = new Vector2(num * x, num2 * x);
        }

        private void Start()
        {
            this.SetFace(this.defaultFaceFrameIndex);
        }

        private void Update()
        {
            if (this.useUpdateFaceIndex && (this._currentFaceIndex != ((int) this.targetFaceIndex)))
            {
                this.SetFace((int) this.targetFaceIndex);
            }
        }
    }
}

