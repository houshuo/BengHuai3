namespace MoleMole.MainMenu
{
    using System;
    using UnityEngine;

    public class LightningObject
    {
        private bool _active;
        public GameObject _object;
        public float DelayTime;
        public float Intensity;
        public float Lifttime;
        public LightningType.LightningMaterial Mat;
        public Vector3 OrigScale;
        public float Size;
        public float StartLifttime;

        public void Show(bool show)
        {
            this.Object.SetActive(show);
        }

        public void UpdatePosition(Vector3 position)
        {
            Transform transform = this.Object.transform;
            transform.localScale = (Vector3) (this.OrigScale * this.Size);
            transform.localPosition = position - ((Vector3) (this.Mat.pivot * this.Size));
        }

        public bool Active
        {
            get
            {
                return this._active;
            }
            set
            {
                this._active = value;
                if (!this._active && (this._object != null))
                {
                    this._object.SetActive(false);
                }
            }
        }

        public GameObject Object
        {
            get
            {
                if (this._object == null)
                {
                    Debug.LogError("Missing lightning quad object, please try restarting the particle system");
                }
                return this._object;
            }
            set
            {
                this._object = value;
            }
        }
    }
}

