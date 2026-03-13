using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float laserLength = 25f;
    [SerializeField] private float laserSpeed = 5f;

    private LineRenderer line;
    private Vector3 startPos;
    private Vector3 endPos;
    private float laserT;

    void Awake()
    {
        line = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        startPos = transform.position;
        endPos = transform.position + transform.forward * laserLength;
        laserT = 0f;

        line.positionCount = 2;
        line.SetPosition(0, startPos);
        line.SetPosition(1, startPos);
    }

    void Update()
    {
        laserT += Time.deltaTime * laserSpeed / laserLength;
        laserT = Mathf.Clamp01(laserT);

        Vector3 nowEnd = Vector3.Lerp(startPos, endPos, laserT);

        line.SetPosition(0, startPos);
        line.SetPosition(1, nowEnd);
    }
}
