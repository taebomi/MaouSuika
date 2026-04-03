using UnityEngine;

namespace SOSG.Stage.Area
{
    public class BattleArea : MonoBehaviour
    {
        public AreaData AreaData { get; private set; }
        
        public const float AreaWidth = 48f;
        public const float AreaYPos = -50f;
    
        public Vector3 GetNextAreaPos()
        {
            return transform.position + new Vector3(AreaWidth, 0f);
        }

        public float GetAreaEndXPoint()
        {
            return transform.position.x + 36f;
        }

        public void Set(AreaData areaData)
        {
            AreaData = areaData;
        }

        public void Move(Vector3 translation)
        {
            transform.Translate(translation);
        }
    }
}