using System.Collections;
using TBM.MaouSuika.Core.Input;
using UnityEngine;

namespace TBM.MaouSuika.Gameplay.Puzzle
{
    public class SuikaSlashSkill : SkillBase
    {
        [SerializeField] private SuikaSlashConfigSO config;
        [SerializeField] private SuikaSlashOverlay overlay;
        [SerializeField] private SuikaSelectionVirtualCursor virtualCursor;

        public override float MaxGauge => config.MaxGauge;

        public override IEnumerator Execute(SkillExecutionContext context)
        {
            WasCancelled = false;
            context.InputLockSystem.Lock(nameof(SuikaSlashSkill), InputChannel.Shooter);

            var inputType = GetSkillInputType(context.InputController.Scheme);
            var selector = CreateSelector(inputType, context);

            SuikaObject highlightedSuika = null;

            while (!selector.IsConfirmed && !selector.IsCancelled)
            {
                selector.Tick(Time.deltaTime);

                var hovered = selector.HoveredSuika;
                if (hovered != highlightedSuika)
                {
                    highlightedSuika?.GetComponentInChildren<SuikaHighlightController>()?.Hide();
                    highlightedSuika = hovered;
                    highlightedSuika?.GetComponentInChildren<SuikaHighlightController>()?.Show();
                }

                yield return null;
            }

            highlightedSuika?.GetComponentInChildren<SuikaHighlightController>()?.Hide();

            var target = selector.HoveredSuika;
            selector.Dispose();

            if (selector.IsCancelled || target == null)
            {
                WasCancelled = true;
                context.InputLockSystem.Unlock(nameof(SuikaSlashSkill));
                yield break;
            }

            yield return overlay.PlayCutscene(target.transform.position, config);

            if (target != null && target.gameObject.activeInHierarchy)
                context.SuikaSystem.Despawn(target);

            context.InputLockSystem.Unlock(nameof(SuikaSlashSkill));
        }

        private ISuikaTargetSelector CreateSelector(SkillInputType inputType, SkillExecutionContext context)
        {
            if (inputType == SkillInputType.Pointer)
                return new PointerSuikaTargetSelector(context.Camera, context.InputController.Scheme);

            return new VirtualCursorSuikaTargetSelector(
                context.Camera,
                context.InputController,
                virtualCursor,
                config.CursorMoveSpeed);
        }

        private static SkillInputType GetSkillInputType(string scheme)
        {
            return scheme switch
            {
                InputConstants.ControlScheme.MOUSE => SkillInputType.Pointer,
                InputConstants.ControlScheme.KEYBOARD_MOUSE => SkillInputType.Pointer,
                InputConstants.ControlScheme.TOUCHSCREEN => SkillInputType.Pointer,
                _ => SkillInputType.VirtualCursor,
            };
        }
    }
}
