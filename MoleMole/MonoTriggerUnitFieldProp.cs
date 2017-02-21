namespace MoleMole
{
    using System;
    using System.Collections;
    using UnityEngine;

    public class MonoTriggerUnitFieldProp : MonoTriggerProp
    {
        private Vector3 _childrenOffset;
        private int _numberX;
        private int _numberZ;
        private Vector3 _targetForward;
        private float _xUnitLength;
        private float _zUnitLength;

        protected override void Awake()
        {
            base.Awake();
            base._triggerCollider.enabled = false;
        }

        private void CopyModels()
        {
            Transform child = base.gameObject.transform.GetChild(0);
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            for (int i = 0; i < this._numberX; i++)
            {
                for (int j = 0; j < this._numberZ; j++)
                {
                    if ((i != 0) || (j != 0))
                    {
                        Transform transform2 = UnityEngine.Object.Instantiate<Transform>(child);
                        transform2.SetParent(base.gameObject.transform);
                        transform2.localRotation = Quaternion.Euler(Vector3.zero);
                        transform2.localPosition = (Vector3) (((right * this._xUnitLength) * i) + ((forward * this._zUnitLength) * j));
                    }
                }
            }
            IEnumerator enumerator = base.gameObject.transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    current.localPosition += new Vector3(-this._childrenOffset.x, 0f, -this._childrenOffset.z);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
            base.transform.forward = this._targetForward;
        }

        public virtual void DisableProp()
        {
            base._triggerCollider.enabled = false;
        }

        public virtual void EnableProp()
        {
            base._triggerCollider.enabled = true;
        }

        private void GetUnitLength()
        {
            this._xUnitLength = base.config.PropArguments.Length;
            this._zUnitLength = base.config.PropArguments.Length;
        }

        public virtual void InitUnitFieldPropRange(int numberX, int numberZ)
        {
            this._targetForward = base.transform.forward;
            base.transform.forward = Vector3.forward;
            this._numberX = numberX;
            this._numberZ = numberZ;
            this.GetUnitLength();
            this.ResetColliderSize();
            this.CopyModels();
        }

        private void ResetColliderSize()
        {
            BoxCollider collider = (BoxCollider) base._triggerCollider;
            Vector3 center = collider.center;
            collider.size = new Vector3(this._xUnitLength * this._numberX, collider.size.y, this._zUnitLength * this._numberZ);
            Vector3 forward = Vector3.forward;
            Vector3 right = Vector3.right;
            this._childrenOffset = (Vector3) ((center + ((right * (collider.size.x - this._xUnitLength)) / 2f)) + ((forward * (collider.size.z - this._zUnitLength)) / 2f));
        }
    }
}

