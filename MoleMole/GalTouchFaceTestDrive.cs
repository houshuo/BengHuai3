namespace MoleMole
{
    using MoleMole.Config;
    using System;
    using UnityEngine;

    public class GalTouchFaceTestDrive : MonoBehaviour
    {
        public FaceAnimation _faceAnimation;
        private string animationName = string.Empty;
        public Renderer leftEye;
        private FacePartControl leftEyeControl;
        private int leftEyeIndex;
        public TestMatInfoProvider leftEyeProvider;
        public Renderer mouth;
        private FacePartControl mouthControl;
        private int mouthIndex;
        public TestMatInfoProvider mouthProvider;
        public Renderer rightEye;
        private FacePartControl rightEyeControl;
        private int rightEyeIndex;
        public TestMatInfoProvider rightEyeProvider;

        private void Awake()
        {
            this._faceAnimation = new FaceAnimation();
            ConfigFaceAnimation config = Resources.Load<ConfigFaceAnimation>("FaceAnimation/Mei");
            this.leftEyeControl = new FacePartControl();
            this.leftEyeControl.Init(this.leftEyeProvider, this.leftEye);
            this.rightEyeControl = new FacePartControl();
            this.rightEyeControl.Init(this.rightEyeProvider, this.rightEye);
            this.mouthControl = new FacePartControl();
            this.mouthControl.Init(this.mouthProvider, this.mouth);
            this._faceAnimation.Setup(config, this.leftEyeControl, this.rightEyeControl, this.mouthControl);
        }

        private void OnGUI()
        {
            this.animationName = GUILayout.TextField(this.animationName, new GUILayoutOption[0]);
            if (GUILayout.Button("Play", new GUILayoutOption[0]))
            {
                this._faceAnimation.PlayFaceAnimation(this.animationName, FaceAnimationPlayMode.Normal);
            }
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Left : ", new GUILayoutOption[0]);
            int index = 0;
            int maxIndex = this.leftEyeControl.GetMaxIndex();
            while (index < maxIndex)
            {
                if (GUILayout.Button(index.ToString(), new GUILayoutOption[0]))
                {
                    this.leftEyeControl.SetFacePartIndex(index);
                }
                index++;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Right : ", new GUILayoutOption[0]);
            int num3 = 0;
            int num4 = this.rightEyeControl.GetMaxIndex();
            while (num3 < num4)
            {
                if (GUILayout.Button(num3.ToString(), new GUILayoutOption[0]))
                {
                    this.rightEyeControl.SetFacePartIndex(num3);
                }
                num3++;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal(new GUILayoutOption[0]);
            GUILayout.Label("Mouth : ", new GUILayoutOption[0]);
            int num5 = 0;
            int num6 = this.mouthControl.GetMaxIndex();
            while (num5 < num6)
            {
                if (GUILayout.Button(num5.ToString(), new GUILayoutOption[0]))
                {
                    this.mouthControl.SetFacePartIndex(num5);
                }
                num5++;
            }
            GUILayout.EndHorizontal();
        }

        private void Update()
        {
            this._faceAnimation.Process(Time.deltaTime);
        }
    }
}

