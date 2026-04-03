using System.Collections.Generic;
using SOSG.Stage.SplitScreenMode;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaQueueManager : MonoBehaviour
    {
        private List<SuikaQueue> _queueList;

        private const int AddNum = 10;


        public void SetUp(PlayerSuikaManager[] players)
        {
            _queueList = new List<SuikaQueue>();
            foreach (var player in players)
            {
                _queueList.Add(player.Queue);
            }
            EnqueueAll();
        }

        public void EnqueueAll()
        {
            for (var i = 0; i < AddNum; i++)
            {
                var randomTier = SuikaUtility.GetShootableRandomTier();
                foreach (var queue in _queueList)
                {
                    queue.Push(randomTier);
                }
            }
        }
    }
}