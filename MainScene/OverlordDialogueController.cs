using Cysharp.Threading.Tasks;
using SOSG.System.Dialogue;
using SOSG.System.Localization;
using SOSG.System.Scene;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SOSG.MainScene
{
    public class OverlordDialogueController : MonoBehaviour
    {
        [SerializeField] private IntEventSO timingEventSO;

        private TBMStringTable _stringTable;

        private void Awake()
        {
            SceneSetUpHelper.AddTask(SetUpAsync);
        }

        private void OnEnable()
        {
            timingEventSO.OnEventRaised += OnTitleTimingEventRaised;
        }

        private void OnDisable()
        {
            timingEventSO.OnEventRaised -= OnTitleTimingEventRaised;
        }

        private async UniTask SetUpAsync()
        {
            _stringTable = GetComponent<TBMStringTable>();
            await _stringTable.SetUpAsync(LocalizationTableName.MainScene);
        }

        private void OnTitleTimingEventRaised(int timing)
        {
            switch (timing)
            {
                case 10:
                    ShowFirstLine();
                    break;
            }
        }

        private void ShowFirstLine()
        {
            DialogueHelper.SetPortraitActive(true);
            _stringTable.PrintConversation($"start_{Random.Range(1, 6)}");
        }
    }
}