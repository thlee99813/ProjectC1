using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy2 : MonoBehaviour
{
    private Transform player;
    private string playerTag = "Player";
    private NavMeshAgent agent;
    private Renderer rend;

    [Header("Speed Settings")]
    [SerializeField] private float maxSpeedTime = 2f; // 최고 속도까지 도달하는 데 걸리는 시간
    [SerializeField] private float maxSpeed = 10f;    // 최고 속도
    private float speedTimer = 0f; // 가속도용 타이머

    [Header("Light Settings")]
    public bool speedFlag = false; //빛 여부
    [SerializeField] private float timeToDie = 1.0f; // 타죽는 데 걸리는 시간
    private float lightTimer = 0f; // 빛을 받은 누적 시간

    [SerializeField] private int Enemy_Health = 3;

    public GameObject healItem;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        if (rend != null) rend.enabled = true;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        rend.enabled = true;
        if (player == null) return;
        HandleLightAndSpeed(); // 스피드와 빛 로직 처리

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }

    private void HandleLightAndSpeed()
    {
        if (speedFlag == true) //  빛을 받고 있을 때
        {
            // 빛 누적 타이머 증가
            lightTimer += Time.deltaTime;
            float colorRatio = lightTimer / timeToDie;
            if (rend != null) rend.material.color = Color.Lerp(Color.black, Color.white, colorRatio);
            if (lightTimer >= timeToDie)
            {
                Instantiate(healItem, transform.position, Quaternion.identity);
                SoundManager.Instance.PlaySfx(Sound.Item_Spawn, 0.25f);
                Destroy(gameObject);
            }
            agent.speed = 0.5f;
            speedTimer = 0f;
        }
        else // 빛을 받지 않을 때 (평상시)
        {
            
            speedTimer += Time.deltaTime;         
            float speedRatio = speedTimer / maxSpeedTime;         
            agent.speed = Mathf.Lerp(1.5f, maxSpeed, Mathf.Clamp01(speedRatio));
        }
    }

}