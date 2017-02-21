namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class MonoElevatorProjectiveLight : MonoBehaviour
    {
        private Vector2 _duvdxy;
        private List<List<int>> _groups;
        private Mesh _mesh;
        private MeshFilter _meshFilter;
        private MaterialPropertyBlock _mpb;
        private Vector2[] _originaluvs;
        private float _pivotX;
        private float[] _posInProjectSpace;
        private Renderer _renderer;
        private Mesh _sharedMesh;
        private Matrix4x4 _tbnMat = new Matrix4x4();
        private Matrix4x4 _tbnMat_inv = new Matrix4x4();
        private float _timer;
        private Vector2[] _uvs;
        private Vector3[] _vertices;
        private Vector3[] _verticesInTangentSpace;
        [Header("True if light source at the right light sheet; false if at the right")]
        public bool isLightSourceAtRight;
        [Range(0f, 60f)]
        public float lightSourceCycleTime;
        [Header("Distance between light source and the left or right edge of the projective light sheet (object space)")]
        public float lightSourceDist;
        public float lightSourceHeigthEnd;
        [Header("Light source animation")]
        public float lightSourceHeigthStart;
        public AnimationCurve lightSourceMovementCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [Range(0f, 1f)]
        public float lightSourceMovementPhase;
        public AnimationCurve lightStrenthCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float lightStrenthCurveScale = 1f;
        public bool pause;
        [Header("Height of bottom of the projective obj")]
        public float projObjBottomY;
        [Header("Distance between projected object and the left of right edge of the projective light sheet (object space)")]
        public float projObjDist;
        [Header("Height of top of the projective obj")]
        public float projObjTopY;

        private void OnDestroy()
        {
            this.Release();
        }

        private void OnDisable()
        {
            this._meshFilter.mesh = this._sharedMesh;
            this.Release();
        }

        private void OnEnable()
        {
            this._meshFilter = base.GetComponent<MeshFilter>();
            this._mesh = this._meshFilter.mesh;
            this._vertices = this._mesh.vertices;
            this._originaluvs = this._mesh.uv;
            this._uvs = new Vector2[this._originaluvs.Length];
            this._renderer = base.GetComponent<Renderer>();
            this._mpb = new MaterialPropertyBlock();
            this._renderer.GetPropertyBlock(this._mpb);
            this._verticesInTangentSpace = this._mesh.vertices;
            Vector3 rhs = this._mesh.normals[0];
            Vector3 lhs = new Vector3(0f, 1f, 0f);
            Vector3 v = Vector3.Cross(lhs, rhs);
            this._tbnMat = new Matrix4x4();
            this._tbnMat.SetRow(0, v);
            this._tbnMat.SetRow(1, lhs);
            this._tbnMat.SetRow(2, rhs);
            this._tbnMat.m33 = 1f;
            this._tbnMat_inv = this._tbnMat.inverse;
            for (int i = 0; i < this._mesh.vertexCount; i++)
            {
                this._verticesInTangentSpace[i] = this._tbnMat.MultiplyPoint(this._verticesInTangentSpace[i]);
            }
            this._groups = new List<List<int>>();
            List<float> list = new List<float>();
            List<float> list2 = new List<float>();
            float num2 = 0.01f;
            for (int j = 0; j < this._mesh.vertexCount; j++)
            {
                bool flag = false;
                for (int num4 = 0; num4 < this._groups.Count; num4++)
                {
                    List<int> list3 = this._groups[num4];
                    if (Mathf.Abs((float) (this._verticesInTangentSpace[list3[0]].x - this._verticesInTangentSpace[j].x)) < num2)
                    {
                        flag = true;
                        list3.Add(j);
                        list[num4] = Mathf.Min(list[num4], this._verticesInTangentSpace[j].y);
                        list2[num4] = Mathf.Max(list2[num4], this._verticesInTangentSpace[j].y);
                        break;
                    }
                }
                if (!flag)
                {
                    List<int> list4;
                    list4 = new List<int> {
                        j,
                        list4,
                        this._verticesInTangentSpace[j].y,
                        this._verticesInTangentSpace[j].y
                    };
                }
            }
            this._posInProjectSpace = new float[this._mesh.vertexCount];
            for (int k = 0; k < this._groups.Count; k++)
            {
                float num6 = list2[k] - list[k];
                List<int> list5 = this._groups[k];
                foreach (int num7 in list5)
                {
                    this._posInProjectSpace[num7] = (this._verticesInTangentSpace[num7].y - list[k]) / num6;
                }
            }
            float num8 = 0f;
            int index = -1;
            float num10 = 0f;
            int num11 = -1;
            for (int m = 1; m < (this._mesh.vertexCount - this._groups.Count); m++)
            {
                Vector3 vector4 = this._verticesInTangentSpace[m] - this._verticesInTangentSpace[0];
                if (Mathf.Abs(vector4.x) > num8)
                {
                    num8 = Mathf.Abs(vector4.x);
                    index = m;
                }
                if (Mathf.Abs(vector4.y) > num10)
                {
                    num10 = Mathf.Abs(vector4.y);
                    num11 = m;
                }
            }
            this._duvdxy = new Vector2();
            this._duvdxy.x = (this._originaluvs[0].x - this._originaluvs[index].x) / (this._verticesInTangentSpace[0].x - this._verticesInTangentSpace[index].x);
            this._duvdxy.y = (this._originaluvs[0].y - this._originaluvs[num11].y) / (this._verticesInTangentSpace[0].y - this._verticesInTangentSpace[num11].y);
            for (int n = 0; n < this._groups.Count; n++)
            {
                List<int> list6 = this._groups[n];
                int num14 = list6[list6.Count - 1];
                int num15 = list6[list6.Count - 2];
                this._originaluvs[num14].y = this._originaluvs[num15].y + ((this._verticesInTangentSpace[num14].y - this._verticesInTangentSpace[num15].y) * this._duvdxy.y);
            }
            this._pivotX = !this.isLightSourceAtRight ? ((float) 0x1869f) : ((float) (-99999));
            foreach (List<int> list7 in this._groups)
            {
                int num16 = list7[0];
                if (this.isLightSourceAtRight)
                {
                    this._pivotX = Mathf.Max(this._pivotX, this._verticesInTangentSpace[num16].x);
                }
                else
                {
                    this._pivotX = Mathf.Min(this._pivotX, this._verticesInTangentSpace[num16].x);
                }
            }
        }

        private void Release()
        {
            if (this._mesh != null)
            {
                UnityEngine.Object.Destroy(this._mesh);
            }
        }

        private void Update()
        {
            float num = !this.isLightSourceAtRight ? (this._pivotX + this.projObjDist) : (this._pivotX - this.projObjDist);
            float num2 = !this.isLightSourceAtRight ? (this._pivotX + this.lightSourceDist) : (this._pivotX - this.lightSourceDist);
            this._timer += Time.deltaTime;
            float f = this._timer / this.lightSourceCycleTime;
            f += this.lightSourceMovementPhase;
            f -= Mathf.FloorToInt(f);
            float t = this.lightSourceMovementCurve.Evaluate(f);
            float num5 = Mathf.Lerp(this.lightSourceHeigthStart, this.lightSourceHeigthEnd, t);
            float num6 = this.lightStrenthCurve.Evaluate(f) * this.lightStrenthCurveScale;
            this._mpb.SetFloat("_LightStrength", num6);
            this._renderer.SetPropertyBlock(this._mpb);
            for (int i = 0; i < this._groups.Count; i++)
            {
                List<int> list = this._groups[i];
                float x = this._verticesInTangentSpace[list[0]].x;
                float num9 = num5 + (((this.projObjTopY - num5) * (x - num2)) / (num - num2));
                float num10 = num5 + (((this.projObjBottomY - num5) * (x - num2)) / (num - num2));
                for (int j = 0; j < list.Count; j++)
                {
                    int index = list[j];
                    Vector3 v = this._verticesInTangentSpace[index];
                    v.y = num10 + ((num9 - num10) * this._posInProjectSpace[index]);
                    this._vertices[index] = this._tbnMat_inv.MultiplyPoint(v);
                    Vector3 vector2 = v - this._verticesInTangentSpace[index];
                    this._uvs[index].x = this._originaluvs[index].x + (vector2.x * this._duvdxy.x);
                    this._uvs[index].y = this._originaluvs[index].y + (vector2.y * this._duvdxy.y);
                }
            }
            if (!this.pause)
            {
                this._mesh.vertices = this._vertices;
                this._mesh.uv = this._uvs;
            }
        }
    }
}

