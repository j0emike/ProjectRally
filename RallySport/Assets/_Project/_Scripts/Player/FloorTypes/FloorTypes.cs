using UnityEngine;

public class FloorTypes : MonoBehaviour
{
    private KartController kartController;

    [Header("Detection Settings")]
    [SerializeField] private Transform detectionPoint; // Colócalo muy cerca del suelo, entre las llantas traseras o en el centro del kart
    [SerializeField] private float detectionRadius = 0.6f; // El tamaño del "área circular" de detección
    [SerializeField] private LayerMask surfaceLayers; // Asegúrate de marcar Basic, Mud, Ice y Sand aquí

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
        // Guardará todos los colisionadores que entren en la esfera de detección
        Collider[] hitColliders = Physics.OverlapSphere(detectionPoint.position, detectionRadius, surfaceLayers);

        if (hitColliders.Length > 0)
        {
            // Creamos variables para saber qué capas encontramos dentro de la esfera
            bool foundSpecialSurface = false;
            string chosenLayer = "Basic";

            // Revisamos TODOS los colisionadores que están tocando las llantas/kart
            foreach (Collider col in hitColliders)
            {
                string layerName = LayerMask.LayerToName(col.gameObject.layer);

                // Si encontramos una capa especial, la priorizamos de inmediato y salimos del bucle
                if (layerName == "Mud" || layerName == "Ice" || layerName == "Sand")
                {
                    chosenLayer = layerName;
                    foundSpecialSurface = true;
                    break; // Rompe el ciclo porque ya encontramos la superficie importante
                }
            }

            // Aplicamos los valores de la capa elegida (si no encontró especial, por defecto será "Basic")
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
            // Si el kart vuela o sale de los colisionadores
            ApplySurfaceSettings(basicSurface);
        }
    }

    private void ApplySurfaceSettings(SurfacePreset preset)
    {
        kartController.NormalTraction = preset.normalTraction;
        kartController.DriftTraction = preset.driftTraction;
        kartController.Deceleration = preset.deceleration;
    }

    // Para ver la esfera en la pestaña de Scene y ajustarla perfectamente
    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(detectionPoint.position, detectionRadius);
    }
}
