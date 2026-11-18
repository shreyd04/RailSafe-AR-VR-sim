
using System;
using UnityEngine;
using UnityEngine.AI;

public class CrowdDensityMonitor : MonoBehaviour
{
    public enum DensityLevel
    {
        Low,
        Medium,
        High
    }

    [Serializable]
    public class CrowdZone
    {
        public string zoneId;

        public BoxCollider zoneArea;

        [Min(1)]
        public int capacity = 10;

        [HideInInspector]
        public int passengerCount;

        [HideInInspector]
        public float occupancyPercentage;

        [HideInInspector]
        public DensityLevel densityLevel;
    }

    [Header("Monitoring Settings")]
    [SerializeField] private CrowdZone[] zones;

    [SerializeField] private float updateInterval = 1f;

    [Header("Density Thresholds")]
    [Range(0f, 1f)]
    [SerializeField] private float mediumThreshold = 0.5f;

    [Range(0f, 1f)]
    [SerializeField] private float highThreshold = 0.8f;

    private float timer;

    public CrowdZone[] Zones => zones;

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdateCrowdDensity();
        }
    }

    private void UpdateCrowdDensity()
    {
        PassengerNavigation[] passengers =
            FindObjectsByType<PassengerNavigation>(
                FindObjectsSortMode.None
            );

        foreach (CrowdZone zone in zones)
        {
            if (zone == null || zone.zoneArea == null)
            {
                continue;
            }

            zone.passengerCount = 0;

            foreach (PassengerNavigation passenger in passengers)
            {
                if (passenger == null)
                {
                    continue;
                }

                Vector3 passengerPosition =
                    passenger.transform.position;

                if (zone.zoneArea.bounds.Contains(passengerPosition))
                {
                    zone.passengerCount++;
                }
            }

            zone.occupancyPercentage =
                (float)zone.passengerCount / zone.capacity * 100f;

            float occupancyRatio =
                (float)zone.passengerCount / zone.capacity;

            if (occupancyRatio >= highThreshold)
            {
                zone.densityLevel = DensityLevel.High;
            }
            else if (occupancyRatio >= mediumThreshold)
            {
                zone.densityLevel = DensityLevel.Medium;
            }
            else
            {
                zone.densityLevel = DensityLevel.Low;
            }

            Debug.Log(
                "Crowd Zone: " + zone.zoneId +
                " | Passengers: " + zone.passengerCount +
                " | Capacity: " + zone.capacity +
                " | Occupancy: " +
                zone.occupancyPercentage.ToString("F1") + "%" +
                " | Density: " + zone.densityLevel
            );
        }
    }

    public int GetPassengerCount(string zoneId)
    {
        CrowdZone zone = Array.Find(
            zones,
            z => z != null && z.zoneId == zoneId
        );

        return zone != null ? zone.passengerCount : 0;
    }

    public float GetOccupancyPercentage(string zoneId)
    {
        CrowdZone zone = Array.Find(
            zones,
            z => z != null && z.zoneId == zoneId
        );

        return zone != null ? zone.occupancyPercentage : 0f;
    }

    public DensityLevel GetDensityLevel(string zoneId)
    {
        CrowdZone zone = Array.Find(
            zones,
            z => z != null && z.zoneId == zoneId
        );

        return zone != null ? zone.densityLevel : DensityLevel.Low;
    }
}
