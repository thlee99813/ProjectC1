using UnityEngine;

public class TutorialSpawnTrigger : MonoBehaviour
{
    public int TriggerNumber = 0;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            {
                if(TriggerNumber == 0)
                {
                    TutorialSpawnManager.Instance.spawnflag = true;

                }
                else
                {
                    TutorialSpawnManager.Instance.spawnadvanceflag = true;
                }
            }    
    }
}