using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaSystem : MonoBehaviour
    {
        [SerializeField] private SuikaObjectFactory factory;

        [SerializeField] private Transform activeSuikaContainer;

        public IReadOnlyList<SuikaObject> ActiveSuikas => _activeSuikas;
        private readonly List<SuikaObject> _activeSuikas = new();

        private int _creationOrder;


        public void Initialize(MergeSystem mergeSystem, SuikaTierDataTable tierDataTable)
        {
            factory.Initialize(mergeSystem, tierDataTable);
        }

        public void Setup()
        {
            _creationOrder = 0;

            foreach (var activeSuika in _activeSuikas)
            {
                factory.Release(activeSuika);
            }

            _activeSuikas.Clear();
        }

        public SuikaObject Spawn(Vector2 pos, int tier)
        {
            return Spawn(pos, tier, ++_creationOrder);
        }

        public SuikaObject Spawn(Vector2 pos, int tier, int creationOrder)
        {
            var suikaObject = factory.Get(tier, creationOrder);
            suikaObject.SetParent(activeSuikaContainer);
            suikaObject.SetVisualPosition(pos);

            _activeSuikas.Add(suikaObject);
            return suikaObject;
        }

        public void Despawn(SuikaObject suikaObject)
        {
            _activeSuikas.Remove(suikaObject);
            factory.Release(suikaObject);
        }

        public void HandleSuikaMerged(SuikaObject sourceSuika, SuikaObject targetSuika, MergeEvent mergeEvent)
        {
            Despawn(sourceSuika);
            Despawn(targetSuika);
            if (!mergeEvent.HasResult) return;

            var newSuika = Spawn(mergeEvent.Pos, mergeEvent.ResultTier, mergeEvent.CreationOrder);
            newSuika.SetState_Merged();
            newSuika.SetGrounded(mergeEvent.IsGrounded);
        }


        // ################################################
        // DEV
        // ################################################
#if UNITY_EDITOR

        [Button]
        private void Dev_Spawn(Vector2 pos, int tier) => Spawn(pos, tier);

#endif
    }
}