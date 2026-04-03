using UnityEngine;
using Random = UnityEngine.Random;

public class Cloud : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    
    private float _speed;

    
    private const float AreaWidth = TitleScreenMap.AreaWidth * 0.5f;

    public void Set(Sprite sprite, float speed)
    {
        sr.sprite = sprite;
        _speed = speed;
        sr.sortingOrder = Random.value > 0.5f ? -5 : -15;
    }

    private void Update()
    {
        transform.position -= new Vector3(_speed * Time.deltaTime, 0f, 0f);
        if (transform.localPosition.x < -AreaWidth)
        {
            transform.position += new Vector3(AreaWidth * 2f, 0);
        }
    }
}
