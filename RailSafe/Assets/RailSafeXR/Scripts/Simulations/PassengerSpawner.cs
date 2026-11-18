
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PassengerSpawner : MonoBehaviour
{
    [Header("Passenger Settings")]
    [SerializeField] private GameObject passengerPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Station Route")]
    [SerializeField] private Transform stationEntrance;
    [SerializeField] private Transform centralConcourse;
    [SerializeField] private Transform[] destinations;

    [Header("Spawn Settings")]
    [SerializeField] private int passengerCount = 10;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private float spawnRadius = 5f;

    [Header("Destination Distribution")]
    [SerializeField] private float destinationSpreadRadius = 1.5f;

    private IEnumerator Start()
    {
        if (passengerPrefab == null ||
            spawnPoint == null ||
            stationEntrance == null ||
            centralConcourse == null ||
            destinations == null ||
            destinations.Length == 0)
        {
            Debug.LogError(
                "PassengerSpawner: Assign the prefab, spawn point, " +
                "entrance, concourse and platform destinations."
            );
            yield break;
        }

        for (int i = 0; i < passengerCount; i++)
        {
            // Find a valid NavMesh position near the spawn point.
            Vector2 spawnOffset =
                Random.insideUnitCircle * spawnRadius;

            Vector3 requestedSpawn = spawnPoint.position +
                new Vector3(spawnOffset.x, 0f, spawnOffset.y);

            if (!NavMesh.SamplePosition(
                requestedSpawn,
                out NavMeshHit spawnHit,
                2f,
                NavMesh.AllAreas))
            {
                Debug.LogWarning(
                    "PassengerSpawner: No NavMesh near spawn position."
                );

                yield return new WaitForSeconds(spawnInterval);
                continue;
            }

            // Randomly select one of the platform destinations.
            Transform destinationCenter =
                destinations[Random.Range(0, destinations.Length)];

            if (destinationCenter == null)
            {
                Debug.LogError(
                    "PassengerSpawner: A platform destination is not assigned."
                );
                yield break;
            }

            // Spread the final passenger targets across the selected platform.
            Vector2 targetOffset =
                Random.insideUnitCircle * destinationSpreadRadius;

            Vector3 requestedTarget = destinationCenter.position +
                new Vector3(targetOffset.x, 0f, targetOffset.y);

            if (!NavMesh.SamplePosition(
                requestedTarget,
                out NavMeshHit targetHit,
                3f,
                NavMesh.AllAreas))
            {
                Debug.LogWarning(
                    "PassengerSpawner: No NavMesh near platform destination."
                );

                yield return new WaitForSeconds(spawnInterval);
                continue;
            }

            // Verify the entrance waypoint.
            if (!NavMesh.SamplePosition(
                stationEntrance.position,
                out NavMeshHit entranceHit,
                2f,
                NavMesh.AllAreas))
            {
                Debug.LogError(
                    "PassengerSpawner: Station entrance is not near the NavMesh."
                );
                yield break;
            }

            // Verify the concourse waypoint.
            if (!NavMesh.SamplePosition(
                centralConcourse.position,
                out NavMeshHit concourseHit,
                2f,
                NavMesh.AllAreas))
            {
                Debug.LogError(
                    "PassengerSpawner: Central concourse is not near the NavMesh."
                );
                yield break;
            }

            GameObject passenger = Instantiate(
                passengerPrefab,
                spawnHit.position,
                spawnPoint.rotation
            );

            passenger.name = "Passenger_" + (i + 1);

            PassengerNavigation navigation =
                passenger.GetComponent<PassengerNavigation>();

            if (navigation == null)
            {
                Debug.LogError(
                    "PassengerSpawner: PassengerNavigation is missing " +
                    "from the passenger prefab."
                );

                Destroy(passenger);
            }
            else
            {
                Vector3[] route =
                {
                    entranceHit.position,
                    concourseHit.position,
                    targetHit.position
                };

                navigation.FollowRoute(route);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}

