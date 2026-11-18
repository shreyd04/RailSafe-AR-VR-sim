
using UnityEngine;

public class CrowdDensityHeatmap : MonoBehaviour
{
    [System.Serializable]
    public class HeatmapZone
    {
        public string zoneId;
        public Renderer overlayRenderer;
    }

    [Header("References")]
    [SerializeField] private CrowdDensityMonitor densityMonitor;
    [SerializeField] private HeatmapZone[] heatmapZones;

    [Header("Heatmap Colours")]
    [SerializeField] private Color lowColour =
        new Color(0.1f, 0.8f, 0.2f, 0.35f);

    [SerializeField] private Color mediumColour =
        new Color(1f, 0.75f, 0f, 0.45f);

    [SerializeField] private Color highColour =
        new Color(1f, 0.1f, 0.1f, 0.55f);

    private void Update()
    {
        if (densityMonitor == null || heatmapZones == null)
            return;

        foreach (HeatmapZone heatmapZone in heatmapZones)
        {
            if (heatmapZone == null ||
                heatmapZone.overlayRenderer == null)
                continue;

            var level =
                densityMonitor.GetDensityLevel(heatmapZone.zoneId);

            Color selectedColour = lowColour;

            switch (level)
            {
                case CrowdDensityMonitor.DensityLevel.Medium:
                    selectedColour = mediumColour;
                    break;

                case CrowdDensityMonitor.DensityLevel.High:
                    selectedColour = highColour;
                    break;
            }

            heatmapZone.overlayRenderer.material.color =
                selectedColour;
        }
    }
}
