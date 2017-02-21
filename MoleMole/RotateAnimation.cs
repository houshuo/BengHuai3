namespace MoleMole
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [RequireComponent(typeof(Renderer))]
    public class RotateAnimation : MonoBehaviour
    {
        public List<Transform> m_Objects;
        public Vector3 SpeedVector = new Vector3(0f, 1f, 0f);

        private void Update()
        {
            if (this.m_Objects.Count == 0)
            {
                base.transform.Rotate((Vector3) (this.SpeedVector * Time.deltaTime));
            }
            else
            {
                for (int i = 0; i < this.m_Objects.Count; i++)
                {
                    this.m_Objects[i].transform.Rotate((Vector3) (this.SpeedVector * Time.deltaTime));
                }
            }
        }
    }
}

