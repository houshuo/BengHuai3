namespace MoleMole
{
    using CinemaDirector;
    using System;
    using UnityEngine;

    public class AvatarCinema_Common : MonoBehaviour, ICinema
    {
        [SerializeField]
        private Transform _anchor;
        [SerializeField]
        private Transform _camera;
        [SerializeField]
        private CharacterTrackGroup _characterTrackGroup;
        [SerializeField]
        private Cutscene _cutScene;
        [SerializeField, Header("Init Near Z Plane, only positive value works")]
        private float _initClipZNear = -1f;
        [SerializeField, Header("Init FOV, only positive value works")]
        private float _initFov = -1f;
        private MonoMainCamera _mainCamera;
        private bool _shouldStop;

        private void Awake()
        {
            this._camera.GetComponent<Camera>().enabled = false;
        }

        private void CutsceneFinished(object sender, CutsceneEventArgs e)
        {
            this._shouldStop = true;
            Cutscene cutScene = sender as Cutscene;
            cutScene.CutsceneFinished -= new CutsceneHandler(this.CutsceneFinished);
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            Singleton<EventManager>.Instance.FireEvent(new EvtCinemaFinish(runtimeID, cutScene), MPEventDispatchMode.Normal);
            this._mainCamera.TransitToFollow();
        }

        public Transform GetCameraTransform()
        {
            if (this._cutScene.State == Cutscene.CutsceneState.Playing)
            {
                return this._camera;
            }
            return null;
        }

        public Cutscene GetCutscene()
        {
            return this._cutScene;
        }

        public float GetInitCameraClipZNear()
        {
            return this._initClipZNear;
        }

        public float GetInitCameraFOV()
        {
            return this._initFov;
        }

        public void Init(Transform actor)
        {
            this._anchor.parent = actor;
            this._anchor.localPosition = Vector3.zero;
            this._anchor.localRotation = Quaternion.identity;
            this._characterTrackGroup.Actor = actor;
            this._cutScene.CutsceneFinished += new CutsceneHandler(this.CutsceneFinished);
            this._mainCamera = Singleton<CameraManager>.Instance.GetMainCamera();
        }

        public bool IsShouldStop()
        {
            return this._shouldStop;
        }

        public void Play()
        {
            this._mainCamera.TransitToCinema(this);
            this._cutScene.Play();
        }

        private void ReceiveMessage(string messageID)
        {
            uint runtimeID = Singleton<AvatarManager>.Instance.GetLocalAvatar().GetRuntimeID();
            Singleton<EventManager>.Instance.FireEvent(new EvtCinemaReceiveMessage(runtimeID, this._cutScene, messageID), MPEventDispatchMode.Normal);
            if (messageID == "CloseToEnd")
            {
                this._shouldStop = true;
                this._cutScene.Pause();
            }
        }

        private void Start()
        {
        }

        private void Update()
        {
        }
    }
}

