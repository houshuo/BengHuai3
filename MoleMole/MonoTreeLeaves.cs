namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class MonoTreeLeaves : MonoBehaviour
    {
        private Vector3[] _originalLocalPositions;
        private static readonly string TRUNK_PREFIX = "Trunk";
        public float ZOffsetStep = 0.001f;

        private void SetBilloardOffset(Mesh mesh, Transform trsf)
        {
            Vector3[] vertices = mesh.vertices;
            if (vertices.Length != 0)
            {
                mesh.RecalculateNormals();
                Vector3 rhs = trsf.TransformDirection(mesh.normals[0]);
                Vector3 normalized = Vector3.Cross(Vector3.up, rhs).normalized;
                Vector2[] vectorArray2 = new Vector2[vertices.Length];
                for (int i = 0; i < vertices.Length; i++)
                {
                    Vector3 lhs = trsf.TransformPoint(vertices[i]) - trsf.position;
                    float x = Vector3.Dot(lhs, normalized);
                    vectorArray2[i] = new Vector2(x, lhs.y);
                    vertices[i] = Vector3.zero;
                }
                mesh.vertices = vertices;
                mesh.uv2 = vectorArray2;
            }
        }

        private void Start()
        {
            List<MeshTransformPair> list = new List<MeshTransformPair>();
            foreach (MeshFilter filter in base.GetComponentsInChildren<MeshFilter>())
            {
                if (!filter.name.StartsWith(TRUNK_PREFIX))
                {
                    list.Add(new MeshTransformPair(filter.mesh, filter.transform));
                }
            }
            foreach (MeshTransformPair pair in list)
            {
                this.SetBilloardOffset(pair.mesh, pair.trsf);
            }
        }

        private class MeshTransformPair
        {
            public Mesh mesh;
            public Transform trsf;

            public MeshTransformPair(Mesh mesh, Transform trsf)
            {
                this.mesh = mesh;
                this.trsf = trsf;
            }
        }
    }
}

