using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _carPrefab;
    
    public static Spawner LastSpawn;

    public Vector3 PlayerEnterPosition { get; private set; }
    public Quaternion PlayerEnterRotation { get; private set; }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            LastSpawn = this;
            PlayerEnterPosition = collider.transform.position;
            PlayerEnterRotation = collider.transform.rotation;
            Debug.Log("Car Entered Spawn");         
        }
    }
}
