namespace MoleMole
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public class ScreenRecorder : MonoBehaviour
    {
        private Camera _camera;
        private int _frameCount;
        private float _frameTime;
        [SerializeField]
        private bool _isRunning;
        private PostFX _postFX;
        private int _recordFrameCount;
        private bool _stateChanged;
        private float _timeScale;
        private int _totalFrameCount;
        [Range(1f, 4f)]
        public int decimateNumber = 1;
        public float duration = -1f;
        [Range(10f, 120f)]
        public int frameRate = 30;
        [HideInInspector]
        public string outputPath = "D:/tmp/record";
        [HideInInspector]
        public float sizeScale = 1f;
        public KeyCode startKey = KeyCode.Q;
        public float updateDeltaTime = 0.2f;
        public bool writeAlpha = true;

        private void Awake()
        {
        }

        public void EndRecord()
        {
            if (this._isRunning)
            {
                this._isRunning = false;
                this._stateChanged = true;
            }
        }

        private bool GetInput()
        {
            if (Input.GetKeyUp(this.startKey))
            {
                this._stateChanged = true;
                this._isRunning = !this._isRunning;
                return true;
            }
            return false;
        }

        private void InitRecord()
        {
            this._frameTime = 1f / ((float) this.frameRate);
            this._frameCount = 0;
            if (this.duration > 0f)
            {
                this._totalFrameCount = (int) (this.duration / this._frameTime);
            }
            else
            {
                this._totalFrameCount = -1;
            }
            this._recordFrameCount = 0;
            this._camera = Camera.main;
            this._postFX = this._camera.GetComponent<PostFX>();
            Time.captureFramerate = (int) (1f / this.updateDeltaTime);
            this._timeScale = this._frameTime / this.updateDeltaTime;
            Time.timeScale *= this._timeScale;
            if (!true)
            {
                this.EndRecord();
            }
        }

        [DebuggerHidden]
        private IEnumerator Record()
        {
            return new <Record>c__Iterator4F { <>f__this = this };
        }

        private void SaveImage(string fileName)
        {
            RenderTextureWrapper wrapper = GraphicsUtils.GetRenderTexture((int) (Screen.width * this.sizeScale), (int) (Screen.height * this.sizeScale), 0, RenderTextureFormat.ARGB32);
            bool writeAlpha = false;
            if (this._postFX != null)
            {
                writeAlpha = this._postFX.WriteAlpha;
                this._postFX.WriteAlpha = this.writeAlpha;
            }
            this._camera.targetTexture = (RenderTexture) wrapper;
            this._camera.Render();
            this._camera.targetTexture = null;
            if (this._postFX != null)
            {
                this._postFX.WriteAlpha = writeAlpha;
            }
            RenderTexture.active = (RenderTexture) wrapper;
            Texture2D textured = new Texture2D(wrapper.width, wrapper.height, TextureFormat.ARGB32, false);
            textured.ReadPixels(new Rect(0f, 0f, (float) wrapper.width, (float) wrapper.height), 0, 0);
            if (!this.writeAlpha || (this._postFX == null))
            {
                Color[] pixels = textured.GetPixels();
                for (int i = 0; i < pixels.Length; i++)
                {
                    pixels[i].a = 1f;
                }
                textured.SetPixels(pixels);
            }
            textured.Apply();
            byte[] bytes = textured.EncodeToPNG();
            File.WriteAllBytes(string.Format("{0}/{1}.png", this.outputPath, fileName), bytes);
            UnityEngine.Object.Destroy(textured);
            GraphicsUtils.ReleaseRenderTexture(wrapper);
        }

        public void StartRecord()
        {
            if (!this._isRunning)
            {
                this._isRunning = true;
                this._stateChanged = true;
            }
        }

        private void Update()
        {
            this.GetInput();
            if (this._isRunning)
            {
                if (this._stateChanged)
                {
                    this._stateChanged = false;
                    this.InitRecord();
                }
                else
                {
                    this._frameCount++;
                    if (((this._frameCount - 1) % this.decimateNumber) == 0)
                    {
                        this._recordFrameCount++;
                        base.StartCoroutine(this.Record());
                    }
                    if ((this._totalFrameCount > 0) && (this._frameCount > this._totalFrameCount))
                    {
                        this.EndRecord();
                    }
                }
            }
            else if (this._stateChanged)
            {
                Time.timeScale /= this._timeScale;
                this._timeScale = 1f;
                Time.captureFramerate = 0;
            }
        }

        [CompilerGenerated]
        private sealed class <Record>c__Iterator4F : IEnumerator, IDisposable, IEnumerator<object>
        {
            internal object $current;
            internal int $PC;
            internal ScreenRecorder <>f__this;
            internal string <fileName>__0;

            [DebuggerHidden]
            public void Dispose()
            {
                this.$PC = -1;
            }

            public bool MoveNext()
            {
                uint num = (uint) this.$PC;
                this.$PC = -1;
                switch (num)
                {
                    case 0:
                        this.$current = new WaitForEndOfFrame();
                        this.$PC = 1;
                        goto Label_0089;

                    case 1:
                        this.<fileName>__0 = string.Format("{0,8:D8}", this.<>f__this._recordFrameCount);
                        this.<>f__this.SaveImage(this.<fileName>__0);
                        this.$current = null;
                        this.$PC = 2;
                        goto Label_0089;

                    case 2:
                        this.$PC = -1;
                        break;
                }
                return false;
            Label_0089:
                return true;
            }

            [DebuggerHidden]
            public void Reset()
            {
                throw new NotSupportedException();
            }

            object IEnumerator<object>.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }

            object IEnumerator.Current
            {
                [DebuggerHidden]
                get
                {
                    return this.$current;
                }
            }
        }
    }
}

