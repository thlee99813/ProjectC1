using UnityEngine;
using System.Collections;

public class TutorialSpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject BaseEnemyPrefab;
    [SerializeField] private GameObject AdvanceEnemyPrefab;

    [SerializeField] private GameObject warningPrefab;

    [SerializeField] private GameObject spawnBasePosition;
    [SerializeField] private GameObject spawnAdvancePosition;

    public bool spawnflag = false;
    public bool spawnadvanceflag = false;
    public static TutorialSpawnManager Instance { get; private set; }


    [SerializeField] private GameObject currentBaseEnemy;
    [SerializeField] private GameObject currentAdvanceEnemy;
    private bool baseSpawnPending = false;
    private bool advanceSpawnPending = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    void Update()
    {
        if (spawnflag)
        {
            if (currentBaseEnemy == null && !baseSpawnPending)
            {
                StartCoroutine(SpawnBaseAfterDelay(1.0f));
            }
            
        }
        if  (spawnadvanceflag)
        {
            if (currentAdvanceEnemy == null && !advanceSpawnPending)
            {
                StartCoroutine(SpawnAdvanceAfterDelay(1.0f));
            }
        }
    }

    private IEnumerator SpawnBaseAfterDelay(float delay)
    {
        baseSpawnPending = true;
        yield return new WaitForSeconds(delay);

        if (currentBaseEnemy == null)
        {
            SpawnBaseEnemy();
        }

        baseSpawnPending = false;
    }

    private IEnumerator SpawnAdvanceAfterDelay(float delay)
    {
        advanceSpawnPending = true;
        yield return new WaitForSeconds(delay);

        if (currentAdvanceEnemy == null)
        {
            SpawnAdvanceEnemy();
        }

        advanceSpawnPending = false;
    }

    private void SpawnBaseEnemy()
    {

        if (warningPrefab != null)
        {
            WaveWarning();
            Invoke(nameof(WaveWarning), 0.3f);
        }

        currentBaseEnemy = Instantiate(BaseEnemyPrefab, spawnBasePosition.transform.position, Quaternion.identity);
    }
    private void SpawnAdvanceEnemy()
    {
        SoundManager.Instance.PlaySfx(Sound.Enemy2_Warning, 0.7f);
        currentAdvanceEnemy = Instantiate(AdvanceEnemyPrefab, spawnAdvancePosition.transform.position, Quaternion.identity);
    }
    private void WaveWarning()
    {
        Instantiate(warningPrefab, spawnBasePosition.transform.position, Quaternion.identity);
    }

}
