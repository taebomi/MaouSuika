using UnityEngine;
using Random = UnityEngine.Random;

public class PlanetController : MonoBehaviour
{
    [SerializeField] private float minRotation;
    [SerializeField] private float maxRotation;
    [SerializeField] private float rotationSpeed;

    private float _curZRotation;

    private void Awake()
    {
        _curZRotation = Random.Range(minRotation, maxRotation);
        transform.rotation = Quaternion.Euler(0f, 0f, _curZRotation);
    }

    private void Update()
    {
        _curZRotation -= rotationSpeed * Time.deltaTime;
        if (rotationSpeed > 0f) // 반시계 방향
        {
            if(_curZRotation < minRotation)
            {
                _curZRotation = maxRotation;
            }
        }
        else // 시계 방향
        {
            if(_curZRotation > maxRotation)
            {
                _curZRotation = minRotation;
            }
        }
        transform.rotation = Quaternion.Euler(0f, 0f, _curZRotation);
    }
    
}