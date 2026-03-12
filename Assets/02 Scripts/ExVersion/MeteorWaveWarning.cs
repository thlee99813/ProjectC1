using UnityEngine;

public class MeteorWaveWarning : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float startScaleX = 8.0f;
    [SerializeField] private float startScaleZ = 5.0f;
    [SerializeField] private float endScaleX = 2.0f;
    [SerializeField] private float endScaleZ = 1.25f;

    private float timer;
    private Material materialInstance;
    private Color baseColor;

    void Start()
    {
        transform.localScale = new Vector3(startScaleX, transform.localScale.y, startScaleZ);
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            materialInstance = renderer.material;
            baseColor = materialInstance.color;
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifeTime;

        // 크기 증가
        float scaleX = Mathf.Lerp(startScaleX, endScaleX, t);
        float scaleZ = Mathf.Lerp(startScaleZ, endScaleZ, t);
        transform.localScale = new Vector3(scaleX, 0.01f, scaleZ);

        // 투명도 감소
        if (materialInstance != null)
        {
            Color c = baseColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            materialInstance.color = c;
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }
}
