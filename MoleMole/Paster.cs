namespace MoleMole
{
    using System;
    using UnityEngine;

    public class Paster : MonoBehaviour
    {
        private float _groundHeight;
        protected UnityEngine.Material _material;
        private QuadPasterMesh _pasterMesh;
        private GameObject _pasterObj;
        protected Transform _pasterTrsf;
        private Transform _trsf;
        public float AspectRatio = 1f;
        public float FalloffEndDistance = 3f;
        public float FalloffStartDistance = 1f;
        public int LayerMask = 0x20000;
        public UnityEngine.Material Material;
        protected const float MAX_HEIGHT = 4f;
        protected const float MAX_RATIO = 1f;
        protected const float MIN_HEIGHT = 1f;
        protected const float MIN_RATIO = 0.4f;
        public float Size = 1f;

        private void CalcGroundHeight()
        {
            this._groundHeight = 0f;
        }

        private void OnDestroy()
        {
            UnityEngine.Object.Destroy(this._material);
        }

        private void OnEnable()
        {
            if (this._trsf != null)
            {
                this.CalcGroundHeight();
            }
        }

        protected virtual void Start()
        {
            this._trsf = base.transform;
            this._pasterMesh = new QuadPasterMesh(this.AspectRatio, 1f);
            this._pasterMesh.Transform(this._trsf);
            this._pasterMesh.ProjectorHorizontal();
            this._pasterObj = new GameObject();
            this._pasterObj.name = "Paster";
            this._pasterObj.AddComponent<MeshFilter>().sharedMesh = this._pasterMesh.getMesh();
            MeshRenderer renderer = this._pasterObj.AddComponent<MeshRenderer>();
            renderer.material = this.Material;
            this._material = renderer.material;
            this._pasterTrsf = this._pasterObj.transform;
            this._pasterTrsf.SetParent(this._trsf, false);
            this._pasterTrsf.localPosition = Vector3.zero;
            this._pasterTrsf.rotation = Quaternion.identity;
            this.CalcGroundHeight();
        }

        protected virtual void Update()
        {
            float num = this._trsf.position.y - this._groundHeight;
            this._pasterTrsf.position = this._trsf.position - ((Vector3) ((this._trsf.forward * num) / this._trsf.forward.y));
            this._pasterTrsf.localScale = (Vector3) (Vector3.one * this.Size);
            float num2 = Mathf.Clamp01(1f - ((num - this.FalloffStartDistance) / this.FalloffEndDistance));
            this._material.SetFloat(InLevelData.SHADER_FALLOFF, num2);
        }

        public UnityEngine.Material PasterMaterial
        {
            get
            {
                return this._material;
            }
        }

        private class QuadPasterMesh
        {
            private Vector3 dir = Vector3.forward;
            private Vector3[] offsets = new Vector3[4];
            private int[] tris;
            private Vector2[] uvs = new Vector2[4];

            public QuadPasterMesh(float aspect, float size)
            {
                float y = size;
                float x = y * aspect;
                this.offsets[0] = new Vector3(-x, y);
                this.uvs[0] = new Vector2(0f, 1f);
                this.offsets[1] = new Vector3(x, y);
                this.uvs[1] = new Vector2(1f, 1f);
                this.offsets[2] = new Vector3(-x, -y);
                this.uvs[2] = new Vector2(0f, 0f);
                this.offsets[3] = new Vector3(x, -y);
                this.uvs[3] = new Vector2(1f, 0f);
                this.tris = new int[] { 0, 3, 2, 0, 1, 3 };
            }

            public Mesh getMesh()
            {
                return new Mesh { vertices = this.offsets, uv = this.uvs, triangles = this.tris };
            }

            public void ProjectorHorizontal()
            {
                for (int i = 0; i < 4; i++)
                {
                    this.offsets[i] = this.ProjectPoint(this.offsets[i]);
                }
            }

            private Vector3 ProjectPoint(Vector3 point)
            {
                return (point - ((Vector3) ((this.dir * point.y) / this.dir.y)));
            }

            public void Transform(UnityEngine.Transform tranform)
            {
                this.dir = tranform.TransformDirection(this.dir);
                for (int i = 0; i < 4; i++)
                {
                    this.offsets[i] = tranform.TransformPoint(this.offsets[i]);
                    this.offsets[i] -= tranform.position;
                }
            }
        }
    }
}

