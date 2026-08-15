using UnityEngine;
using UnityEngine.UI;

public class UILineRenderer : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
    }
}
