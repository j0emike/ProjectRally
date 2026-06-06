using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _carPrefab;
    [SerializeField] private Transform _spawnPoint;
    
    public static Spawner LastSpawn;

    public Vector3 SpawnPosition => _spawnPoint != null ? _spawnPoint.position : transform.position;
    public Quaternion SpawnRotation => _spawnPoint != null ? _spawnPoint.rotation : transform.rotation;

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            LastSpawn = this;
            Debug.Log("Car Entered Spawn");         
        }
    }
}
