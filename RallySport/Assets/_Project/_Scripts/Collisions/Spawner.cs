using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _carPrefab;
    
    public static Spawner LastSpawn;


    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            LastSpawn = this;
            Debug.Log("Car Entered Spawn");         
        }
    }

}
