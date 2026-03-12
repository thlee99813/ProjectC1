using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemySpawnTest : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject testEnemyPrefab;
    [SerializeField] private GameObject warningPrefab;
    //[SerializeField] private float warningDuration = 0.5f;

    [SerializeField] private float phase1spawnInterval = 3f;
    [SerializeField] private float phase2spawnInterval = 2f;
    [SerializeField] private float phase3spawnInterval = 1f;

    [SerializeField] private int maxAliveEnemies = 100;

    [SerializeField] private float minSpawnRadius = 5f;
    [SerializeField] private float maxSpawnRadius = 10f;


    private Vector3 spawnPosition;
    private GameObject warning;

    private float spawnTimer;
    private int aliveEnemies;

    void Start()
    {
        spawnTimer = phase1spawnInterval;
    }

    void Update()
    {

        if (aliveEnemies >= maxAliveEnemies)
        {
            return;
        }
        if (GameManager.Instance.CurrentPhase == GameManager.Phase.Death || GameManager.Instance.CurrentPhase == GameManager.Phase.End)
        {
            return;
        }

        spawnTimer -= Time.deltaTime;

        if (spawnTimer > 0f)
        {
            return;
        }

        SpawnEnemy();

        switch (GameManager.Instance.CurrentPhase)
        {
            case GameManager.Phase.Phase1:
                spawnTimer = phase1spawnInterval;
                break;
            case GameManager.Phase.Phase2:
                spawnTimer = phase2spawnInterval;
                break;
            case GameManager.Phase.Phase3:
                spawnTimer = phase3spawnInterval;
                break;


        }

    }

    private void SpawnEnemy()
    {
        spawnPosition = GetRandomPoint(player.position, minSpawnRadius, maxSpawnRadius);

        if (warningPrefab != null)
        {
            WaveWarning();
            Invoke(nameof(WaveWarning), 0.3f);
        }
        Instantiate(testEnemyPrefab, spawnPosition, Quaternion.identity);
        aliveEnemies++;
    }

    private void WaveWarning()
    {
        Instantiate(warningPrefab, spawnPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPoint(Vector3 center, float minRadius, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Mathf.Sqrt(Random.Range(minRadius * minRadius, maxRadius * maxRadius));

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);

    }


}
