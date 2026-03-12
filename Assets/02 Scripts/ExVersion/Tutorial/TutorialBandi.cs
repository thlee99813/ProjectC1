using UnityEngine;
using UnityEngine.AI;

public class TutorialBandi : MonoBehaviour
{
    private NavMeshAgent agent;

    GameObject destinaton;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destinaton = GameObject.Find("Stone_Trigger");
        if (DollyManager.Instance.middleflag == true)
        {
            destinaton = GameObject.Find("Stone_Exit");
        }

        if (destinaton != null && agent.isOnNavMesh)
        {
            agent.SetDestination(destinaton.transform.position);
        }
    }
    private void Update()
    {
        if (destinaton == null)
        {
            Destroy(gameObject);
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            OnReachedDestination();
        }
    }

    private void OnReachedDestination()
    {
        // 도착 시 파괴
        Destroy(gameObject);
    }


}
