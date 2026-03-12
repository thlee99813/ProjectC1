using UnityEngine;

public class TutorialExit : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            {
                DollyManager.Instance.MainSceneLoad();
            }    
    }
}
