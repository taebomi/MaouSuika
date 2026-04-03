using Unity.Mathematics;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
    [SerializeField] private Sprite[] cloudSpriteArr;
    [SerializeField] private Cloud cloudPrefab;

    [Header("Option")]
    [SerializeField] private int cloudCount;
    [SerializeField] private Vector3 minCloudPos;
    [SerializeField] private Vector3 maxCloudPos;
    [SerializeField] private float minCloudSpeed;
    [SerializeField] private float maxCloudSpeed;


    private void Awake()
    {
        CreateClouds();
    }

    private void CreateClouds()
    {
        var centerPos = transform.position;
        for (var i = 0; i < cloudCount; i++)
        {
            var randomPos = centerPos + new Vector3(UnityEngine.Random.Range(minCloudPos.x, maxCloudPos.x),
                UnityEngine.Random.Range(minCloudPos.y, maxCloudPos.y), 0f);
            var cloud = Instantiate(cloudPrefab, randomPos, quaternion.identity, transform);
            cloud.Set(cloudSpriteArr[UnityEngine.Random.Range(0, cloudSpriteArr.Length)],
                UnityEngine.Random.Range(minCloudSpeed, maxCloudSpeed));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (minCloudPos + maxCloudPos) * 0.5f, maxCloudPos - minCloudPos);
    }
#endif
}