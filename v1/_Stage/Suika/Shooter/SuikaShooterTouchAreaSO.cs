using System;
using SOSG.System.Display;
using UnityEngine;

namespace SOSG.Stage.Suika.Shooter
{
    [CreateAssetMenu(menuName = "SOSG/Input/Touch Area")]
    public class SuikaShooterTouchAreaSO : ScriptableObject
    {
        public TouchArea[] playerTouchAreaArr;

        public int GetPlayerNum(Vector2 screenPos)
        {
            var x = screenPos.x / Screen.width;
            var y = screenPos.y / Screen.height;
            for (var playerIdx = 0; playerIdx < playerTouchAreaArr.Length; playerIdx++)
            {
                var touchArea = playerTouchAreaArr[playerIdx];
                if (touchArea.minX <= x && x <= touchArea.maxX && touchArea.minY <= y && y <= touchArea.maxY)
                {
                    return playerIdx;
                }
            }

            return 0;
        }


        [Serializable]
        public struct TouchArea
        {
            public float minX;
            public float maxX;
            public float minY;
            public float maxY;

            public bool isYInverted;
        }
    }
}