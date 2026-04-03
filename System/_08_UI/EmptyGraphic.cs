using UnityEngine;
using UnityEngine.UI;

namespace SOSG.System
{
    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh) => vh.Clear();
    }
}