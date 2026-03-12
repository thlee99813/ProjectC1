using UnityEngine;
using UnityEngine.AI;

public class Bandi : MonoBehaviour
{
    private NavMeshAgent agent;

    GameObject destinaton;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        destinaton = GameObject.Find("Stone_Ground_Trigger");
        if (GameManager.Instance.CurrentPhase == GameManager.Phase.Boss)
        {
            destinaton = GameObject.Find("Stone_Ground_ExitDoor");
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
