namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class FaceAnimationEditAvatar : MonoBehaviour
    {
        private Animator _animator;
        private ConfigFaceAnimation _config;
        private FaceAnimation _faceAnimation = new FaceAnimation();
        private FacePartControl _leftEye = new FacePartControl();
        private FacePartControl _mouth = new FacePartControl();
        private FacePartControl _rightEye = new FacePartControl();
        public int avatarId = 0x65;
        public AtlasMatInfoProvider eyeProviderAtlas;
        public int heartLevel = 1;
        public TestMatInfoProvider leftEyeProvider;
        public Renderer leftEyeRenderer;
        public TestMatInfoProvider mouthProvider;
        public AtlasMatInfoProvider mouthProviderAtlas;
        public Renderer mouthRenderer;
        public TestMatInfoProvider rightEyeProvider;
        public Renderer rightEyeRenderer;
        public bool useAtlas;

        private void Awake()
        {
            this._animator = base.GetComponent<Animator>();
            if (this.useAtlas)
            {
                this._leftEye.Init(this.eyeProviderAtlas, this.leftEyeRenderer);
                this._rightEye.Init(this.eyeProviderAtlas, this.rightEyeRenderer);
                this._mouth.Init(this.mouthProviderAtlas, this.mouthRenderer);
            }
            else
            {
                this._leftEye.Init(this.leftEyeProvider, this.leftEyeRenderer);
                this._rightEye.Init(this.rightEyeProvider, this.rightEyeRenderer);
                this._mouth.Init(this.mouthProvider, this.mouthRenderer);
            }
        }

        public void PlayBodyAnimation(string name)
        {
            if (this._animator != null)
            {
                this._animator.Play(name);
            }
        }

        public void PlayFaceAnimation(string name)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.PlayFaceAnimation(name, FaceAnimationPlayMode.Normal);
            }
        }

        public void PrepareFaceAnimation(string name)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.PrepareFaceAnmation(name);
            }
        }

        public void RebuildFaceAnimation()
        {
            this._faceAnimation.Setup(this._config, this._leftEye, this._rightEye, this._mouth);
        }

        public void SetAnimationTime(float time)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.SetTime(time);
            }
        }

        public void SetAnimationTimePerFrame(float time)
        {
            if (this._faceAnimation != null)
            {
                this._faceAnimation.SetTimePerFrame(time);
            }
        }

        public void SetLeftEyeImage(int index)
        {
            if (this._leftEye != null)
            {
                this._leftEye.SetFacePartIndex(index);
            }
        }

        public void SetMouthImage(int index)
        {
            if (this._mouth != null)
            {
                this._mouth.SetFacePartIndex(index);
            }
        }

        public void SetRightEyeImage(int index)
        {
            if (this._rightEye != null)
            {
                this._rightEye.SetFacePartIndex(index);
            }
        }

        public void SetupFaceAnimation(ConfigFaceAnimation config)
        {
            this._faceAnimation.Setup(config, this._leftEye, this._rightEye, this._mouth);
            this._config = config;
        }

        public void TriggerAudioPattern(string name)
        {
            Singleton<WwiseAudioManager>.Instance.Post(name, null, null, null);
        }
    }
}

