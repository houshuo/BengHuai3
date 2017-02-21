namespace MoleMole
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class UVSheetAnimation : MonoBehaviour
    {
        private Material _material;
        private int _selectedTotalFrames = 1;
        private float _step_X;
        private float _step_Y;
        private float _time;
        private int _totalFrames = 1;
        [Tooltip("If do interpolation between frames")]
        public bool doInterpolation;
        public AnimationCurve frameOverTime = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public bool isLoop = true;
        [Tooltip("The material id of this renderer to apply on")]
        public int materialId;
        [Tooltip("If need to scale uv to fit a frame")]
        public bool needScale;
        [Header("Manual Mode")]
        public Vector2[] offsetList;
        [Header("Common Option")]
        public OffsetMode offsetMode;
        public float playbackSpeed = 1f;
        [Tooltip("Only select tiles in this list")]
        public int[] tileFilterList;
        [Header("Auto Mode")]
        public int tiles_X = 1;
        public int tiles_Y = 1;

        private Vector2 OffsetOfFrame(int frame)
        {
            if (this.offsetMode == OffsetMode.Auto)
            {
                Vector2 vector = new Vector2();
                frame = frame % this._selectedTotalFrames;
                frame = this.tileFilterList[frame];
                vector.x = (frame % this.tiles_X) * this._step_X;
                vector.y = ((this.tiles_Y - 1) - (frame / this.tiles_X)) * this._step_Y;
                return vector;
            }
            frame = frame % this.offsetList.Length;
            return this.offsetList[frame];
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            this.Preparation();
        }

        private void Play(float time)
        {
            time = Mathf.Clamp01(time);
            float num = time * this._selectedTotalFrames;
            int frame = (int) num;
            if (this.needScale)
            {
                this._material.SetTextureScale("_MainTex", new Vector2(this._step_X, this._step_Y));
            }
            Vector2 offset = this.OffsetOfFrame(frame);
            this._material.SetTextureOffset("_MainTex", offset);
            if (this.doInterpolation)
            {
                this._material.SetVector("_nextFrameOffset", this.OffsetOfFrame(frame + 1) - offset);
                this._material.SetFloat("_frameInterpolationFactor", num - frame);
            }
            else
            {
                this._material.SetVector("_nextFrameOffset", offset);
                this._material.SetFloat("_frameInterpolationFactor", 0f);
            }
        }

        private void Preparation()
        {
            Material[] materials = base.GetComponent<Renderer>().materials;
            this._material = materials[this.materialId];
            this._totalFrames = this.tiles_X * this.tiles_Y;
            this._step_X = 1f / ((float) this.tiles_X);
            this._step_Y = 1f / ((float) this.tiles_Y);
            if (this.isLoop)
            {
                this.frameOverTime.preWrapMode = WrapMode.Loop;
                this.frameOverTime.postWrapMode = WrapMode.Loop;
            }
            if (this.tileFilterList.Length == 0)
            {
                this.tileFilterList = new int[this._totalFrames];
                for (int i = 0; i < this._totalFrames; i++)
                {
                    this.tileFilterList[i] = i;
                }
            }
            this._selectedTotalFrames = this.tileFilterList.Length;
        }

        private void Start()
        {
            this.Preparation();
        }

        public void Update()
        {
            if (Application.isPlaying)
            {
                this._time += Time.deltaTime * this.playbackSpeed;
            }
            else
            {
                this._time += 0.01666667f * this.playbackSpeed;
            }
            if (!this.isLoop && (this._time > 1f))
            {
                this._time = 1f;
            }
            float time = this.frameOverTime.Evaluate(this._time);
            this.Play(time);
        }

        public enum OffsetMode
        {
            Auto,
            Manual
        }
    }
}

