
using UnityEngine;
using UnityEngine.InputSystem;

public class EmergencyEvacuationManager : MonoBehaviour
{
    [Header("Evacuation Route")]
    [SerializeField] private Transform centralConcourse;
    [SerializeField] private Transform stationExit;

    [Header("Settings")]
    [SerializeField] private KeyCode unusedLegacyKey;

    private bool evacuationStarted = false;

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            !evacuationStarted)
        {
            StartEvacuation();
        }
    }

    public void StartEvacuation()
    {
        if (evacuationStarted)
            return;

        if (centralConcourse == null || stationExit == null)
        {
            Debug.LogError(
                "Evacuation Manager: Assign Central Concourse and Station Exit."
            );
            return;
        }

        PassengerNavigation[] passengers =
            FindObjectsByType<PassengerNavigation>(
                FindObjectsSortMode.None
            );

        if (passengers.Length == 0)
        {
            Debug.LogWarning("No passengers found to evacuate.");
            return;
        }

        evacuationStarted = true;

        Debug.Log(
            "EMERGENCY EVACUATION STARTED. Passengers: " +
            passengers.Length
        );

        foreach (PassengerNavigation passenger in passengers)
        {
            if (passenger == null)
                continue;

            Vector3[] evacuationRoute =
            {
                centralConcourse.position,
                stationExit.position
            };

            passenger.FollowRoute(evacuationRoute);
        }
    }
}
