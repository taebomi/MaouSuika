using System.Threading;
using Cysharp.Threading.Tasks;
using SOSG.Customization.Monster;
using SOSG.Monster;
using SOSG.System.UI;
using SOSG.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MonsterSummonScreen : MonoBehaviour, IModalUI
{
    [SerializeField] private MonsterUnlock monsterUnlock;

    [SerializeField] private MonsterIconUIElement monsterIcon;
    [SerializeField] private TMP_Text descriptionTmp;
    [SerializeField] private RectMask2D rectMask;
    [field:SerializeField] public Canvas Canvas { get; private set; }

    [SerializeField] private GameObject resultScreen;

    public bool CanInteract { get; private set; }


    public void Initialize()
    {
        resultScreen.gameObject.SetActive(false);
    }

    public void OnSummonStarted()
    {
        CanInteract = false;
        // modalUI.Show();
    }

    public void ShowResultScreen(MonsterDataSO monsterData, string text)
    {
        monsterIcon.Set(monsterData);
        descriptionTmp.text = text;
        rectMask.enabled = false;
        CanInteract = true;
        resultScreen.SetActive(true);
    }

    public void OnOverlayClicked() => CloseResultScreenAsync().Forget();

    private async UniTaskVoid CloseResultScreenAsync()
    {
        CanInteract = false;
        // modalUI.Hide();
        monsterUnlock.OnResultClosing();

        rectMask.enabled = true;
        var height = ((RectTransform)transform).rect.height;
        var padding = Vector4.zero;
        var timer = 0f;
        const float duration = 0.5f;
        while (timer < duration)
        {
            var easing = Easing.InSine(timer, duration);
            padding.y = height * easing;
            rectMask.padding = padding;
            timer += Time.deltaTime;
            await UniTask.Yield(destroyCancellationToken);
        }

        resultScreen.SetActive(false);
    }
}