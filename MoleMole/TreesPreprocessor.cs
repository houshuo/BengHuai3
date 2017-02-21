namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class TreesPreprocessor
    {
        private int[][] _branchVrtxIds;
        private Mesh[] _leafMeshes;
        private Transform[] _leafTransforms;
        private GameObject _leavesRootObj;
        private int[][] _rootVrtxIds;
        private int[][] _truncVrtxIds;
        private Mesh _trunksMesh;
        private GameObject _trunksObj;
        private Transform _trunksTransform;

        public TreesPreprocessor(GameObject trunksObj, GameObject leavesRootObj)
        {
            this._trunksObj = trunksObj;
            this._leavesRootObj = leavesRootObj;
        }

        private bool CollectMesh(bool collectShared)
        {
            if ((this._trunksObj == null) || (this._leavesRootObj == null))
            {
                return false;
            }
            MeshFilter component = this._trunksObj.GetComponent<MeshFilter>();
            if (collectShared)
            {
                this._trunksMesh = component.sharedMesh;
            }
            else
            {
                this._trunksMesh = component.mesh;
            }
            this._trunksTransform = component.transform;
            List<Mesh> list = new List<Mesh>();
            List<Transform> list2 = new List<Transform>();
            foreach (MeshFilter filter2 in this._leavesRootObj.GetComponentsInChildren<MeshFilter>())
            {
                if (filter2.name.StartsWith("Leaf"))
                {
                    if (collectShared)
                    {
                        list.Add(filter2.sharedMesh);
                    }
                    else
                    {
                        list.Add(filter2.mesh);
                    }
                    list2.Add(filter2.transform);
                }
            }
            this._leafMeshes = list.ToArray();
            this._leafTransforms = list2.ToArray();
            return true;
        }

        private void DistinguishRootsAndBranches()
        {
            this._truncVrtxIds = this.SplitObjs(this._trunksMesh);
            Vector3[] vertices = this._trunksMesh.vertices;
            List<int[]> list = new List<int[]>();
            List<int[]> list2 = new List<int[]>();
            foreach (int[] numArray in this._truncVrtxIds)
            {
                Bounds bounds = this.GetBounds(vertices, numArray);
                if ((bounds.size.y / bounds.size.magnitude) > 0.94)
                {
                    list.Add(numArray);
                }
                else
                {
                    list2.Add(numArray);
                }
            }
            this._rootVrtxIds = list.ToArray();
            this._branchVrtxIds = list2.ToArray();
        }

        private Bounds GetBounds(Vector3[] vertices, int[] vids)
        {
            Vector3 lhs = (Vector3) (Vector3.one * 3.402823E+38f);
            Vector3 vector2 = (Vector3) (Vector3.one * -3.402823E+38f);
            foreach (int num in vids)
            {
                lhs = Vector3.Min(lhs, vertices[num]);
                vector2 = Vector3.Max(vector2, vertices[num]);
            }
            return new Bounds((Vector3) ((lhs + vector2) * 0.5f), vector2 - lhs);
        }

        private void GetConnectedGraph(int id, List<int>[] edges, bool[] accTable, List<int> vertices)
        {
            if (!accTable[id])
            {
                vertices.Add(id);
                accTable[id] = true;
                foreach (int num in edges[id])
                {
                    this.GetConnectedGraph(num, edges, accTable, vertices);
                }
            }
        }

        private Vector3 GetLowestVertex(Vector3[] vertices, int[] vids)
        {
            Vector3 vector = vertices[vids[0]];
            foreach (int num in vids)
            {
                if (vector.y > vertices[num].y)
                {
                    vector = vertices[num];
                }
            }
            return vector;
        }

        public void Process()
        {
            if (this.CollectMesh(false))
            {
                this.DistinguishRootsAndBranches();
                this.SetTrunks();
                this.SetLeaves();
            }
        }

        private void SetLeafRoot(Mesh mesh, Transform trsf)
        {
            Vector3 zero = Vector3.zero;
            Vector3[] vertices = mesh.vertices;
            foreach (Vector3 vector2 in vertices)
            {
                zero += vector2;
            }
            zero = (Vector3) (zero / ((float) vertices.Length));
            zero = trsf.TransformPoint(zero);
            float[] numArray = new float[this._branchVrtxIds.Length];
            Vector3[] vectorArray3 = this._trunksMesh.vertices;
            for (int i = 0; i < vectorArray3.Length; i++)
            {
                vectorArray3[i] = this._trunksTransform.TransformPoint(vectorArray3[i]);
            }
            int index = 0;
            for (int j = 0; j < numArray.Length; j++)
            {
                numArray[j] = 0f;
                int[] numArray2 = this._branchVrtxIds[j];
                for (int m = 0; m < numArray2.Length; m++)
                {
                    Vector3 vector4 = zero - vectorArray3[numArray2[m]];
                    numArray[j] += 1f / Mathf.Pow(vector4.magnitude + 0.1f, 2f);
                }
                numArray[j] /= (float) numArray2.Length;
                if (numArray[index] < numArray[j])
                {
                    index = j;
                }
            }
            Vector3 lowestVertex = this.GetLowestVertex(vectorArray3, this._branchVrtxIds[index]);
            lowestVertex = trsf.InverseTransformVector(lowestVertex);
            Vector4[] vectorArray4 = new Vector4[mesh.vertexCount];
            for (int k = 0; k < vectorArray4.Length; k++)
            {
                vectorArray4[k] = new Vector4(lowestVertex.x, lowestVertex.y, lowestVertex.z, 0f);
            }
            mesh.tangents = vectorArray4;
        }

        private void SetLeaves()
        {
            for (int i = 0; i < this._leafMeshes.Length; i++)
            {
                this.SetLeafRoot(this._leafMeshes[i], this._leafTransforms[i]);
            }
        }

        private void SetTrunks()
        {
            Vector3[] vertices = this._trunksMesh.vertices;
            Vector4[] vectorArray2 = new Vector4[this._trunksMesh.vertexCount];
            foreach (int[] numArray in this._rootVrtxIds)
            {
                Vector3 lowestVertex = this.GetLowestVertex(vertices, numArray);
                Vector4 vector2 = new Vector4(lowestVertex.x, lowestVertex.y, lowestVertex.z, 0f);
                foreach (int num2 in numArray)
                {
                    vectorArray2[num2] = vector2;
                }
            }
            foreach (int[] numArray4 in this._branchVrtxIds)
            {
                Vector3 vector3 = this.GetLowestVertex(vertices, numArray4);
                Vector4 vector4 = new Vector4(vector3.x, vector3.y, vector3.z, 1f);
                foreach (int num5 in numArray4)
                {
                    vectorArray2[num5] = vector4;
                }
            }
            this._trunksMesh.tangents = vectorArray2;
        }

        private int[][] SplitObjs(Mesh mesh)
        {
            List<int[]> list = new List<int[]>();
            bool[] accTable = new bool[mesh.vertexCount];
            for (int i = 0; i < accTable.Length; i++)
            {
                accTable[i] = false;
            }
            List<int>[] edges = new List<int>[mesh.vertexCount];
            for (int j = 0; j < edges.Length; j++)
            {
                edges[j] = new List<int>();
            }
            int[] triangles = mesh.triangles;
            for (int k = 0; k < (triangles.Length / 3); k++)
            {
                for (int n = 0; n < 3; n++)
                {
                    for (int num5 = 0; num5 < 3; num5++)
                    {
                        if (n != num5)
                        {
                            edges[triangles[(k * 3) + n]].Add(triangles[(k * 3) + num5]);
                            edges[triangles[(k * 3) + num5]].Add(triangles[(k * 3) + n]);
                        }
                    }
                }
            }
            Vector3[] vertices = mesh.vertices;
            for (int m = 0; m < vertices.Length; m++)
            {
                for (int num7 = 0; num7 < vertices.Length; num7++)
                {
                    Vector3 vector = vertices[m] - vertices[num7];
                    if (vector.magnitude < float.Epsilon)
                    {
                        edges[m].Add(num7);
                    }
                }
            }
            while (true)
            {
                int index = 0;
                while (index < accTable.Length)
                {
                    if (!accTable[index])
                    {
                        break;
                    }
                    index++;
                }
                if (index == accTable.Length)
                {
                    return list.ToArray();
                }
                List<int> list2 = new List<int>();
                this.GetConnectedGraph(index, edges, accTable, list2);
                list.Add(list2.ToArray());
            }
        }
    }
}

