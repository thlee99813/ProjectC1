using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private GameObject BaseEnemyPrefab;
    [SerializeField] private GameObject AdvanceEnemyPrefab;
    [SerializeField] private GameObject MeteorPrefab;

    [SerializeField] private GameObject warningPrefab;
    [SerializeField] private GameObject meteorWarningPrefab;

    [Header("기본 몹 스폰주기")]

    [SerializeField] private float phase1spawnInterval = 3f;
    [SerializeField] private float phase2spawnInterval = 2f;
    [SerializeField] private float phase3spawnInterval = 1f;
    [SerializeField] private float phase3MinSpawnInterval = 0.1f;

    [SerializeField] private float phase3TimerReduce = 0.005f; // 타이머 감소 주기


    [Header("Advance 몹 스폰주기")]

    [SerializeField] private float phase2AdvanceSpawnInterval = 20f;
    [SerializeField] private float phase3AdvanceSpawnInterval = 10f;

    [SerializeField] private float phase3MinAdvanceSpawnInterval = 1f;

    [SerializeField] private float phase3AdvanceTimerReduce = 0.05f; // 타이머 감소 주기

    [Header("Meteor 스폰주기")]
    [SerializeField] private float phase1MeteorSpawnInterval = 10f;
    [SerializeField] private float phase2MeteorSpawnInterval = 10f;
    [SerializeField] private float phase3MeteorSpawnInterval = 10f;
    [SerializeField] private float phase3MinMeteorSpawnInterval = 2f;
    [SerializeField] private float phase3MeteorTimerReduce = 0.05f; // 타이머 감소 주기



    [Header("최대 몬스터 수")]

    [SerializeField] private int maxAliveEnemies = 200;

    [Header("몬스터 소환 반경")]

    [SerializeField] private float minBaseEnemySpawnRadius = 5f;
    [SerializeField] private float maxBaseEnemySpawnRadius = 10f;
    [SerializeField] private float minAdvanceEnemySpawnRadius = 5f;
    [SerializeField] private float maxAdvanceEnemySpawnRadius = 10f;
    [SerializeField] private float minMeteorSpawnRadius = 5f;
    [SerializeField] private float maxMeteorSpawnRadius = 10f;

    private Vector3 spawnPosition;
    private Vector3 spawnadvancePosition;
    private Vector3 spawnMeteorPosition;

    private GameObject warning;

    private float baseSpawnTimer;
    private float advanceSpawnTimer;
    private float meteorSpawnTimer;

    private int aliveEnemies;

    void Start()
    {
        baseSpawnTimer = phase1spawnInterval;
        advanceSpawnTimer = phase3MinAdvanceSpawnInterval;
    }

    void Update()
    {
        if (!CanSpawn())
        {
            return;
        }
        if (GameManager.Instance.EnemyPool != null)
        {
            TrySpawnBaseEnemy();
            TrySpawnAdvanceEnemy();
        }
        TrySpawnMeteor();

    }

    private bool CanSpawn()
    {
        if (aliveEnemies >= maxAliveEnemies)
        {
            return false;
        }


        GameManager.Phase phase = GameManager.Instance.CurrentPhase;

        if (phase == GameManager.Phase.Ready || phase == GameManager.Phase.Death || phase == GameManager.Phase.End)
        {
            return false;
        }

        return true;
    }
    private void TrySpawnBaseEnemy()
    {
        baseSpawnTimer -= Time.deltaTime;
        if (baseSpawnTimer > 0f)
        {
            return;
        }

        SpawnBaseEnemy();
        switch (GameManager.Instance.CurrentPhase)
        {
            case GameManager.Phase.Phase1:
                baseSpawnTimer = phase1spawnInterval;
                break;
            case GameManager.Phase.Phase2:
                baseSpawnTimer = phase2spawnInterval;
                break;
            case GameManager.Phase.Phase3:
                baseSpawnTimer = phase3spawnInterval;
                if (phase3spawnInterval > phase3MinSpawnInterval)
                {
                    phase3spawnInterval -= phase3TimerReduce;
                }
                break;
            case GameManager.Phase.Boss:
                baseSpawnTimer = phase3spawnInterval;
                break;
        }
    }

    private void TrySpawnAdvanceEnemy()
    {
        if (GameManager.Instance.CurrentPhase == GameManager.Phase.Phase1)
        {
            return;
        }
        advanceSpawnTimer -= Time.deltaTime;
        if (advanceSpawnTimer > 0f) return;

        SpawnAdvanceEnemy();
        switch (GameManager.Instance.CurrentPhase)
        {
            case GameManager.Phase.Phase1:
                advanceSpawnTimer = phase2AdvanceSpawnInterval;
                break;
            case GameManager.Phase.Phase2:
                advanceSpawnTimer = phase2AdvanceSpawnInterval;
                break;
            case GameManager.Phase.Phase3:
                advanceSpawnTimer = phase3AdvanceSpawnInterval;
                if (phase3AdvanceSpawnInterval > phase3MinAdvanceSpawnInterval)
                {
                    phase3AdvanceSpawnInterval -= phase3AdvanceTimerReduce;
                }
                break;
            case GameManager.Phase.Boss:
                advanceSpawnTimer = phase3AdvanceSpawnInterval;
                break;
        }
    }

    public void TrySpawnMeteor()
    {
        meteorSpawnTimer -= Time.deltaTime;
        if (meteorSpawnTimer > 0f) return;
        SpawnMeteor();
        switch (GameManager.Instance.CurrentPhase)
        {
            case GameManager.Phase.Phase1:
                meteorSpawnTimer = phase1MeteorSpawnInterval;
                break;
            case GameManager.Phase.Phase2:
                meteorSpawnTimer = phase2MeteorSpawnInterval;
                break;
            case GameManager.Phase.Phase3:
                meteorSpawnTimer = phase3MeteorSpawnInterval;
                if (phase3MeteorSpawnInterval > phase3MinMeteorSpawnInterval)
                {
                    phase3MeteorSpawnInterval -= phase3MeteorTimerReduce;
                }
                break;
            case GameManager.Phase.Boss:
                meteorSpawnTimer = phase3MeteorSpawnInterval;
                break;
        }
    }

    private void SpawnBaseEnemy()
    {
        spawnPosition = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = GetRandomPoint(player.position, minBaseEnemySpawnRadius, maxBaseEnemySpawnRadius);
            if (CanSummonMonster(randomPos))
            {
                spawnPosition = randomPos;
                break;
            }
        }
        if (spawnPosition == Vector3.zero) return;

        if (warningPrefab != null)
        {
            WaveWarning();
            Invoke(nameof(WaveWarning), 0.3f);
        }
        Instantiate(BaseEnemyPrefab, spawnPosition, Quaternion.identity, GameManager.Instance.EnemyPool);
        aliveEnemies++;
    }
    private void SpawnAdvanceEnemy()
    {
        spawnadvancePosition = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = GetRandomPoint(player.position, minBaseEnemySpawnRadius, maxBaseEnemySpawnRadius);
            if (CanSummonMonster(randomPos))
            {
                spawnadvancePosition = randomPos;
                break;
            }
        }
        if (spawnadvancePosition == Vector3.zero) return;

        GetRandomPoint(player.position, minAdvanceEnemySpawnRadius, maxAdvanceEnemySpawnRadius);

        SoundManager.Instance.PlaySfx(Sound.Enemy2_Warning, 0.7f);
        Instantiate(AdvanceEnemyPrefab, spawnadvancePosition, Quaternion.identity, GameManager.Instance.EnemyPool);
        aliveEnemies++;
    }

    private void SpawnMeteor()
    {
        spawnMeteorPosition = Vector3.zero;
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = GetRandomPoint(player.position, minMeteorSpawnRadius, maxMeteorSpawnRadius);
            if (CanSummonMonster(randomPos))
            {
                spawnMeteorPosition = randomPos;
                spawnMeteorPosition.y = 0;
                break;
            }
        }
        if (spawnMeteorPosition == Vector3.zero) return;

        if (warningPrefab != null)
        {
            MeteorWarning();
            Invoke(nameof(MeteorWarning), 0.5f);
        }
        Invoke(nameof(visibleMeteor), 1.0f);
    }

    private void visibleMeteor()
    {
        Instantiate(MeteorPrefab, spawnMeteorPosition, Quaternion.identity, GameManager.Instance.EnemyPool);
    }

    private void WaveWarning()
    {
        Instantiate(warningPrefab, spawnPosition, Quaternion.identity);
    }
    private void MeteorWarning()
    {
        Instantiate(meteorWarningPrefab, spawnMeteorPosition, Quaternion.identity);
    }

    private Vector3 GetRandomPoint(Vector3 center, float minRadius, float maxRadius)
    {
        float angle = Random.Range(0f, Mathf.PI * 2f);
        float radius = Mathf.Sqrt(Random.Range(minRadius * minRadius, maxRadius * maxRadius));

        float x = Mathf.Cos(angle) * radius;
        float z = Mathf.Sin(angle) * radius;

        return new Vector3(center.x + x, center.y, center.z + z);
    }

    private bool CanSummonMonster(Vector3 position)
    {
        LayerMask targetLayer = LayerMask.GetMask("Wall", "Obstacle");
        if (!Physics.CheckSphere(position, 2f, targetLayer))
        {
            LayerMask FloorLayer = LayerMask.GetMask("Floor");
            return Physics.CheckSphere(position, 2f, FloorLayer);
        }
        return false;
    }
}
