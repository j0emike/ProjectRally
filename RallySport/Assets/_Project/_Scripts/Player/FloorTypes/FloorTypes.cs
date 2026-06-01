using UnityEngine;

public class FloorTypes : MonoBehaviour
{
    private KartController kartController;

    [Header("Detection Settings")]
    [SerializeField] private Transform detectionPoint;
    [SerializeField] private float detectionRadius = 0.6f;
    [SerializeField] private LayerMask surfaceLayers;

    [System.Serializable]
    public struct SurfacePreset
    {
        public string surfaceName;
        public float normalTraction;
        public float driftTraction;
        public float deceleration;
    }

    [Header("Surface Configurations")]
    public SurfacePreset basicSurface = new SurfacePreset { surfaceName = "Basic", normalTraction = 0.92f, driftTraction = 0.82f, deceleration = 10f };
    public SurfacePreset mudSurface = new SurfacePreset { surfaceName = "Mud", normalTraction = 0.60f, driftTraction = 0.50f, deceleration = 35f };
    public SurfacePreset iceSurface = new SurfacePreset { surfaceName = "Ice", normalTraction = 0.99f, driftTraction = 0.95f, deceleration = 2f };
    public SurfacePreset sandSurface = new SurfacePreset { surfaceName = "Sand", normalTraction = 0.75f, driftTraction = 0.65f, deceleration = 20f };

    private void Awake()
    {
        kartController = GetComponent<KartController>();

        if (detectionPoint == null)
        {
            detectionPoint = transform;
        }
    }

    private void FixedUpdate()
    {
        DetectSurface();
    }

    private void DetectSurface()
    {
        Collider[] hitColliders = Physics.OverlapSphere(detectionPoint.position, detectionRadius, surfaceLayers);

        if (hitColliders.Length > 0)
        {
            bool foundSpecialSurface = false;
            string chosenLayer = "Basic";

            foreach (Collider col in hitColliders)
            {
                string layerName = LayerMask.LayerToName(col.gameObject.layer);

                if (layerName == "Mud" || layerName == "Ice" || layerName == "Sand")
                {
                    chosenLayer = layerName;
                    foundSpecialSurface = true;
                    break;
                }
            }

            switch (chosenLayer)
            {
                case "Basic":
                    ApplySurfaceSettings(basicSurface);
                    break;
                case "Mud":
                    ApplySurfaceSettings(mudSurface);
                    break;
                case "Ice":
                    ApplySurfaceSettings(iceSurface);
                    break;
                case "Sand":
                    ApplySurfaceSettings(sandSurface);
                    break;
                default:
                    ApplySurfaceSettings(basicSurface);
                    break;
            }
        }
        else
        {
            ApplySurfaceSettings(basicSurface);
        }
    }

    private void ApplySurfaceSettings(SurfacePreset preset)
    {
        kartController.NormalTraction = preset.normalTraction;
        kartController.DriftTraction = preset.driftTraction;
        kartController.Deceleration = preset.deceleration;
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
    }
}
