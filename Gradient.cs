using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("UI/Effects/Gradient")]
public class Gradient : BaseMeshEffect
{
    [SerializeField]
    public Color32 bottomColor = Color.black;
    [SerializeField]
    public Color32 topColor = Color.white;

    public override void ModifyMesh(VertexHelper vh)
    {
        int num = vh.get_currentVertCount();
        if (this.IsActive() && (num != 0))
        {
            List<UIVertex> list = new List<UIVertex>();
            vh.GetUIVertexStream(list);
            UIVertex vertex = new UIVertex();
            int count = list.Count;
            int num3 = Mathf.Clamp(2, 0, count - 1);
            int num4 = Mathf.Clamp(count / 5, 0, count - 1);
            UIVertex vertex2 = list[num3];
            float y = vertex2.position.y;
            UIVertex vertex3 = list[num4];
            float num7 = vertex3.position.y - y;
            for (int i = 0; i < num; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);
                vertex.color *= Color.Lerp((Color) this.bottomColor, (Color) this.topColor, (vertex.position.y - y) / num7);
                vh.SetUIVertex(vertex, i);
            }
        }
    }
}

