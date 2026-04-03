using System;
using System.Collections.Generic;
using SOSG.Stage.SplitScreenMode;
using SOSG.System.PlayData;
using UnityEngine;
using UnityEngine.Serialization;

namespace SOSG.Stage.Suika
{
    public class SuikaCreator : MonoBehaviour
    {
        [SerializeField] private PlayerSuikaManager player;

        [SerializeField] private SuikaCollection suikaCollection;
        [SerializeField] private SuikaPool suikaPool;

        private int _creationOrder;

        private void Awake()
        {
            _creationOrder = 0;
        }


        public SuikaObject GetSuika(int tier, Vector3 pos)
        {
            var suika = suikaPool.Pop(pos);
            suikaCollection.AddSuika(suika);
            suika.SetData(player, player.Loadout.MonsterDataArr[tier], tier);
            suika.SetCreationOrder(_creationOrder);
            _creationOrder++;
            return suika;
        }

        public SuikaObject GetSuika(int tier, Vector3 pos, int creationOrder)
        {
            var suika = suikaPool.Pop(pos);
            suikaCollection.AddSuika(suika);
            suika.SetData(player, player.Loadout.MonsterDataArr[tier], tier);
            suika.SetCreationOrder(creationOrder);
            return suika;
        }

        public void ReturnSuika(SuikaObject suika)
        {
            suikaCollection.RemoveSuika(suika);
            suikaPool.Push(suika);
        }
    }
}