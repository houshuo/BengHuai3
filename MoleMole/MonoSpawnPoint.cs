namespace MoleMole
{
    using System;
    using UnityEngine;

    public class MonoSpawnPoint : MonoBehaviour
    {
        [SerializeField]
        private Color _color = Color.white;
        [SerializeField]
        private Vector3 _gizmosOffset;
        [SerializeField]
        private Vector3 _gizmosSize;
        [SerializeField]
        private bool _showGizmos;
        [SerializeField]
        private bool _wireMode;

        private void OnDrawGizmos()
        {
            if (this._showGizmos)
            {
                Color color = Gizmos.color;
                Gizmos.color = this._color;
                if (this._wireMode)
                {
                    Gizmos.DrawWireCube(base.transform.position + this._gizmosOffset, this._gizmosSize);
                }
                else
                {
                    Gizmos.DrawCube(base.transform.position + this._gizmosOffset, this._gizmosSize);
                }
                Gizmos.color = color;
            }
        }

        public Vector3 XZPosition
        {
            get
            {
                return new Vector3(base.transform.position.x, 0f, base.transform.position.z);
            }
        }
    }
}

