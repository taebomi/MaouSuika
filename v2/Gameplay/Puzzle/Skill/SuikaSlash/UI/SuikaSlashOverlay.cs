using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaSlashOverlay : MonoBehaviour
    {
        [SerializeField] private CanvasGroup overlayGroup;
        [SerializeField] private GameObject slashEffectContainer;

        private void Awake()
        {
            overlayGroup.alpha = 0f;
            if (slashEffectContainer != null)
                slashEffectContainer.SetActive(false);
        }

        public IEnumerator PlayCutscene(Vector3 targetWorldPos, SuikaSlashConfigSO config)
        {
            yield return overlayGroup
                .DOFade(1f, config.OverlayDarkDuration)
                .WaitForCompletion();

            if (slashEffectContainer != null)
            {
                slashEffectContainer.transform.position = targetWorldPos;
                slashEffectContainer.SetActive(true);
            }

            yield return new WaitForSeconds(config.SlashEffectDuration);

            if (slashEffectContainer != null)
                slashEffectContainer.SetActive(false);

            yield return overlayGroup
                .DOFade(0f, config.OverlayBrightDuration)
                .WaitForCompletion();
        }
    }
}
