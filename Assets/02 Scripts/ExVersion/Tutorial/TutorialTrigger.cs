using UnityEngine;
using System.Collections;

public class TutorialTrigger : MonoBehaviour
{
    public GameObject TargetWall;
    public GameObject TargetExit;
    public float duration = 2f;
    public float exitDuration = 1.5f;
    private bool triggerflag = false;

    void OnTriggerEnter(Collider other)
    {
        if (triggerflag) return;

        if (other.CompareTag("Player") || other.CompareTag("Bullet"))
        {
            triggerflag = true;
            PlayerMove.instance.isMoveable = false;
            StartCoroutine(WallTrigger());

        }
    }

    private IEnumerator WallTrigger()
    {
        float elapsed = 0f;

        Vector3 startposition = TargetWall.transform.localPosition;
        Vector3 endposition = new Vector3(startposition.x, -0.5f, startposition.z);

        TargetWall.transform.localPosition = new Vector3(startposition.x, 0.5f, startposition.z);
        startposition = TargetWall.transform.localPosition;
        CameraManager.Instance.PlayImpulseBurst();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float time = Mathf.Clamp01(elapsed / duration);
            TargetWall.transform.localPosition = Vector3.Lerp(startposition, endposition, time);
            yield return null;
        }
        TargetWall.transform.localPosition = endposition;
        DollyManager.Instance.Vcam1.gameObject.SetActive(false);
        DollyManager.Instance.Vcam2.gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        yield return StartCoroutine(ExitTrigger());
        yield return new WaitForSeconds(2.0f);

        PlayerMove.instance.isMoveable = true;
        DollyManager.Instance.Vcam1.gameObject.SetActive(true);
        DollyManager.Instance.Vcam2.gameObject.SetActive(false);
        DollyManager.Instance.Vcam1.Follow = DollyManager.Instance.Dollyplayer.transform;
        DollyManager.Instance.middleflag = true;



    }

    private IEnumerator ExitTrigger()
    {
        float elapsed = 0f;

        Vector3 startposition = TargetExit.transform.localPosition;
        Vector3 endposition = new Vector3(startposition.x, 1.92f, startposition.z);

        TargetExit.transform.localPosition = new Vector3(startposition.x, -1.92f, startposition.z);
        startposition = TargetExit.transform.localPosition;

        while (elapsed < exitDuration)
        {
            elapsed += Time.deltaTime;
            float time = Mathf.Clamp01(elapsed / exitDuration);
            TargetExit.transform.localPosition = Vector3.Lerp(startposition, endposition, time);
            yield return null;
        }



        TargetExit.transform.localPosition = endposition;
    }

}
