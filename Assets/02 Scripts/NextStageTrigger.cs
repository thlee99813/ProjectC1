using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    //[SerializeField] private Transform currentStageWalls;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StageManager.Instance.NextStage();
            gameObject.SetActive(false);
        }
    }
}
