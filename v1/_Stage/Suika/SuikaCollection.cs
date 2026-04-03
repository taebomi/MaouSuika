using System;
using System.Collections.Generic;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaCollection : MonoBehaviour
    {
        public Dictionary<int, SuikaObject> CollideDict { get; private set; }
        public LinkedList<SuikaObject> ActiveSuikaList { get; private set; }

        private void Awake()
        {
            CollideDict = new Dictionary<int, SuikaObject>();
            ActiveSuikaList = new LinkedList<SuikaObject>();
        }

        public void AddSuika(SuikaObject suika)
        {
            CollideDict.Add(suika.PhysicsComponent.ColliderID, suika);
            ActiveSuikaList.AddFirst(suika);
        }

        public void RemoveSuika(SuikaObject suika)
        {
            CollideDict.Remove(suika.PhysicsComponent.ColliderID);
            ActiveSuikaList.Remove(suika);
        }

        public bool TryGetSuika(int colliderID, out SuikaObject suika)
        {
            return CollideDict.TryGetValue(colliderID, out suika);
        }

        public SuikaObject GetSuika(int colliderID)
        {
            return CollideDict[colliderID];
        }
    }
}