using TMPro;
using UnityEngine;

namespace SOSG.Stage.Suika.Score
{
    public class AccumulatedScoreUIElement : MonoBehaviour
    {
        [SerializeField] private AccumulatedScoreUIElementPoolSO poolSO;

        [SerializeField] private RectTransform rt;
        [SerializeField] private Animator ani;
        [SerializeField] private TextMeshProUGUI tmp;

        public int Score { get; private set; }
        public bool IsAccumulating { get; set; }
        public float CurYPos => rt.anchoredPosition.y;


        public void SetUp(Vector2 pos)
        {
            rt.anchoredPosition = pos;
            Score = 0;
            IsAccumulating = true;
            gameObject.SetActive(true);
        }

        public void SetYPosition(float yPos)
        {
            rt.anchoredPosition = new Vector2(0f, yPos);
        }

        public void AddScore(int score)
        {
            Score += score;
            tmp.text = $"+{Score}";
            ani.SetTrigger(AnimatorCache.RestartTrigger);
        }

        public void UpdateText(int score)
        {
            tmp.text = $"+{score}";
        }

        public void MakeDisappear()
        {
            ani.SetTrigger(AnimatorCache.DisappearTrigger);
        }

        public void Emphasize()
        {
            IsAccumulating = false;
        }

        public void Return()
        {
            gameObject.SetActive(false);
            poolSO.Return(this);
        }
    }
}