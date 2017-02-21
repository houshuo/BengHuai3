namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoIslandModel : MonoBehaviour
    {
        [SerializeField]
        private Renderer[] _extraRenderQueueArray;
        [SerializeField]
        private Animation _lock_disable_animation;
        [SerializeField]
        private Transform _lock_disable_mesh;
        [SerializeField]
        private Material _lock_mat;
        [SerializeField]
        private Transform[] _masked_disable_mesh;
        private Material _normal_mat;
        [SerializeField]
        private Renderer _renderer;
        private CabinStatus _status = CabinStatus.UnLocked;

        public Renderer GetRenderer()
        {
            return this._renderer;
        }

        public Renderer[] GetRenderer_RenderQueue()
        {
            return this._extraRenderQueueArray;
        }

        public void RefreshLockStyle(CabinStatus _targetStatus)
        {
            if ((this._status == CabinStatus.UnLocked) && (_targetStatus == CabinStatus.Locked))
            {
                this.ToLockGraphic();
                this._status = CabinStatus.Locked;
            }
            else if ((this._status == CabinStatus.Locked) && (_targetStatus == CabinStatus.UnLocked))
            {
                this.ToUnLockGraphic();
                this._status = CabinStatus.UnLocked;
            }
        }

        private void ToLockGraphic()
        {
            if (this._lock_disable_mesh != null)
            {
                this._lock_disable_mesh.gameObject.SetActive(false);
            }
            if (this._lock_disable_animation != null)
            {
                this._lock_disable_animation.enabled = false;
            }
            if (this._lock_mat != null)
            {
                this._normal_mat = this._renderer.material;
                this._renderer.material = this._lock_mat;
            }
        }

        public void ToMaskedGraphic()
        {
            if (this._lock_disable_mesh != null)
            {
                this._lock_disable_mesh.gameObject.SetActive(false);
            }
            if (this._lock_disable_animation != null)
            {
                this._lock_disable_animation.enabled = false;
            }
            foreach (Transform transform in this._masked_disable_mesh)
            {
                transform.gameObject.SetActive(false);
            }
        }

        private void ToUnLockGraphic()
        {
            if (this._lock_disable_mesh != null)
            {
                this._lock_disable_mesh.gameObject.SetActive(true);
            }
            if (this._lock_disable_animation != null)
            {
                this._lock_disable_animation.enabled = true;
            }
            if (this._lock_mat != null)
            {
                this._renderer.material = this._normal_mat;
            }
        }

        public void ToUnMaskedGraphic()
        {
            if (this._status == CabinStatus.UnLocked)
            {
                if (this._lock_disable_mesh != null)
                {
                    this._lock_disable_mesh.gameObject.SetActive(true);
                }
                if (this._lock_disable_animation != null)
                {
                    this._lock_disable_animation.enabled = true;
                }
                foreach (Transform transform in this._masked_disable_mesh)
                {
                    transform.gameObject.SetActive(true);
                }
            }
        }
    }
}

