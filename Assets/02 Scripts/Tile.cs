using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{

    [SerializeField] private float durationvalue = 2f;
    [SerializeField] private float moveHeight = 4f;
    private Coroutine moveRoutine;

    
    public void MoveUp()
        {
            StartMove(moveHeight);
        }

    public void MoveDown()
        {
            StartMove(-moveHeight);
        }

    private void StartMove(float dir)
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(MoveTile(dir));
    }
    private IEnumerator MoveTile(float dir)
        {
            float elapsed = 0f;
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = new Vector3 (startPos.x, startPos.y + dir, startPos.z);
            while (elapsed < durationvalue)
            {
                elapsed += Time.deltaTime;
                float time = Mathf.Clamp01(elapsed / durationvalue);
                transform.localPosition = Vector3.Lerp(startPos, endPos, time);
                yield return null;
            }

            transform.localPosition = endPos;
            moveRoutine = null;
        }
    

}