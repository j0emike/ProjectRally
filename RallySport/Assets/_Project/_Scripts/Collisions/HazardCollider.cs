using UnityEngine;

public class HazardCollider : MonoBehaviour
{
    [SerializeField] private float _timeToSpawn;
    

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.CompareTag("Player"))
        {
            Debug.Log("Car Detected");         
        }
    }

}
