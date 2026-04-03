using UnityEngine;
using UnityEngine.UI;

namespace TBM.UI
{
    [AddComponentMenu("TBM/UI/Empty Graphic")]
    public class EmptyGraphic : Graphic
    {
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
        }
    }
}