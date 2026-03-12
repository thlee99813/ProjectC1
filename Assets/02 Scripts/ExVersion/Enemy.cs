using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using static UnityEngine.GraphicsBuffer;
using TMPro;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private Transform player;
    private string playerTag = "Player";
    private NavMeshAgent agent;
    private Renderer rend;

    //[SerializeField] private int Enemy_Health = 3;

    //[Header("Feedback Settings")]
    [SerializeField] private MeshRenderer meshRenderer;
    //[SerializeField] private Color flashColor = Color.white;
    //[SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private Color deadColor = Color.black;
    //[Range(0f, 1f)]
    //[SerializeField] private float emissionIntensity = 0.5f;


    //[Header("Speed Settings")]
    //[SerializeField] private float maxSpeedTime = 2f; // 최고 속도까지 도달하는 데 걸리는 시간
    //[SerializeField] private float maxSpeed = 10f;    // 최고 속도
    private float speedTimer = 0f; // 가속도용 타이머

    [Header("Light Settings")]
    public bool speedFlag = false; //빛 여부
    [SerializeField] private float timeToDie = 1.0f; // 타죽는 데 걸리는 시간
    private float lightTimer = 0f; // 빛을 받은 누적 시간


    private Coroutine hitFeedbackCoroutine;
    private Color fullHealthColor;
    private Color initialEmissionColor;
    private int maxHealth;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();
        meshRenderer = GetComponent<MeshRenderer>();
        rend.enabled = false;
    }

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        player = p.transform;
       
       // maxHealth = Enemy_Health;
        if (meshRenderer != null)
        {
            fullHealthColor = meshRenderer.material.color;
            initialEmissionColor = meshRenderer.material.GetColor("_EmissionColor");
        }
    }

    private void Update()
    {
       
        if (player == null) return;
        HandleLightAndSpeed(); // 스피드와 빛 로직 처리
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }
    }
    /*
    private void UpdateVisuals()
    {
        if (meshRenderer == null) return;
        float healthRatio = (float)Enemy_Health / maxHealth;
        Color currentHealthColor = Color.Lerp(deadColor, fullHealthColor, healthRatio);

        eshRenderer.material.color = currentHealthColor;
    }*/

    //public void TakeDamage(int damage)
    //{
    //    Enemy_Health -= damage;

    //    if (Enemy_Health <= 0)
    //    {
    //        SoundManager.Instance.PlaySfx(Sound.Enemy_Die, 0.7f);
    //        Destroy(gameObject);
    //    }

    //    // 히트 피드백 추가
    //    if (hitFeedbackCoroutine != null)
    //    {
    //        StopCoroutine(hitFeedbackCoroutine);
    //    }

    //    hitFeedbackCoroutine = StartCoroutine(HitFeedbackRoutine());
    //}

    private void HandleLightAndSpeed()
    {
        if (speedFlag == true) //  빛을 받고 있을 때
        {
            // 빛 누적 타이머 증가
            lightTimer += Time.deltaTime;
            float colorRatio = lightTimer / timeToDie;
            if (rend != null) rend.material.color = Color.Lerp( fullHealthColor, deadColor, colorRatio);
            if (lightTimer >= timeToDie)
            {
                
              
                Destroy(gameObject);
            }
            agent.speed = 3.5f;
            speedTimer = 0f;
        }
        else // 빛을 받지 않을 때 (평상시)
        {

        //    speedTimer += Time.deltaTime;
        //    float speedRatio = speedTimer / maxSpeedTime;
        //    agent.speed = Mathf.Lerp(1.5f, maxSpeed, Mathf.Clamp01(speedRatio));
        }
    }

    //private IEnumerator HitFeedbackRoutine()
    //{
    //    Color intenseFlashColor = flashColor * emissionIntensity;

    //    // 플래시
    //    meshRenderer.material.color = flashColor;
    //    meshRenderer.material.SetColor("_EmissionColor", intenseFlashColor);
    //    yield return new WaitForSeconds(flashDuration);

    //    // 현재 체력에 맞는 색상으로 변경
    //    float elapsed = 0f;
    //    float duration = flashDuration;

    //    float healthRatio = (float)Enemy_Health / maxHealth;
    //    Color targetColor = Color.Lerp(deadColor, fullHealthColor, Mathf.Max(0, healthRatio - 0.2f));
    //    Color startColor = meshRenderer.material.color;

    //    while (elapsed < duration)
    //    {
    //        elapsed += Time.deltaTime;
    //        float t = elapsed / duration;
    //        meshRenderer.material.color = Color.Lerp(startColor, targetColor, t);

    //        Color currentEmission = Color.Lerp(intenseFlashColor, initialEmissionColor, t);
    //        meshRenderer.material.SetColor("_EmissionColor", currentEmission);
    //        yield return null;
    //    }

    //    meshRenderer.material.color = targetColor;
    //    meshRenderer.material.SetColor("_EmissionColor", initialEmissionColor);
    //}
}