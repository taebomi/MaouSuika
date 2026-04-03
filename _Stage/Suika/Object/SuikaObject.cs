using System;
using SOSG.Monster;
using SOSG.Stage.SplitScreenMode;
using UnityEngine;

namespace SOSG.Stage.Suika
{
    public class SuikaObject : MonoBehaviour
    {
        [field: SerializeField] public SuikaVisualComponent VisualComponent { get; private set; }
        [field: SerializeField] public SuikaPhysicsComponent PhysicsComponent { get; private set; }
        public PlayerSuikaManager Player { get; private set; }
        public float Size { get; private set; }
        public float Radius => Size * 0.5f;
        public int Tier { get; private set; }
        public int CreationOrder { get; private set; }
        public MonsterDataSO Data { get; private set; }

        public SuikaState State { get; private set; }

        public void SetData(PlayerSuikaManager player, MonsterDataSO data, int tier)
        {
            Data = data;
            Player = player;
            Tier = tier;
            Size = SuikaUtility.SizeArr[tier];
        }

        public void SetCreationOrder(int order)
        {
            CreationOrder = order;
        }

        public void SetPosition(Vector3 pos)
        {
            transform.position = pos;
        }

        /// <summary>
        /// Suika 발사 장전 시
        /// </summary>
        public void OnLoaded()
        {
            State = SuikaState.Loaded;
            VisualComponent.OnLoaded();
            PhysicsComponent.OnLoaded();
            gameObject.SetActive(true);
        }


        /// <summary>
        /// Merge 된 후 새로운 Suika 생성 시
        /// </summary>
        public void OnMerged()
        {
            State = SuikaState.Active;
            VisualComponent.OnMerged();
            PhysicsComponent.OnMerged();
            gameObject.SetActive(true);
        }


        public void OnShot(Vector2 vel)
        {
            State = SuikaState.Active;
            VisualComponent.OnShot();
            PhysicsComponent.OnShot(vel);
        }

        public void OnMerging()
        {
            State = SuikaState.Merging;
            PhysicsComponent.OnMerging();
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
            Player.Creator.ReturnSuika(this);
            State = SuikaState.Idle;
        }
    }
}