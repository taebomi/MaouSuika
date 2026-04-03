using TMPro;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private TMP_Text tmp;
    private string _text;

    void Update()
    {
        var numOfDot = Mathf.FloorToInt(Time.time * 3 % 4);
        var dot = "";
        for (var i = 0; i < numOfDot; i++)
        {
            dot += ".";
        }

        tmp.text = $"{_text}{dot}";
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void SetText(string text)
    {
        _text = text;
    }
}