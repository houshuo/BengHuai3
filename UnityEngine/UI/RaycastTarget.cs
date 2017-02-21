namespace UnityEngine.UI
{
    using System;
    using UnityEngine;

    [DisallowMultipleComponent, AddComponentMenu("UI/RaycastTarget"), RequireComponent(typeof(RectTransform))]
    public class RaycastTarget : Graphic, ICanvasRaycastFilter
    {
        private RectTransform m_RectTransform;

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return true;
        }

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }

        public RectTransform rectTransform
        {
            get
            {
                if (this.m_RectTransform == null)
                {
                }
                return (this.m_RectTransform = base.GetComponent<RectTransform>());
            }
        }
    }
}

