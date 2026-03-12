using System.Collections;
using UnityEngine;

public class BandiSpanwer : MonoBehaviour
{
    public GameObject prefab;
    public float spawnInterval = 15f;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (true) // 반복 조건
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
