
using UnityEngine;

public class CrowdDensityDashboard : MonoBehaviour
{
    [SerializeField]
    private CrowdDensityMonitor densityMonitor;

    private GUIStyle titleStyle;
    private GUIStyle rowStyle;

    private void Awake()
    {
        titleStyle = new GUIStyle
        {
            fontSize = 20,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.white }
        };

        rowStyle = new GUIStyle
        {
            fontSize = 15,
            normal = { textColor = Color.white }
        };
    }

    private void OnGUI()
    {
        if (densityMonitor == null)
            return;

        GUILayout.BeginArea(new Rect(15, 15, 330, 250));

        GUILayout.BeginVertical("box");

        GUILayout.Label("RAILSAFE XR - CROWD MONITOR", titleStyle);

        GUILayout.Space(10);

        foreach (var zone in densityMonitor.Zones)
        {
            if (zone == null)
                continue;

            GUILayout.Label(
                zone.zoneId +
                " | Passengers: " + zone.passengerCount +
                " | Occupancy: " +
                zone.occupancyPercentage.ToString("F1") + "%" +
                " | " + zone.densityLevel,
                rowStyle
            );
        }

        GUILayout.EndVertical();
        GUILayout.EndArea();
    }
}
