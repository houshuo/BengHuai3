namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoEffectPluginUI : BaseMonoEffectPlugin
    {
        private Vector3 _initPosition;
        public float depthInMainCamera;
        [Header("UI transform path, if empty use InitPos")]
        public string FollowTargetUIPath;
        public bool UseParentUIPos;

        protected override void Awake()
        {
            base.Awake();
        }

        public override bool IsToBeRemove()
        {
            return false;
        }

        public void LateUpdate()
        {
            Vector3 one = Vector3.one;
            if (!string.IsNullOrEmpty(this.FollowTargetUIPath))
            {
                BaseMonoCanvas sceneCanvas = Singleton<MainUIManager>.Instance.SceneCanvas;
                if (sceneCanvas == null)
                {
                    return;
                }
                one = sceneCanvas.transform.FindChild(this.FollowTargetUIPath).position;
            }
            else if (this.UseParentUIPos)
            {
                Transform parent = base.transform.parent;
                if ((parent != null) && (parent.gameObject.layer == LayerMask.NameToLayer("UI")))
                {
                    one = parent.position;
                }
            }
            else
            {
                one = this._initPosition;
            }
            Camera cameraComponent = Singleton<CameraManager>.Instance.GetMainCamera().cameraComponent;
            Vector3 position = Singleton<CameraManager>.Instance.GetInLevelUICamera().gameObject.GetComponent<Camera>().WorldToScreenPoint(one);
            position.z = this.depthInMainCamera;
            base._effect.gameObject.transform.position = cameraComponent.ScreenToWorldPoint(position);
        }

        public override void SetDestroy()
        {
        }

        public override void Setup()
        {
            this._initPosition = base.transform.position;
        }
    }
}

