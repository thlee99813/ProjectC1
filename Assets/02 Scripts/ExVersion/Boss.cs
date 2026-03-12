using System.Collections;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Boss : MonoBehaviour
{
    PlayerMove pm;

    [SerializeField] float fadeTime = .5f;
    [SerializeField] float blindTime = 2f;
    [SerializeField] float lightIntensity = 15f;
    [SerializeField] float lightViewRadius = 1f;
    [SerializeField] float speed = 1f;

    public GameObject bossObstacle;

    float baseIntensity;
    float baseViewRadius;

    bool test = true;
    bool moving;

    private void OnEnable()
    {
        pm = PlayerMove.instance;
        moving = false;
        CameraManager.Instance.PlayImpulseBurst();
        Debug.Log("Boss Enabled");
        MoveStart();
    }

    private void Update()
    {

        if (moving)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, PlayerMove.instance.transform.position, speed * Time.deltaTime);
        }
        float distance = Vector3.Distance(PlayerMove.instance.transform.position, transform.position) - 60;
        if (distance > 18) speed = 5f;
        else if (distance > 12) speed = 4.7f;
        else if (distance > 6) speed = 4.5f;
        else if (distance > 2) speed = 4.3f;
    }
    void Blind()
    {
        baseViewRadius = pm.viewRadius;
        baseIntensity = pm.flashLight.intensity;
        //StartCoroutine(CoFadeIn(fadeTime));

    }

    //��ġ, ������, ����, true
    public void SpawnObstacle()
    {
        StartCoroutine(CoSpawnObstacle());
        Invoke("testing", 1f);

    }

    public void MoveStart()
    {
        moving = true;
        //Blind();
        SpawnObstacle();
    }

    public void MoveStop()
    {
        moving = false;
        //StopCoroutine(CoFadeIn(fadeTime));
        StopCoroutine(CoSpawnObstacle());
    }

    // �� ������
    IEnumerator CoFadeIn(float time)
    {
        while (true)
        {
            float elapsedTime = 0f; // ���� ��� �ð�

            while (elapsedTime <= time)
            {
                pm.flashLight.intensity = Mathf.Lerp(baseIntensity, lightIntensity, elapsedTime / time);
                pm.viewRadius = Mathf.Lerp(baseViewRadius, lightViewRadius, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(blindTime);

            elapsedTime = 0f;

            while (elapsedTime <= time)
            {
                pm.flashLight.intensity = Mathf.Lerp(lightIntensity, baseIntensity, elapsedTime / time);
                pm.viewRadius = Mathf.Lerp(lightViewRadius, baseViewRadius, elapsedTime / time);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            test = true;

            yield return new WaitForSeconds(Random.Range(3.0f, 5.0f));
        }

    }

    IEnumerator CoSpawnObstacle()
    {
        while (true)
        {
            float randX = Random.Range(12.0f, 17.0f);
            float randZ = Random.Range(15.0f, 20.0f);
            float rotateY = Random.Range(-30f, 30f);

            Vector3 randomPos = new Vector3(PlayerMove.instance.transform.forward.x * -1 * randX + PlayerMove.instance.transform.position.x, 0, PlayerMove.instance.transform.forward.z * -1 * randZ + PlayerMove.instance.transform.position.z);
            Vector3 randomRotate = new Vector3(PlayerMove.instance.transform.eulerAngles.x, PlayerMove.instance.transform.eulerAngles.y + rotateY, PlayerMove.instance.transform.eulerAngles.z);
            int limiter = 0;
            while (limiter <= 30)
            {
                if (MapManager.Instance.GenerateObstacle(randomPos, bossObstacle, randomRotate, true))
                {
                    yield return new WaitForSeconds(Random.Range(4.0f, 6.0f));
                }
                limiter++;
            }
            yield return new WaitForSeconds(Random.Range(.3f, 1.0f));
        }

    }
}
