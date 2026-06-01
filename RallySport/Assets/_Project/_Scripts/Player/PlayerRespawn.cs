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
    }

    public void Die()
    {
        if (_isRespawning) return;

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
            spawnPos = Spawner.LastSpawn.transform.position;
            spawnRot = Spawner.LastSpawn.transform.rotation;
        }

        
        transform.position = spawnPos;
        transform.rotation = spawnRot;

        
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
}
