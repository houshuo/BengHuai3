namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Paster))]
    public class ShadowPaster : MonoBehaviour
    {
        private Camera _innerCamera;
        private Transform _lightForwardTransform;
        private Paster _paster;
        private RenderTextureWrapper _renderTexture;
        private Transform _rootNodeTransform;
        public float CameraDistance = 2f;
        public Texture2D FallOffTex;
        public RenderTextureFormat format = RenderTextureFormat.RGB565;
        public float PitchAngle = 70f;
        [Header("Distance from projector and camera to the avatar")]
        public float ProjectorDistance;
        [Header("Setting for projector")]
        public UnityEngine.Shader ProjectorShader;
        [Header("Shader used to draw shadow")]
        public UnityEngine.Shader Shader;
        [Header("Setting for RenderTexture to which draw shadow")]
        public int size = 0x40;
        [Header("Size of projector and camera")]
        public float Size = 1.2f;

        private void OnDestroy()
        {
            GraphicsUtils.ReleaseRenderTexture(this._renderTexture);
        }

        private void SetTransform()
        {
            if (this._lightForwardTransform == null)
            {
                this._lightForwardTransform = Singleton<StageManager>.Instance.GetStageEnv().lightForwardTransform;
            }
            this._innerCamera.orthographicSize = this.Size;
            this._paster.Size = this.Size;
            this._paster.AspectRatio = this._innerCamera.aspect;
            base.transform.position = this._lightForwardTransform.position;
            base.transform.rotation = this._lightForwardTransform.rotation;
            Vector3 eulerAngles = base.transform.eulerAngles;
            eulerAngles[0] = this.PitchAngle;
            base.transform.eulerAngles = eulerAngles;
            Vector3 position = this._rootNodeTransform.position;
            base.transform.position = position - ((Vector3) (base.transform.forward * this.ProjectorDistance));
            this._innerCamera.transform.position = position - ((Vector3) (this._innerCamera.transform.forward * this.CameraDistance));
        }

        private void Start()
        {
            this._innerCamera = base.GetComponentInChildren<Camera>();
            this._innerCamera.enabled = true;
            this._innerCamera.SetReplacementShader(this.Shader, string.Empty);
            this._paster = base.GetComponent<Paster>();
            Material material = new Material(this.ProjectorShader);
            material.SetTexture("_FalloffTex", this.FallOffTex);
            this._paster.Material = material;
            this._renderTexture = GraphicsUtils.GetRenderTexture(this.size, this.size, 0, this.format);
            this._renderTexture.BindToCamera(this._innerCamera);
            this._paster.Material.SetTexture("_Cookie", (Texture) this._renderTexture);
            this._rootNodeTransform = base.GetComponentInParent<BaseMonoAnimatorEntity>().RootNode;
            this.SetTransform();
        }

        private void Update()
        {
            this.SetTransform();
        }
    }
}

