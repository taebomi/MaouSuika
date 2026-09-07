using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MaouSuika.Gameplay
{
    [Serializable]
    public class SoulOrbShell
    {
        [SerializeField] private Transform shellTr;

        [SerializeField] private SpriteRenderer glassBack;
        [SerializeField] private SpriteRenderer seamBack;
        [SerializeField] private SpriteRenderer seamFront;
        [SerializeField] private SpriteRenderer sheen;
        [SerializeField] private SpriteRenderer outline;

        private Transform _sheenTr;

        public void Initialize()
        {
            _sheenTr = sheen.transform;
        }

        public void Setup(float scale)
        {
            shellTr.localScale = new Vector3(scale, scale, 1f);
            shellTr.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        public void TickVisuals()
        {
            _sheenTr.rotation = Quaternion.identity;
        }

        public void Rotate(float delta)
        {
            shellTr.Rotate(0f, 0f, delta);
        }

        public void SetCaseColor(Color color)
        {
            seamBack.color = color;
            seamFront.color = color;
            outline.color = color;
        }

        public void SetWindowColor(Color color)
        {
            sheen.color = color;
            glassBack.color = color;
        }

        public void SetSortingLayerID(int layerID)
        {
            glassBack.sortingLayerID = layerID;
            seamBack.sortingLayerID = layerID;
            seamFront.sortingLayerID = layerID;
            sheen.sortingLayerID = layerID;
            outline.sortingLayerID = layerID;
        }

        public void SetOutlineColor(Color color)
        {
            outline.color = color;
        }
    }
}