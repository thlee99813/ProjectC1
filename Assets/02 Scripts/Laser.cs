using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Laser : MonoBehaviour
{
    [SerializeField] private float laserLength = 25f;
    [SerializeField] private float laserSpeed = 5f;
    [SerializeField] public bool Isactive = false;

    [SerializeField] private int maxBounce = 5;
    [SerializeField] private float hitOffset = 0.01f;


    [SerializeField] private LineRenderer line;
    private Vector3 startPos;
    private Vector3 endPos;
    private float laserT;
    private bool laserEnd;


    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        StopAllCoroutines();

        Isactive = true;

        startPos = transform.position;
        endPos = transform.position + transform.forward * laserLength;
        laserT = 0f;

        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, startPos);
    }

    void OnDisable()
    {
        Isactive = false;
    }

    void Update()
    {
        laserT += Time.deltaTime * laserSpeed / laserLength;
        laserT = Mathf.Clamp01(laserT);

        Vector3 nowEnd = Vector3.Lerp(startPos, endPos, laserT);

        line.SetPosition(0, startPos);
        line.SetPosition(1, nowEnd);

        if (laserT >= 1f)
        {
            laserEnd = true;
            StartCoroutine(HideLaser());
        }
    }

    IEnumerator HideLaser()
    {
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}
