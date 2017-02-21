namespace UnityEngine.UI
{
    using System;

    public class GraphicCast : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}

