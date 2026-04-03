using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TBM.MaouSuika.Gameplay
{
    public class SuikaShell : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bgBodySr;
        [SerializeField] private SpriteRenderer fgGlassSr;
        [SerializeField] private SpriteRenderer fgBodySr;
        [SerializeField] private SpriteRenderer glossSr;
        [SerializeField] private SpriteRenderer outlineSr;

        private Transform _glossTr;

        private void Awake()
        {
            _glossTr = glossSr.transform;
        }

        public void Setup(float size)
        {
            transform.localScale = Vector3.one * size;
            transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        }

        private void Update()
        {
            _glossTr.rotation = Quaternion.identity;
        }

        public void SetCoreColor(Color color)
        {
            bgBodySr.color = color;
            fgBodySr.color = color;
            outlineSr.color = color;
        }

        public void SetDetailColor(Color color)
        {
            glossSr.color = color;
            fgGlassSr.color = color;
        }

        public void SetSortingLayerID(int id)
        {
            bgBodySr.sortingLayerID = id;
            fgGlassSr.sortingLayerID = id;
            fgBodySr.sortingLayerID = id;
            glossSr.sortingLayerID = id;
            outlineSr.sortingLayerID = id;
        }
    }
}