using SOSG.Monster;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.Stage.Suika
{
    public class SuikaVisualComponent : MonoBehaviour
    {
        [SerializeField] private SuikaObject suika;
        
        [SerializeField] private Transform capsuleTr;
        [SerializeField] private Transform monsterTr;

        [SerializeField] private SpriteRenderer bgBodySr;
        [SerializeField] private SpriteRenderer fgGlassSr;
        [SerializeField] private SpriteRenderer fgBodySr;
        [SerializeField] private SpriteRenderer glossSr;
        [SerializeField] private SpriteRenderer outlineSr;

        [SerializeField] private SpriteRenderer monsterSr;

        [SerializeField] private Animator monsterAni;

        private void Update()
        {
            glossSr.transform.rotation = Quaternion.identity;
        }

        public void OnLoaded()
        {
            ApplyData();
            SetSortingLayerID(TaeBoMiCache.ForegroundSortingLayerID);
        }

        public void OnMerged()
        {
            ApplyData();
            SetSortingLayerID(TaeBoMiCache.GashaponSortingLayerID);
        }

        public void OnShot()
        {
            SetSortingLayerID(TaeBoMiCache.GashaponSortingLayerID);
        }
    
        public void SetColor(Color color, bool applyAll)
        {
            bgBodySr.color = fgBodySr.color = outlineSr.color = color;
            if (applyAll)
            {
                fgGlassSr.color = glossSr.color = monsterSr.color = color;
            }
            else
            {
                fgGlassSr.color = glossSr.color = monsterSr.color = new Color(1f, 1f, 1f, color.a);
            }
        }

        public void SetAlphaWithCapsuleColor(float alpha)
        {
            var color = suika.Data.capsuleColor;
            color.a = alpha;
            SetColor(color, false);
        }

        private void ApplyData()
        {
            var data = suika.Data;
            var size = suika.Size;
            capsuleTr.localScale = new Vector3(size, size, 1f);
            capsuleTr.rotation = Quaternion.Euler(0f,0f, Random.Range(0, 360f));
        
            var monsterSize = data.ySize;
            var calibratedMonsterSize = 1f;
            if (size < monsterSize)
            {
                calibratedMonsterSize = size / monsterSize;
            }

            monsterTr.localScale = new Vector3(calibratedMonsterSize, calibratedMonsterSize, 1f);
            monsterTr.localPosition = -data.bodyCenterPos * calibratedMonsterSize;
            monsterAni.runtimeAnimatorController = data.animatorOverrideController;

            SetColor(data.capsuleColor, false);
        }

        private void SetSortingLayerID(int id)
        {
            bgBodySr.sortingLayerID = id;
            fgGlassSr.sortingLayerID = id;
            fgBodySr.sortingLayerID = id;
            glossSr.sortingLayerID = id;
            outlineSr.sortingLayerID = id;
            monsterSr.sortingLayerID = id;
        }
    }
}