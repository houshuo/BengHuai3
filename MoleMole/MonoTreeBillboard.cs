namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoTreeBillboard : MonoBehaviour
    {
        private Camera _camera;
        private int _frame = -1;
        private Vector3[] _originalLocalPositions;
        private bool _visible;
        public bool CheckVisible;
        public int UpdateRate = 1;
        public float ZOffsetStep = 0.001f;

        private void OnBecameInvisible()
        {
            this._visible = false;
        }

        private void OnBecameVisible()
        {
            this._visible = true;
            this._frame = -1;
        }

        private void Start()
        {
            this._camera = Camera.main;
            this._originalLocalPositions = new Vector3[base.transform.childCount];
            for (int i = 0; i < base.transform.childCount; i++)
            {
                this._originalLocalPositions[i] = base.transform.GetChild(i).localPosition;
            }
        }

        private void Update()
        {
            this._frame++;
            if ((!this.CheckVisible || this._visible) && ((this._frame % this.UpdateRate) == 0))
            {
                for (int i = 0; i < base.transform.childCount; i++)
                {
                    Transform child = base.transform.GetChild(i);
                    Vector3 vector = this._camera.transform.position - child.position;
                    vector.y = 0f;
                    vector.Normalize();
                    child.LookAt(child.position + vector);
                    child.localPosition = this._originalLocalPositions[i] - ((Vector3) ((vector * this.ZOffsetStep) * i));
                }
            }
        }
    }
}

