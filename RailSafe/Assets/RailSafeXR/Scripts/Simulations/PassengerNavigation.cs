

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class PassengerNavigation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Coroutine routeCoroutine;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void FollowRoute(Vector3[] waypoints)
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError(name + ": No route waypoints supplied.");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(name + " is not on the NavMesh.");
            return;
        }

        if (routeCoroutine != null)
        {
            StopCoroutine(routeCoroutine);
        }

        agent.isStopped = false;
        routeCoroutine = StartCoroutine(FollowRouteRoutine(waypoints));
    }

    
private IEnumerator FollowRouteRoutine(Vector3[] waypoints)
{
    for (int i = 0; i < waypoints.Length; i++)
    {
        agent.isStopped = false;

        // Skip a waypoint if the passenger is already close to it.
        if (Vector3.Distance(transform.position, waypoints[i]) <=
            agent.stoppingDistance + 0.25f)
        {
            Debug.Log(name + ": Already near waypoint " + (i + 1));
            continue;
        }

        // Calculate a fresh path to this waypoint.
        NavMeshPath path = new NavMeshPath();

        bool calculated = agent.CalculatePath(waypoints[i], path);

        if (!calculated ||
            path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogError(
                name + ": Cannot calculate complete path to waypoint " +
                (i + 1) + ". Status: " + path.status
            );

            yield break;
        }

        // Assign the calculated path.
        if (!agent.SetPath(path))
        {
            Debug.LogError(
                name + ": Could not assign path to waypoint " + (i + 1)
            );

            yield break;
        }

        Debug.Log(
            name + ": Moving to waypoint " + (i + 1) +
            " of " + waypoints.Length
        );

        float elapsed = 0f;
        float timeout = 90f;

        while (true)
        {
            if (agent.pathPending)
            {
                yield return null;
                continue;
            }

            float distance = agent.remainingDistance;

            if (distance <= agent.stoppingDistance + 0.1f)
            {
                break;
            }

            elapsed += Time.deltaTime;

            if (elapsed >= timeout)
            {
                Debug.LogWarning(
                    name + ": Timed out at waypoint " + (i + 1) +
                    " | Position: " + transform.position +
                    " | Target: " + waypoints[i] +
                    " | Remaining Distance: " + agent.remainingDistance +
                    " | Velocity: " + agent.velocity
                );

                yield break;
            }

            yield return null;
        }

        Debug.Log(name + ": Reached waypoint " + (i + 1));
    }

    agent.isStopped = true;

    Debug.Log(name + ": Reached final platform destination.");

    routeCoroutine = null;
}

}
