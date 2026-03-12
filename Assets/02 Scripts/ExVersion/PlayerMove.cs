using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.ExtrusionShapes;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove instance;

    private bool normalizeDiagonal = true;
    private Transform rotateTarget;

    public float viewRadius = 8f;
    [SerializeField] private float viewAngle = 60f;
    [SerializeField] private float maxReducibleAngle = 30f; // 최대로 줄어들 수 있는 각도
    [SerializeField] private Vector3 totalLightElevation = new Vector3(0.001f, 0.2f, -0.55f);
    [SerializeField] private Transform viewOrigin;
    [SerializeField] private bool useObstacle = false;
    [SerializeField] private float updateRate = 30f;
    [SerializeField] private float nearRadius = 1.5f;
    private float timer;
    private readonly HashSet<Renderer> visibleNow = new HashSet<Renderer>();

    //���� ������ �浹�� ���̾�
    public LayerMask enemyMask;
    public LayerMask obstacleMask;
    public bool isMoveable = true;
    public float moveSpeed = 5f;
    public float maxRotateSpeed = 360f;
    public Light flashLight;

    public int currentHp = 3;
    private int maxHp;
    private float hpRatio => (float)currentHp / maxHp;

    private Vector3 direct;
    private Vector3 lightElevation;
    private Vector2 input;
    private Rigidbody rigid;

    private float lightRatio;

    // 초기값
    private float initialSpotAngle;
    private float initialInnerAngle;
    private float initialViewAngle;
    private Vector3 initialElevation;
    private Vector3 initialPosition;
    private Color initialColor;
    private Color initialEmissionColor;

    [Header("Feedback Color Settings")]
    [SerializeField] private MeshRenderer meshRenderer; // 플레이어 메쉬
    [SerializeField] private float recoverDuration = 0.2f;

    private Coroutine hitFeedbackCoroutine;

    [Header("Invincible Settings")]
    [SerializeField] private float invincibilityDuration = 1.5f; // 무적 지속 시간
    public bool isInvincible = false;
    [SerializeField] private bool isTutorial = false;


    private void Awake()
    {
        instance = this;
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (rotateTarget == null) rotateTarget = transform;

        if (viewOrigin == null) viewOrigin = transform;

        //���׹� ������ ���ֱ�
        HideAllTargetRenderers();
        maxHp = currentHp;

        initialSpotAngle = flashLight.spotAngle;
        initialInnerAngle = flashLight.innerSpotAngle;
        initialViewAngle = viewAngle;
        initialPosition = flashLight.transform.localPosition;
        initialElevation = flashLight.gameObject.transform.localPosition;
        initialColor = meshRenderer.material.color;
        meshRenderer.material.EnableKeyword("_EMISSION");
        initialEmissionColor = meshRenderer.material.GetColor("_EmissionColor");

        lightRatio = maxReducibleAngle / maxHp;
        Debug.Log(flashLight.gameObject.transform.localPosition);
        lightElevation = (initialElevation - totalLightElevation) / maxHp;
        Debug.Log(lightElevation.x + ", " + lightElevation.y + ", " + lightElevation.z);
    }
    private void Start()
    {
    }

    private void Update()
    {
        direct = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        direct.Normalize(); // �밢������ �ӵ� ����

        if (normalizeDiagonal && input.sqrMagnitude > 1f)
            input = input.normalized;
        if (isMoveable)
        {
            LookMousePoint();
            flashLight.enabled = true;
        }
        else
            flashLight.enabled = false;

        float interval = 1f / updateRate;
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            UpdateVisibility();
        }
    }

    private void FixedUpdate()
    {
        if (isMoveable)
            HandleMovement();
        else
            rigid.linearVelocity = Vector3.zero;
    }

    private void HandleMovement()
    {
        rigid.AddForce(direct * moveSpeed / 2, ForceMode.Impulse);

        if (direct.x == 0)
            rigid.linearVelocity = new Vector3(rigid.linearVelocity.x * .3f, 0, rigid.linearVelocity.z);
        if (direct.z == 0)
            rigid.linearVelocity = new Vector3(rigid.linearVelocity.x, 0, rigid.linearVelocity.z * .3f);
        if (rigid.linearVelocity.x > moveSpeed)
            rigid.linearVelocity = new Vector2(moveSpeed, rigid.linearVelocity.z);
        else if (rigid.linearVelocity.x < moveSpeed * -1)
            rigid.linearVelocity = new Vector2(moveSpeed * -1, rigid.linearVelocity.z);
        if (rigid.linearVelocity.z > moveSpeed)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, moveSpeed);
        else if (rigid.linearVelocity.z < moveSpeed * -1)
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.x, moveSpeed * -1);
        if (Mathf.Abs(rigid.linearVelocity.x) < 0.5f)
            rigid.linearVelocity = new Vector3(0, 0, rigid.linearVelocity.z);
        if (Mathf.Abs(rigid.linearVelocity.z) < 0.5f)
            rigid.linearVelocity = new Vector3(rigid.linearVelocity.x, 0, 0);
    }

    public void LookMousePoint()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;
        if (plane.Raycast(cameraRay, out rayLength))
        {
            Vector3 dir = cameraRay.GetPoint(rayLength);
            dir.y = transform.position.y;
            float mouseBlock = Vector3.Distance(dir, transform.position);
            if (mouseBlock >= .45f)
                transform.LookAt(new Vector3(dir.x, transform.position.y, dir.z));
        }
    }
    //���� �������� ���ִ� �ڵ�
    private void HideAllTargetRenderers()
    {
        Renderer[] all = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        for (int i = 0; i < all.Length; i++)
        {
            Renderer r = all[i];
            if (r == null) continue;

            int layerBit = 1 << r.gameObject.layer;
            if ((enemyMask.value & layerBit) != 0)
                r.enabled = false;
        }
    }

    private void UpdateVisibility()
    {
        //hashSet �ʱ�ȭ
        foreach (var r in visibleNow)
        {
            if (r != null) r.enabled = false;
        }
        visibleNow.Clear();

        //�� ��ġ
        Vector3 originPos = viewOrigin.position;

        //physics.OverlapSphere -> �� ������ ������ ������ Ư�� ����ũ�� Ž���ϴ� �Լ�
        //physics.OverlapSphere(�߽� ��ġ, ������, Ž���� Ư�� ����ũ, Trigger Collider�� ���� ���� {QueryTriggerInteraction.Collide -> Trigger����}
        Collider[] hits = Physics.OverlapSphere(originPos, viewRadius, enemyMask, QueryTriggerInteraction.Collide);
        Collider[] nearObj = Physics.OverlapSphere(originPos, nearRadius);
        for (int i = 0; i < nearObj.Length; i++)
        {
            Collider c = nearObj[i];
            if (c == null || c.gameObject.tag == "Wall" || c.gameObject.tag == "Bullet" || c.gameObject.tag == "Obstacle" || c.gameObject.tag == "Floor" || c.gameObject.tag == "BossAttack") continue;
            Renderer r = c.GetComponentInChildren<Renderer>();
            if (r == null) continue;
            r.enabled = true;
            visibleNow.Add(r);
        }
        Vector3 forward = viewOrigin.forward;
        forward.y = 0f;
        forward.Normalize();
        float halfAngle = viewAngle * 0.5f;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider c = hits[i];
            if (c == null) continue;
            Renderer r = c.GetComponentInChildren<Renderer>();
            if (r == null) continue;
            //r.bounds.center -> ������ �߽��� ���Ͱ�
            //Ÿ���� ����
            Vector3 toTarget = r.bounds.center - originPos;
            toTarget.y = 0f;

            //Ÿ�ٱ����� �Ÿ��� ������
            float sqrDist = toTarget.sqrMagnitude;

            if (sqrDist < 0.0001f) continue;

            //Ÿ���� �����ȿ� ���� �ִ��� �Ǵ�(�÷��̾��� ������ �������� ������ ������ halfAngle�ȿ� ������ �Ǵ�)
            float angle = Vector3.Angle(forward, toTarget.normalized);
            if (angle > halfAngle)
            {
                if (r.gameObject.CompareTag("Enemy2"))
                {
                    r.GetComponent<Enemy2>().speedFlag = false;
                }
                if (r.gameObject.CompareTag("Enemy"))
                {
                    r.GetComponent<Enemy>().speedFlag = false;
                }
                continue;
            }


            if (useObstacle)
            {
                Ray ray = new Ray(originPos, (r.bounds.center - originPos).normalized);
                float dist = Mathf.Sqrt(sqrDist);
                Debug.DrawRay(ray.origin, ray.direction * dist);
                if (Physics.Raycast(ray, dist, obstacleMask, QueryTriggerInteraction.Collide))
                    continue;
            }

            if (r.gameObject.CompareTag("Enemy2"))
            {
                r.GetComponent<Enemy2>().speedFlag = true;
            }
            if (r.gameObject.CompareTag("Enemy"))
            {
                r.GetComponent<Enemy>().speedFlag = true;
            }

            r.enabled = true;
            //���� Ž���� �ȵ� �� ���� �� �ֵ��� �������� ����
            visibleNow.Add(r);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "BossAttack" || collision.gameObject.tag == "Enemy2") && (currentHp > 0) && !isTutorial)
        {
            Destroy(collision.gameObject);
            SoundManager.Instance.PlaySfx(Sound.Player_Hit, 0.8f);
            ChangeHealth(-1);
        }
        if ((collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Enemy2") && isTutorial)
        {
            Destroy(collision.gameObject);
            SoundManager.Instance.PlaySfx(Sound.Player_Hit, 0.8f);
            CameraManager.Instance.PlayImpulseBurst(1);
            if (hitFeedbackCoroutine != null) StopCoroutine(hitFeedbackCoroutine);
            hitFeedbackCoroutine = StartCoroutine(HitFeedbackRoutine());        
        }

        if ((collision.gameObject.tag == "Item") && (currentHp <= 3))
        {
            if (currentHp == 3)
            {
                Destroy(collision.gameObject);
                SoundManager.Instance.PlaySfx(Sound.Item_PickUp, 0.1f);
                return;
            }

            Destroy(collision.gameObject);
            SoundManager.Instance.PlaySfx(Sound.Item_PickUp, 0.1f);
            ChangeHealth(+1);
        }

        if (currentHp <= 0)
        {
            GameManager.Instance.SetPhase(GameManager.Phase.Death);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "BossAttack")
        {
            ChangeHealth(-1);
        }
    }

    public void ChangeHealth(int amount)
    {
        // 무적 처리
        if (isInvincible && amount < 0) return;

        currentHp = Mathf.Clamp(currentHp + amount, 0, maxHp);
        UIManager.Instance.UpdateLifeText(currentHp);
        UpdateVisuals();

        // 대미지 피드백
        if (amount < 0 && currentHp > 0)
        {
            UIManager.Instance.TriggerDamageFlash();
            CameraManager.Instance.PlayImpulseBurst(1);
            if (hitFeedbackCoroutine != null) StopCoroutine(hitFeedbackCoroutine);
            hitFeedbackCoroutine = StartCoroutine(HitFeedbackRoutine());
        }

        if (currentHp <= 0)
        {
            // TODO : 죽는거
            Debug.Log("죽음");
            // GameManager.Instance.SetPhase(GameManager.Phase.Death);
        }

    }

    

    private IEnumerator HitFeedbackRoutine()
    {
        isInvincible = true;

        meshRenderer.material.color = Color.white;
        meshRenderer.material.SetColor("_EmissionColor", Color.white);
        yield return new WaitForSeconds(0.1f);

        float elapsed = 0f;
        while (elapsed < recoverDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoverDuration;
            meshRenderer.material.color = Color.Lerp(Color.white, initialColor, t);

            Color currentEmission = Color.Lerp(Color.white, initialEmissionColor, t);
            meshRenderer.material.SetColor("_EmissionColor", currentEmission);
            yield return null;
        }
        meshRenderer.material.color = initialColor;
        meshRenderer.material.SetColor("_EmissionColor", initialEmissionColor);

        float remainingInvincibleTime = invincibilityDuration - (0.1f + recoverDuration);
        if (remainingInvincibleTime > 0)
        {
            yield return new WaitForSeconds(remainingInvincibleTime);
        }

        isInvincible = false;
        hitFeedbackCoroutine = null;
    }

    public void UpdateVisuals()
    {
        int lostHp = maxHp - currentHp;
        float totalReduction = lostHp * lightRatio;

        flashLight.spotAngle = initialSpotAngle - totalReduction;
        flashLight.innerSpotAngle = initialInnerAngle - totalReduction;
        viewAngle = initialViewAngle - totalReduction;

        flashLight.transform.localPosition = Vector3.Lerp(
            initialElevation - totalLightElevation,
            initialElevation,
            hpRatio
        );
    }
}
