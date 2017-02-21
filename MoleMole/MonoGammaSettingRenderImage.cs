namespace MoleMole
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(RawImage))]
    public class MonoGammaSettingRenderImage : MonoBehaviour
    {
        private RenderTextureWrapper _renderTexture;

        private void DoSetupRenderTexture()
        {
            if (this._renderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._renderTexture);
                this._renderTexture = null;
            }
            base.transform.GetComponent<RawImage>().enabled = true;
            float scaleFactor = 1f;
            Canvas component = Singleton<MainUIManager>.Instance.SceneCanvas.GetComponent<Canvas>();
            if ((component != null) && (component.renderMode != RenderMode.WorldSpace))
            {
                scaleFactor = component.scaleFactor;
            }
            float width = ((RectTransform) base.transform).rect.width;
            float height = ((RectTransform) base.transform).rect.height;
            this._renderTexture = GraphicsUtils.GetRenderTexture((int) (width * scaleFactor), (int) (height * scaleFactor), 0x18, RenderTextureFormat.ARGB32);
            this._renderTexture.content.filterMode = FilterMode.Point;
            base.transform.GetComponent<RawImage>().texture = (Texture) this._renderTexture;
            GameObject obj2 = GameObject.Find("MainCamera");
            if (obj2 != null)
            {
                this._renderTexture.BindToCamera(obj2.GetComponent<Camera>());
            }
        }

        public void OnDestroy()
        {
            this.ReleaseRenderTexture();
        }

        public void ReleaseRenderTexture()
        {
            if (this._renderTexture != null)
            {
                GraphicsUtils.ReleaseRenderTexture(this._renderTexture);
                this._renderTexture = null;
                GameObject obj2 = GameObject.Find("MainCamera");
                if (obj2 != null)
                {
                    obj2.GetComponent<Camera>().targetTexture = null;
                }
            }
        }

        public void SetupView()
        {
            this.DoSetupRenderTexture();
        }
    }
}

