using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private GameObject _destructionFXPrefab;
    [SerializeField] private float _timeToSpawn;

    // References
    private KartController _kartController;
    private CharacterController _characterController;
    private Renderer[] _renderers;

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private bool _isRespawning = false;

    [Header("Off-Road Detection")]
    [SerializeField] private float _maxOffRoadTime = 1.5f;
    [SerializeField] private float _maxFallDistance = 5f;

    private float _lastRoadHeight;
    private float _offRoadTimer = 0f;

    private void Awake()
    {
        _kartController = GetComponent<KartController>();
        _characterController = GetComponent<CharacterController>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        _lastRoadHeight = transform.position.y;
    }

    public void Die()
    {
        if (_isRespawning) return;
        if (UILevelEndManager.Instance != null && UILevelEndManager.Instance.IsLevelOver) return;

        // Print a warning with the stack trace so we can see exactly what triggered the death
        Debug.LogWarning($"[PlayerRespawn] Die() was called! Stack Trace:\n{System.Environment.StackTrace}");

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        _isRespawning = true;

        
        if (_destructionFXPrefab != null)
        {
            Instantiate(_destructionFXPrefab, transform.position, transform.rotation);
        }

        
        if (_characterController != null) _characterController.enabled = false;
        if (_kartController != null)
        {
            _kartController.ResetVelocity();
            _kartController.enabled = false;
        }

        foreach (Renderer r in _renderers)
        {
            r.enabled = false;
        }

        
        yield return new WaitForSeconds(_timeToSpawn);

        
        Vector3 spawnPos = _startPosition;
        Quaternion spawnRot = _startRotation;

        if (Spawner.LastSpawn != null)
        {
            spawnPos = Spawner.LastSpawn.PlayerEnterPosition;
            spawnRot = Spawner.LastSpawn.PlayerEnterRotation;
        }

        
        transform.position = spawnPos;
        transform.rotation = spawnRot;
        _lastRoadHeight = spawnPos.y;
        _offRoadTimer = 0f;

        
        if (_characterController != null) _characterController.enabled = true;
        if (_kartController != null)
        {
            _kartController.ResetVelocity(); 
            _kartController.enabled = true;
        }

        foreach (Renderer r in _renderers)
        {
            r.enabled = true;
        }

        _isRespawning = false;
    }

    private void FixedUpdate()
    {
        if (_isRespawning) return;

        CheckOffRoad();
    }

    private float _logTimer = 0f;

    private void CheckOffRoad()
    {
        if (UILevelEndManager.Instance != null && UILevelEndManager.Instance.IsLevelOver) return;

        Vector3 origin = transform.position + Vector3.up * 1f;
        bool isAboveRoad = false;

        _logTimer += Time.fixedDeltaTime;
        bool shouldLog = _logTimer >= 0.5f;
        if (shouldLog) _logTimer = 0f;

        // Perform a raycast down to check if we are above the road
        // We query all layers and include triggers in case the road has a trigger collider
        RaycastHit[] hits = Physics.RaycastAll(origin, Vector3.down, 20f, ~0, QueryTriggerInteraction.Collide);
        
        if (shouldLog && hits.Length == 0)
        {
            Debug.LogWarning("[PlayerRespawn] Off-road raycast hit absolutely nothing under the car!");
        }

        foreach (RaycastHit hit in hits)
        {
            // Ignore ourselves and any of our child objects
            if (hit.collider.gameObject == gameObject || hit.collider.transform.IsChildOf(transform))
            {
                continue;
            }

            if (shouldLog)
            {
                Debug.LogWarning($"[PlayerRespawn] Raycast hit object: '{hit.collider.name}' with Tag: '{hit.collider.tag}' (Has 'Road' tag: {IsRoad(hit.collider.transform)})");
            }

            if (IsRoad(hit.collider.transform))
            {
                isAboveRoad = true;
                break;
            }
        }

        if (isAboveRoad)
        {
            _lastRoadHeight = transform.position.y;
            _offRoadTimer = 0f;
        }
        else
        {
            _offRoadTimer += Time.fixedDeltaTime;

            if (transform.position.y < _lastRoadHeight - _maxFallDistance || _offRoadTimer > _maxOffRoadTime)
            {
                Die();
            }
        }
    }

    private bool IsRoad(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Road"))
            {
                return true;
            }
            t = t.parent;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * 20f);
    }
}
