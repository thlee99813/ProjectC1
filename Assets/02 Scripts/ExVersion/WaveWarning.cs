using System;
using UnityEngine;

public class WaveWarning : MonoBehaviour
{

    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float startScale = 0.5f;
    [SerializeField] private float endScale = 2.0f;

    private float timer;
    private Material materialInstance;
    private Color baseColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = Vector3.one * startScale;

        Renderer renderer = GetComponent<Renderer>();
        if(renderer != null )
        {
            materialInstance = renderer.material;
            baseColor = materialInstance.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        float t = timer / lifeTime;

        // 크기 증가
        float scale = Mathf.Lerp(startScale, endScale, t);
        transform.localScale = new Vector3(scale, 0.02f, scale);

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
